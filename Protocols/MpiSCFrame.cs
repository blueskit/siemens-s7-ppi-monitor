namespace S7PpiMonitor.Protocols;

/// <summary>
/// SC：短应答（0xE5）
/// </summary>
public class MpiSCFrame
    : MpiFrame
{
    public override MpiFrameType Header { get; set; } = MpiFrameType.SC;


    public MpiSCFrame()
    {
        Header = (MpiFrameType)0;
    }

    public MpiSCFrame(byte[] source)
    {
        Header = (MpiFrameType)source[0];
    }


    /// <summary>
    /// 当前帧是否格式合法
    /// </summary>
    public override bool IsValid()
    {
        return this.Header == MpiFrameType.SC;
    }

    /// <summary>
    /// 获取帧的实际总字节长度
    /// </summary>
    public override int GetFrameLength()
    {
        return 1;
    }

    public override string ToString()
    {
        return $"SC|Header={Header}";
    }

}
