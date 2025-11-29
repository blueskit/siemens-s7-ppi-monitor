namespace S7PpiMonitor.Forms
{
    partial class S7PPIForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(S7PPIForm));
            toolStrip1 = new ToolStrip();
            toolStripSeparator1 = new ToolStripSeparator();
            btnRefreshPorts = new ToolStripButton();
            toolPortDropDown = new ToolStripDropDownButton();
            toolStripSeparator2 = new ToolStripSeparator();
            btnOpenPort = new ToolStripButton();
            btnClosePort = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            btnSaveVarlist = new ToolStripButton();
            btnLoadVarlist = new ToolStripButton();
            statusStrip1 = new StatusStrip();
            tabs = new TabControl();
            tabPage0 = new TabPage();
            numRefreshInterval = new NumericUpDown();
            chkAutoRefresh = new CheckBox();
            tabPage1 = new TabPage();
            ucVarTotalListView = new ucVarInfoListView();
            groupBox1 = new GroupBox();
            tabPage2 = new TabPage();
            ucVarWriteListView = new ucVarInfoListView();
            timerAutoRefresh = new System.Windows.Forms.Timer(components);
            toolStripSplitButton1 = new ToolStripSplitButton();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripByteCount = new ToolStripStatusLabel();
            toolStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            tabs.SuspendLayout();
            tabPage0.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numRefreshInterval).BeginInit();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(32, 32);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripSeparator1, btnRefreshPorts, toolPortDropDown, toolStripSeparator2, btnOpenPort, btnClosePort, toolStripSeparator3, btnSaveVarlist, btnLoadVarlist });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1239, 39);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 39);
            // 
            // btnRefreshPorts
            // 
            btnRefreshPorts.Image = (Image)resources.GetObject("btnRefreshPorts.Image");
            btnRefreshPorts.ImageTransparentColor = Color.Magenta;
            btnRefreshPorts.Name = "btnRefreshPorts";
            btnRefreshPorts.Size = new Size(75, 36);
            btnRefreshPorts.Text = "串口";
            btnRefreshPorts.Click += btnRefreshPorts_Click;
            // 
            // toolPortDropDown
            // 
            toolPortDropDown.Image = (Image)resources.GetObject("toolPortDropDown.Image");
            toolPortDropDown.ImageTransparentColor = Color.Magenta;
            toolPortDropDown.Name = "toolPortDropDown";
            toolPortDropDown.Size = new Size(115, 36);
            toolPortDropDown.Text = "串口列表";
            toolPortDropDown.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 39);
            // 
            // btnOpenPort
            // 
            btnOpenPort.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnOpenPort.Image = (Image)resources.GetObject("btnOpenPort.Image");
            btnOpenPort.ImageTransparentColor = Color.Magenta;
            btnOpenPort.Name = "btnOpenPort";
            btnOpenPort.Size = new Size(73, 36);
            btnOpenPort.Text = "打开串口";
            btnOpenPort.Click += btnOpenPort_Click;
            // 
            // btnClosePort
            // 
            btnClosePort.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnClosePort.Image = (Image)resources.GetObject("btnClosePort.Image");
            btnClosePort.ImageTransparentColor = Color.Magenta;
            btnClosePort.Name = "btnClosePort";
            btnClosePort.Size = new Size(73, 36);
            btnClosePort.Text = "关闭串口";
            btnClosePort.Click += btnClosePort_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 39);
            // 
            // btnSaveVarlist
            // 
            btnSaveVarlist.Image = (Image)resources.GetObject("btnSaveVarlist.Image");
            btnSaveVarlist.ImageTransparentColor = Color.Magenta;
            btnSaveVarlist.Name = "btnSaveVarlist";
            btnSaveVarlist.Size = new Size(135, 36);
            btnSaveVarlist.Text = "保存变量定义";
            btnSaveVarlist.Click += btnSaveVarlist_Click;
            // 
            // btnLoadVarlist
            // 
            btnLoadVarlist.Image = (Image)resources.GetObject("btnLoadVarlist.Image");
            btnLoadVarlist.ImageTransparentColor = Color.Magenta;
            btnLoadVarlist.Name = "btnLoadVarlist";
            btnLoadVarlist.Size = new Size(135, 36);
            btnLoadVarlist.Text = "载入变量定义";
            btnLoadVarlist.Click += btnLoadVarlist_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripSplitButton1, toolStripStatusLabel1, toolStripByteCount });
            statusStrip1.Location = new Point(0, 580);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1239, 26);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // tabs
            // 
            tabs.Alignment = TabAlignment.Bottom;
            tabs.Controls.Add(tabPage0);
            tabs.Controls.Add(tabPage1);
            tabs.Controls.Add(tabPage2);
            tabs.Dock = DockStyle.Fill;
            tabs.Location = new Point(0, 39);
            tabs.Multiline = true;
            tabs.Name = "tabs";
            tabs.SelectedIndex = 0;
            tabs.Size = new Size(1239, 541);
            tabs.TabIndex = 2;
            // 
            // tabPage0
            // 
            tabPage0.Controls.Add(numRefreshInterval);
            tabPage0.Controls.Add(chkAutoRefresh);
            tabPage0.Location = new Point(4, 4);
            tabPage0.Name = "tabPage0";
            tabPage0.Size = new Size(1231, 508);
            tabPage0.TabIndex = 2;
            tabPage0.Text = "概要";
            tabPage0.UseVisualStyleBackColor = true;
            // 
            // numRefreshInterval
            // 
            numRefreshInterval.Enabled = false;
            numRefreshInterval.Location = new Point(223, 76);
            numRefreshInterval.Name = "numRefreshInterval";
            numRefreshInterval.Size = new Size(107, 27);
            numRefreshInterval.TabIndex = 1;
            numRefreshInterval.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numRefreshInterval.ValueChanged += numRefreshInterval_ValueChanged;
            // 
            // chkAutoRefresh
            // 
            chkAutoRefresh.AutoSize = true;
            chkAutoRefresh.Location = new Point(62, 79);
            chkAutoRefresh.Name = "chkAutoRefresh";
            chkAutoRefresh.Size = new Size(146, 24);
            chkAutoRefresh.TabIndex = 0;
            chkAutoRefresh.Text = "启用自动刷新(秒)";
            chkAutoRefresh.UseVisualStyleBackColor = true;
            chkAutoRefresh.CheckedChanged += chkAutoRefresh_CheckedChanged;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(ucVarTotalListView);
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Location = new Point(4, 4);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1231, 512);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "所有变量";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // ucVarTotalListView
            // 
            ucVarTotalListView.Dock = DockStyle.Fill;
            ucVarTotalListView.LabelEdit = true;
            ucVarTotalListView.LabelWrap = false;
            ucVarTotalListView.Location = new Point(3, 3);
            ucVarTotalListView.Name = "ucVarTotalListView";
            ucVarTotalListView.Size = new Size(973, 506);
            ucVarTotalListView.TabIndex = 1;
            ucVarTotalListView.UseCompatibleStateImageBehavior = false;
            ucVarTotalListView.View = View.Details;
            // 
            // groupBox1
            // 
            groupBox1.Dock = DockStyle.Right;
            groupBox1.Location = new Point(976, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(252, 506);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(ucVarWriteListView);
            tabPage2.Location = new Point(4, 4);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1231, 512);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "写变量";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // ucVarWriteListView
            // 
            ucVarWriteListView.Dock = DockStyle.Fill;
            ucVarWriteListView.LabelEdit = true;
            ucVarWriteListView.LabelWrap = false;
            ucVarWriteListView.Location = new Point(3, 3);
            ucVarWriteListView.Name = "ucVarWriteListView";
            ucVarWriteListView.Size = new Size(1225, 506);
            ucVarWriteListView.TabIndex = 2;
            ucVarWriteListView.UseCompatibleStateImageBehavior = false;
            ucVarWriteListView.View = View.Details;
            // 
            // timerAutoRefresh
            // 
            timerAutoRefresh.Tick += timerAutoRefresh_Tick;
            // 
            // toolStripSplitButton1
            // 
            toolStripSplitButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripSplitButton1.Image = (Image)resources.GetObject("toolStripSplitButton1.Image");
            toolStripSplitButton1.ImageTransparentColor = Color.Magenta;
            toolStripSplitButton1.Name = "toolStripSplitButton1";
            toolStripSplitButton1.Size = new Size(39, 24);
            toolStripSplitButton1.Text = "toolStripSplitButton1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(39, 20);
            toolStripStatusLabel1.Text = "读取";
            // 
            // toolStripByteCount
            // 
            toolStripByteCount.ForeColor = Color.Blue;
            toolStripByteCount.Name = "toolStripByteCount";
            toolStripByteCount.Size = new Size(54, 20);
            toolStripByteCount.Text = "字节数";
            // 
            // S7PPIForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1239, 606);
            Controls.Add(tabs);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            DoubleBuffered = true;
            Name = "S7PPIForm";
            Text = "西门子S7-PPI协议监听分析";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            tabs.ResumeLayout(false);
            tabPage0.ResumeLayout(false);
            tabPage0.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numRefreshInterval).EndInit();
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip1;
        private StatusStrip statusStrip1;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton btnOpenPort;
        private ToolStripButton btnClosePort;
        private ToolStripDropDownButton toolPortDropDown;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton btnRefreshPorts;
        private TabControl tabs;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage0;
        private ucVarInfoListView ucVarTotalListView;
        private CheckBox chkAutoRefresh;
        private NumericUpDown numRefreshInterval;
        private System.Windows.Forms.Timer timerAutoRefresh;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton btnSaveVarlist;
        private ToolStripButton btnLoadVarlist;
        private GroupBox groupBox1;
        private ucVarInfoListView ucVarWriteListView;
        private ToolStripSplitButton toolStripSplitButton1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripByteCount;
    }
}