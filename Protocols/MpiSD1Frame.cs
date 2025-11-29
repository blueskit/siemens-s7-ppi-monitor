using System.Diagnostics;

namespace S7PpiMonitor.Protocols;

/// <summary>
/// SD1 消息类型
/// </summary>
public class MpiSD1Frame
    : MpiFrame
{
    public override MpiFrameType Header { get; set; } = MpiFrameType.SD1;

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
    ///   -- 请求：49（FDL查询），5C、7C（交替）
    ///   -- 应答：02（无资源），03（无服务）
    ///   -- FDL应答：00（从站），01（主站未就绪），02（主站已就绪） 03（主站已在令牌）
    /// </summary>
    public byte FC { get; set; }

    /// <summary>
    /// 帧校验（DA到FC的数据和）
    /// </summary>
    public byte FCS { get; set; }

    /// <summary>
    /// 帧结束（固定：0x16）
    /// </summary>
    public byte ED { get; set; } = 0x16;


    public MpiSD1Frame()
    {
        Header = (MpiFrameType)0;
    }

    public MpiSD1Frame(byte[] source)
    {
        Debug.Assert(source.Length >= 6);

        int pos = 0;

        this.Header = (MpiFrameType)source[pos++];
        this.DA = source[pos++];
        this.SA = source[pos++];
        this.FC = source[pos++];
        this.FCS = source[pos++];
        this.ED = source[pos++];
    }

    /// <summary>
    /// 获取帧的实际总字节长度
    /// </summary>
    public override int GetFrameLength()
    {
        return 6;
    }

    public override byte MakeFCS()
    {
        return (byte)((DA + SA + FC) & 0xFF);
    }

    /// <summary>
    /// 当前帧是否格式合法
    /// </summary>
    public override bool IsValid()
    {
        return this.Header == MpiFrameType.SD1 &&
            this.ED == 0x16 &&
            this.FCS == MakeFCS();
    }

    public override string ToString()
    {
        return $"SD1|Header={Header},DA={DA},SA={SA},FC={FC},FCS={FCS},ED={ED}";
    }

}
