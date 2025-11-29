using System.Diagnostics;

namespace S7PpiMonitor.Protocols;

/// <summary>
/// SD2 消息类型
/// </summary>
public class PpiSD2Frame
    : MpiFrame
{
    public byte[] _rawFrameBuff { get; private set; }

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
    /// </summary>
    public byte FC { get; set; }

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
    /// 根据解析得到的PDU的服务类型
    /// </summary>
    public PpiServiceId ServiceId { get; set; } = PpiServiceId.Undefined;

    /// <summary>
    /// 解析后的元素列表
    /// </summary>
    public override List<PpiVarElement> Elements { get; protected set; }


    public PpiSD2Frame()
    {
        _rawFrameBuff = new byte[0];
        Header = (MpiFrameType)0;
        Elements = new List<PpiVarElement>();
    }

    public PpiSD2Frame(byte[] source)
    {
        Debug.Assert(source.Length > 10);

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

        var dataUnitLen = this.LE - 3;
        this.DataUnit = new byte[dataUnitLen];

        for (int i = 0; i < dataUnitLen; i++)
            this.DataUnit[i] = source[pos++];

        this.FCS = source[pos++];
        this.ED = source[pos++];

        _rawFrameBuff = new byte[pos];
        Array.Copy(source, _rawFrameBuff, pos);
    }

    /// <summary>
    /// 缓冲区内数据，是否满足帧长度要求？
    /// </summary>
    public static bool IsMeetFrameLength(byte[] source)
    {
        if (source is null || source.Length < 10)
            return false;

        int pos = 0;

        var header = (MpiFrameType)source[pos++];
        var LE = source[pos++];
        var LEr = source[pos++];
        var SD2r = source[pos++];
        var DA = source[pos++];
        var SA = source[pos++];
        var FC = source[pos++];

        return header == MpiFrameType.SD2 &&
            LE == LEr &&
            SD2r == (byte)MpiFrameType.SD2 &&
            source.Length > (LE + 9);
    }

    /// <summary>
    /// 根据PDU数据解析，并创建对应的PDU对象
    /// </summary>
    public override bool ParsePDU()
    {
        var pdu = new PpiSD2PDU(this.DataUnit);
        pdu.TryParse();

        this.ServiceId = pdu.ServiceId;
        this.Elements = pdu.Elements;

        return true;
    }

    /// <summary>
    /// 获取帧的实际总字节长度
    /// </summary>
    public override int GetFrameLength()
    {
        return 9 + DataUnit.Length;
    }

    public override byte MakeFCS()
    {
        byte crc = (byte)((SD2r ^ DA ^ SA ^ FC) & 0xFF);

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
            this.ED == 0x16;
    }

    public override string ToString()
    {
        var duString = string.Join(" ", DataUnit.Select(b => b.ToString("X2")));

        var elements = new List<string>();
        foreach (var c in Elements) {
            if (c.Values is null || c.Values.Count == 0)
                elements.Add($"({c.GetVarName()},{c.Count})");
            else
                elements.Add($"({c.GetVarName()},{c.Count})=[{string.Join(",", c.Values)}]");
        }
        var elementsText = string.Join(" ", elements);

        return $"SD2|Header={Header:X},LEN={LE:X2}H,DA={DA},SA={SA},FC={FC:X},FCS={FCS:X},ED={ED:X},DU=[{duString}] => [{elementsText}]";
    }
}
