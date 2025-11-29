using System.Diagnostics;

namespace S7PpiMonitor.Protocols;

public class MpiSD4Frame
    : MpiFrame
{
    public override MpiFrameType Header { get; set; } = MpiFrameType.SD4;

    /// <summary>
    /// 目的地址
    /// </summary>
    public byte DA { get; set; }
    /// <summary>
    /// 源地址
    /// </summary>
    public byte SA { get; set; }


    public MpiSD4Frame()
    {
        Header = (MpiFrameType)0;
    }

    public MpiSD4Frame(byte[] source)
    {
        Debug.Assert(source.Length >= 3);

        int pos = 0;

        this.Header = (MpiFrameType)source[pos++];
        this.DA = source[pos++];
        this.SA = source[pos++];
    }

    /// <summary>
    /// 获取帧的实际总字节长度
    /// </summary>
    public override int GetFrameLength()
    {
        return 3;
    }

    public override string ToString()
    {
        return $"SD4|Header={Header},DA={DA},SA={SA}";
    }

}
