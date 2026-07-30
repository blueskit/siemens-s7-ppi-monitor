using System.Diagnostics;

using Microsoft.Extensions.Logging;

using S7PpiMonitor.Common;
using S7PpiMonitor.Models;
using S7PpiMonitor.MpiProtocols;

using SharpPcap;

namespace S7PpiMonitor.BackService;

public class S7CommunicationAnalyzingService
{
    public static S7CommunicationAnalyzingService Instance { get; } = new S7CommunicationAnalyzingService();

    public ILiveDevice _device;

    /// PLC地址(作为DA或SA), 一般为2、3等
    public string PlcIpAddr { get; set; }

    /// <summary>
    /// 网卡上读取到的字节数
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

    public S7BufferManager BufferHelper { get; private set; } = new S7BufferManager();

    private Microsoft.Extensions.Logging.ILogger _logger;

    private S7CommunicationAnalyzingService()
    {
        _device = null;
        PlcIpAddr = string.Empty;
        VarList = new VarInfoList();
        WriteVarList = new VarInfoList();

        BufferHelper = new S7BufferManager();
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

        return Task.CompletedTask;
    }
    #endregion

    private void excute_loopings(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested) {

            if (_device is null || !_device.Started) {
                Thread.Sleep(1000);
                break;
            }

            // 获取已经抓到的报文,并解析
            Thread.Sleep(100);
            while (BufferHelper.TryGetFrame(out var ipPacket)) {
                if (ipPacket is null)
                    break;

                //updateVarList(VarList, frame, writeOnly: false);
                //updateVarList(WriteVarList, frame, writeOnly: true);

                _logger.LogInformation($"Got a sourceFrame: {ipPacket}");
            }
        }
    }

    /// <summary>
    /// 根据解析到的帧及数据，更新变量列表
    /// </summary>
    private void updateVarList(VarInfoList targetVarList, MpiFrame sourceFrame, bool writeOnly = false)
    {

    }

    internal void device_OnPacketArrival(object sender, PacketCapture e)
    {
        var time = e.Header.Timeval.Date;
        var len = e.Data.Length;
        var rawPacket = e.GetPacket();

        var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

        var ipPacket = packet.Extract<PacketDotNet.IPPacket>();
        var tcpPacket = packet.Extract<PacketDotNet.TcpPacket>();

        if (tcpPacket != null) {
            System.Net.IPAddress srcIp = ipPacket.SourceAddress;
            System.Net.IPAddress dstIp = ipPacket.DestinationAddress;
            int srcPort = tcpPacket?.SourcePort ?? 0;
            int dstPort = tcpPacket?.DestinationPort ?? 0;

            Debug.WriteLine("{0}:{1}:{2},{3} Len={4} {5}:{6} -> {7}:{8}, {9}",
                time.Hour, time.Minute, time.Second, time.Millisecond, len,
                srcIp, srcPort, dstIp, dstPort,
                tcpPacket?.PayloadData?.ToHexString() ?? string.Empty);
        }
    }
}
