using System.IO.Ports;
using System.Text;
using Microsoft.Extensions.Logging;
using S7PpiMonitor.Models;
using S7PpiMonitor.Protocols;

namespace S7PpiMonitor.BackService;

public class S7PpiCommunicationAnalyzingService
{
    public static S7PpiCommunicationAnalyzingService Instance { get; } = new S7PpiCommunicationAnalyzingService();

    private SerialPort _serialPort;

    public bool SerialIsOpen => _serialPort?.IsOpen ?? false;

    /// <summary>
    /// 串口名称
    /// </summary>
    public string PortName { get; set; } = "COM1";

    /// <summary>
    /// 上位机或HMI地址(作为DA或SA), 一般为0
    /// </summary>
    public byte MasterAddr { get; set; } = 0;
    /// PLC地址(作为DA或SA), 一般为2、3等
    public byte SlavaAddr { get; set; } = 3;

    /// <summary>
    /// 串口上读取到的字节数
    /// </summary>
    public int ReadedByteCount { get; private set; } = 0;

    /// <summary>
    /// 所有变量列表
    /// </summary>
    public VarInfoList VarList { get; private set; } = new VarInfoList();

    /// <summary>
    /// 写变量列表
    /// </summary>
    public VarInfoList WriteVarList { get; private set; } = new VarInfoList();

    public BufferManager BufferHelper { get; private set; } = new BufferManager();

    private Microsoft.Extensions.Logging.ILogger _logger;

    private S7PpiCommunicationAnalyzingService()
    {
        _serialPort = default;

        MasterAddr = 0;
        SlavaAddr = 3;
        VarList = new VarInfoList();
        WriteVarList = new VarInfoList();

        BufferHelper = new BufferManager();
    }

    public void SetLogger(Microsoft.Extensions.Logging.ILogger logger)
    {
        _logger = logger;
    }

    #region IHostedService
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.Factory.StartNew(() => {
            excute_loopings(cancellationToken);
        }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Task.Delay(1000).Wait();

        closeSerialPort();

        return Task.CompletedTask;
    }
    #endregion


    private void excute_loopings(CancellationToken cancellationToken)
    {
        openSerialPort();

        while (!cancellationToken.IsCancellationRequested) {

            if (_serialPort is null || !_serialPort.IsOpen)
                break;

            // 读取数据   
            var buff = readSerialPort();
            if (buff.Length > 0) {
                // 寻找 “68 XX XX 68” 形式的字节，并在其之前插入换行
                var logText = new StringBuilder();
                for (int i = 0; i < buff.Length - 4; i++) {
                    if (buff[i] == 0x68 && buff[i + 1] == buff[i + 2] && buff[i + 3] == 0x68) {
                        logText.AppendLine();
                        logText.Append("\t");
                    }
                    logText.Append(buff[i].ToString("X2"));
                    logText.Append(" ");
                }
                if (logText.ToString().StartsWith("\r\n\t"))
                    logText.Remove(0, 3);

                _logger.LogInformation($"RECV HEX/{buff.Length,4:#} <<<\r\n\t" + logText);

                // 准备解析
                BufferHelper.WriteBytes(buff);
            }

            ReadedByteCount += buff.Length;

            // 尝试解析帧
            while (BufferHelper.TryGetFrame(out var frame)) {
                if (frame is null)
                    break;
                if (!frame.IsValid()) {
                    _logger.LogWarning($"残缺帧: {frame}");
                    break;
                }

                frame.ParsePDU();

                updateVarList(VarList, frame, writeOnly: false);
                updateVarList(WriteVarList, frame, writeOnly: true);

                _logger.LogInformation($"Got a sourceFrame: {frame}");
            }
        }

        closeSerialPort();
    }

    /// <summary>
    /// 根据解析到的帧及数据，更新变量列表
    /// </summary>
    private void updateVarList(VarInfoList targetVarList, MpiFrame sourceFrame, bool writeOnly = false)
    {
        var frame = sourceFrame as PpiSD2Frame;
        if (frame is null)
            return;

        if (frame.Elements is null || frame.Elements.Count == 0)
            return;

        // 下发（查询命令或写值）
        if (frame.SA == 0 && frame.ServiceId == PpiServiceId.Write) {
            foreach (var element in frame.Elements) {
                if (element is null || element.Values is null || element.Values.Count == 0)
                    continue;

                for (var i = 0; i < element.Values.Count; i++) {
                    var varName = element.GetVarName(i);
                    var val = element.Values[i];

                    if (targetVarList.TryGetByPathName(varName, out var v)) {
                        var changed = !(val.Equals(v.Value));

                        v.Value = val;
                        v.ReadedAt = DateTime.Now;

                        if (changed)
                            v.ChangedAt = DateTime.Now;

                        if (element.WordValues.Count > i)
                            v.WordValue = element.WordValues[i];
                        if (element.DWordValues.Count > i)
                            v.DWordValue = element.DWordValues[i];
                        if (element.RealValues.Count > i)
                            v.RealValue = element.RealValues[i];

                    } else {
                        v = new VarInfoModel() {
                            PathName = varName,
                            Title = varName,
                            Value = val,
                            AddedAt = DateTime.Now,
                            ReadedAt = DateTime.Today,
                            ChangedAt = DateTime.Today,
                        };

                        if (element.Type == PpiVarType.Byte) {
                            if (element.WordValues.Count > i)
                                v.WordValue = element.WordValues[i];
                            if (element.DWordValues.Count > i)
                                v.DWordValue = element.DWordValues[i];
                            if (element.RealValues.Count > i)
                                v.RealValue = element.RealValues[i];
                        }

                        targetVarList.Add(v);
                    }
                }
            }
        }

        if (writeOnly)
            return;

        // 上传（查询结果反馈）
        if (frame.DA == 0) {
            foreach (var element in frame.Elements) {
                if (element is null || element.Values is null || element.Values.Count == 0)
                    continue;

                for (var i = 0; i < element.Values.Count; i++) {
                    var varName = element.GetVarName(i);
                    var val = element.Values[i];

                    if (varName.StartsWith("I0"))
                        Console.WriteLine(varName);

                    if (targetVarList.TryGetByPathName(varName, out var v)) {
                        var changed = !(val.Equals(v.Value));

                        v.Value = val;
                        v.ReadedAt = DateTime.Now;

                        if (changed)
                            v.ChangedAt = DateTime.Now;

                        if (element.WordValues.Count > i)
                            v.WordValue = element.WordValues[i];
                        if (element.DWordValues.Count > i)
                            v.DWordValue = element.DWordValues[i];
                        if (element.RealValues.Count > i)
                            v.RealValue = element.RealValues[i];

                    } else {
                        v = new VarInfoModel() {
                            PathName = varName,
                            Title = varName,
                            Value = val,
                            AddedAt = DateTime.Now,
                            ReadedAt = DateTime.Today,
                            ChangedAt = DateTime.Today,
                        };

                        if (element.WordValues.Count > i)
                            v.WordValue = element.WordValues[i];
                        if (element.DWordValues.Count > i)
                            v.DWordValue = element.DWordValues[i];
                        if (element.RealValues.Count > i)
                            v.RealValue = element.RealValues[i];

                        targetVarList.Add(v);
                    }
                }
            }
        }
    }

    private void openSerialPort()
    {
        _serialPort = new SerialPort();
        _serialPort.PortName = PortName ?? "COM1";
        _serialPort.BaudRate = 9600;
        _serialPort.DataBits = 8;
        _serialPort.Parity = Parity.Even;
        _serialPort.StopBits = StopBits.One;

        _serialPort.ReadTimeout = 1000;
        _serialPort.WriteTimeout = 1000;

        _serialPort.ReadBufferSize = 102400;
        _serialPort.WriteBufferSize = 4096;

        try {
            _serialPort.Open();
        } catch (Exception ex) {
            _serialPort = null;
        }
    }

    private void closeSerialPort()
    {
        if (_serialPort is not null && _serialPort.IsOpen) {
            _serialPort.Close();
        }
    }

    private byte[] readSerialPort()
    {
        var len = _serialPort.BytesToRead;
        if (len > 100) {
            var buffer = new byte[len];
            _serialPort.Read(buffer, 0, len);

            return buffer;
        }

        return Array.Empty<byte>();
    }
}