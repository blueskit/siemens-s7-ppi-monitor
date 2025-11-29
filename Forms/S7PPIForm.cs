using System.IO.Ports;
using System.Management;
using S7PpiMonitor.BackService;
using S7PpiMonitor.Models;
using S7PpiMonitor.Utilities;

namespace S7PpiMonitor.Forms;

public partial class S7PPIForm : Form
{
    /// <summary>
    /// 所有变量信息列表(包含读写)
    /// </summary>
    public VarInfoList VarTotalList { get; private set; }

    /// <summary>
    /// 上一次刷新时 VarTotalList 长度
    /// </summary>
    private int VarTotalListCount = 0;

    /// <summary>
    /// 最近写入变量信息列表(临时)
    /// </summary>
    public VarInfoList VarWritedList { get; private set; }

    /// <summary>
    /// 上一次刷新时 VarWritedList 长度
    /// </summary>
    private int VarWritedListCount = 0;

    private CancellationTokenSource _cts = new CancellationTokenSource();
    private S7PpiCommunicationAnalyzingService _commService;

    public S7PPIForm()
    {
        InitializeComponent();

        loadVarList();
        initUserControls();

        this.btnOpenPort.Enabled = true;
        this.btnClosePort.Enabled = false;
    }

    protected override void OnActivated(EventArgs e)
    {
        loadPorts();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_commService is not null)
            _commService.StopAsync(_cts.Token);

        base.OnFormClosing(e);
    }

    private void btnRefreshPorts_Click(object sender, EventArgs e)
    {
        this.toolPortDropDown.DropDownItems.Clear();
        loadPorts();
    }

    private void loadPorts()
    {
        if (this.toolPortDropDown.DropDownItems.Count > 0)
            return;

        var aliasNames = getDevicePortName();

        string[] ports = SerialPort.GetPortNames();
        this.toolPortDropDown.Text = ports.Length > 0 ? ports[0] : "No COM";

        this.toolPortDropDown.DropDownItems.Clear();
        foreach (string port in ports) {

            var aliasName = aliasNames.FirstOrDefault(x => x.Contains(port)) ?? string.Empty;

            var item = new ToolStripMenuItem(port);
            item.Name = port;
            item.Text = port + ": " + aliasName;
            item.Click += (s, e) => {
                this.toolPortDropDown.Text = port + ": " + aliasName;
            };
            this.toolPortDropDown.DropDownItems.Add(item);
        }
    }

    private void initUserControls()
    {
        numRefreshInterval.Enabled = chkAutoRefresh.Checked;

        ucVarTotalListView.VarList = VarTotalList;
        ucVarTotalListView.RefreshVarListView();

        ucVarWriteListView.VarList = VarWritedList;
        ucVarWriteListView.RefreshVarListView();

        this.toolStripByteCount.Text = "0";

    }

    private void loadVarList()
    {
        VarTotalList = S7PpiCommunicationAnalyzingService.Instance.VarList;
        VarWritedList = S7PpiCommunicationAnalyzingService.Instance.WriteVarList;
    }

    private void chkAutoRefresh_CheckedChanged(object sender, EventArgs e)
    {
        var interval = (int)numRefreshInterval.Value;
        if (interval <= 0) {
            numRefreshInterval.Value = 1;
            numRefreshInterval.Refresh();
        }

        numRefreshInterval_ValueChanged(sender, e);
    }

    private void numRefreshInterval_ValueChanged(object sender, EventArgs e)
    {
        var interval = (int)numRefreshInterval.Value;
        if (interval <= 0) {
            this.chkAutoRefresh.Checked = false;
            timerAutoRefresh.Enabled = false;
            return;
        }

        numRefreshInterval.Enabled = chkAutoRefresh.Checked;

        if (chkAutoRefresh.Checked) {
            timerAutoRefresh.Interval = interval * 1000;
            timerAutoRefresh.Start();

            loadVarList();
            initUserControls();
        } else {
            timerAutoRefresh.Stop();
        }
    }

    /// <summary>
    /// 刷新变量最新值
    /// </summary>
    private void timerAutoRefresh_Tick(object sender, EventArgs e)
    {
        if (VarTotalList.Count != VarTotalListCount) {
            VarTotalListCount = VarTotalList.Count;
            VarTotalList.SortByVarName();
            ucVarTotalListView.RefreshVarListView();
        } else
            ucVarTotalListView.UpdateVarListView();

        if (VarWritedList.Count != VarWritedListCount) {
            VarWritedListCount = VarWritedList.Count;
            VarWritedList.SortByVarName();
            ucVarWriteListView.RefreshVarListView();
        } else
            ucVarWriteListView.UpdateVarListView();

        this.toolStripByteCount.Text = S7PpiCommunicationAnalyzingService.Instance.ReadedByteCount.ToString("#,##0 B");
    }

    private void btnOpenPort_Click(object sender, EventArgs e)
    {
        if (_commService is null) {
            _commService = S7PpiCommunicationAnalyzingService.Instance;
            _commService.SetLogger(LoggerWriter.Logger);
        } else {
            _commService.StopAsync(_cts.Token);
        }

        _cts = new CancellationTokenSource();

        var portName = (this.toolPortDropDown.Text ?? "").Trim();
        if (portName.IndexOf(":") > 0)
            portName = portName.Substring(0, portName.IndexOf(":")).Trim();
        if (portName.IndexOf(" ") > 0)
            portName = portName.Substring(0, portName.IndexOf(" ")).Trim();

        _commService.PortName = portName;

        _commService.StartAsync(_cts.Token);

        if (!chkAutoRefresh.Checked) {
            numRefreshInterval.Value = 1;
            chkAutoRefresh.Checked = true;
            chkAutoRefresh_CheckedChanged(sender, e);
        }

        if (this.tabs.SelectedIndex != 1)
            this.tabs.SelectedIndex = 1;

        this.btnOpenPort.Enabled = !_commService.SerialIsOpen;
        this.btnClosePort.Enabled = _commService.SerialIsOpen;
    }

    private void btnClosePort_Click(object sender, EventArgs e)
    {
        if (_commService is null)
            return;

        _commService.StopAsync(_cts.Token);

        this.btnOpenPort.Enabled = !_commService.SerialIsOpen;
        this.btnClosePort.Enabled = _commService.SerialIsOpen;
    }

    private void btnSaveVarlist_Click(object sender, EventArgs e)
    {
        var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        var dlg = new SaveFileDialog();
        dlg.Title = "保存当前变量信息";
        dlg.DefaultExt = "json";
        dlg.AddExtension = true;
        dlg.Filter = "Json文件|*.json||";
        dlg.FileName = $"plc-{DateTime.Today:yyyyMMdd}.json";
        dlg.OverwritePrompt = true;
        dlg.InitialDirectory = dir;

        if (dlg.ShowDialog() == DialogResult.OK) {
            var fileName = dlg.FileName;

            var file = new VarConfigFile();
            file.CreateWith(this.VarTotalList);
            file.UpdateWith(this.VarWritedList);

            var writer = new VariableFileReader();
            writer.WriteToFile(fileName);
        }
    }

    private void btnLoadVarlist_Click(object sender, EventArgs e)
    {
        var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        var dlg = new OpenFileDialog();
        dlg.Title = "读取当前变量信息";
        dlg.DefaultExt = "json";
        dlg.AddExtension = true;
        dlg.Filter = "Json文件|*.json||";
        dlg.FileName = string.Empty;
        dlg.InitialDirectory = dir;

        if (dlg.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dlg.FileName)) {

            var fileName = dlg.FileName;

            var writer = new VariableFileReader();
            writer.ReadFromFile(fileName);
        }
    }

    #region 杂项函数
    private List<string> getDevicePortName()
    {
        List<string> list = new List<string>();

        using (ManagementObjectSearcher searcher = new ManagementObjectSearcher
            ("select * from Win32_PnPEntity where Name like '%(COM%'")) {
            var hardInfos = searcher.Get();
            foreach (var hardInfo in hardInfos) {
                if (hardInfo.Properties["Name"].Value != null) {
                    string deviceName = hardInfo.Properties["Name"].Value.ToString();
                    if (string.IsNullOrEmpty(deviceName))
                        continue;
                    list.Add(deviceName);
                }
            }
        }
        return list;
    }
    #endregion
}
