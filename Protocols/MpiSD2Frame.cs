using System.Diagnostics;
using S7PpiMonitor.Models;

namespace S7PpiMonitor.Protocols;

/// <summary>
/// SD2 消息类型
/// </summary>
public class MpiSD2Frame
    : MpiFrame
{
    public override MpiFrameType Header { get; set; } = MpiFrameType.SD2;

    /// <summary>
    /// 长度(从DA到DataUnit的字节数)
    /// </summary>
    public byte LE { get; set; }

    /// <summary>
    /// 长度重复
    /// </summary>
    public byte LEr { get; set; }

    /// <summary>
    /// 重复Header
    /// </summary>
    public byte SD2r { get; set; }

    /// <summary>
    /// 目的地址
    /// </summary>
    public byte DA { get; set; }
    /// <summary>
    /// 源地址
    /// </summary>
    public byte SA { get; set; }

    /// <summary>
    /// 帧控制
    ///   帧控制，6C（首帧），5C、7C（交替
    /// </summary>
    public byte FC { get; set; }
    /// <summary>
    /// 目的设备的服务点（SAP）
    /// </summary>
    public byte DSAP { get; set; }
    /// <summary>
    /// 来源设备的服务点
    /// </summary>
    public byte SSAP { get; set; }

    /// <summary>
    /// 数据内容（层7信息）
    /// </summary>
    public byte[] DataUnit { get; set; }

    /// <summary>
    /// 帧校验（DA到DataUnit的数据和）
    /// </summary>
    public byte FCS { get; set; }

    /// <summary>
    /// 帧结束（固定：0x16）
    /// </summary>
    public byte ED { get; set; } = 0x16;

    /// <summary>
    /// 解析后的元素列表
    /// </summary>
    public override List<PpiVarElement> Elements { get; protected set; }

    public MpiSD2Frame()
    {
        Header = (MpiFrameType)0;
        Elements = new List<PpiVarElement>();
    }

    public MpiSD2Frame(byte[] source)
    {
        Debug.Assert(source.Length > 30);

        Elements = new List<PpiVarElement>();

#if DEBUG
        var pos16H = source.ToList().FindIndex(b => b == 0x16);
#endif

        int pos = 0;

        this.Header = (MpiFrameType)source[pos++];
        this.LE = source[pos++];
        this.LEr = source[pos++];
        this.SD2r = source[pos++];
        this.DA = source[pos++];
        this.SA = source[pos++];
        this.FC = source[pos++];

        this.DSAP = source[pos++];
        this.SSAP = source[pos++];

        var dataUnitLen = this.LE - 5;
        this.DataUnit = new byte[dataUnitLen];

        for (int i = 0; i < dataUnitLen; i++)
            this.DataUnit[i] = source[pos++];

        this.FCS = source[pos++];
        this.ED = source[pos++];
    }

    /// <summary>
    /// 获取帧的实际总字节长度
    /// </summary>
    public override int GetFrameLength()
    {
        return 9 + 2 + DataUnit.Length;
    }

    public override byte MakeFCS()
    {
        byte crc = (byte)((SD2r ^ DA ^ SA ^ FC ^ DSAP ^ SSAP) & 0xFF);

        foreach (var c in DataUnit)
            crc ^= c;

        return crc;
    }

    /// <summary>
    /// 当前帧是否格式合法
    /// </summary>
    public override bool IsValid()
    {
        return this.Header == MpiFrameType.SD2 &&
            this.ED == 0x16 &&
            this.FCS == MakeFCS();
    }

    public override string ToString()
    {
        var duString = string.Join(" ", DataUnit.Select(b => b.ToString("X2")));
        return $"SD2|Header={Header:X},LEN={LE:X2}H,DA={DA},SA={SA},FC={FC:X},FCS={FCS:X},ED={ED:X},DU=[{duString}]";
    }
}
