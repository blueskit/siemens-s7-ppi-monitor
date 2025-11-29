using S7PpiMonitor.Models;

namespace S7PpiMonitor.Protocols;


public enum MpiFrameType : byte
{
    SC = 0xE5,
    SD1 = 0x10,
    SD2 = 0x68,
    SD4 = 0xDC,
}

public abstract class MpiFrame
{
    public abstract MpiFrameType Header { get; set; }

    /// <summary>
    /// 解析后的元素列表
    /// </summary>
    public virtual List<PpiVarElement> Elements { get; protected set; } = new List<PpiVarElement>();

    /// <summary>
    /// 获取帧的实际总字节长度
    /// </summary>
    public virtual int GetFrameLength()
    {
        return 0;
    }

    /// <summary>
    /// 计算帧校验并返回值
    /// </summary>
    public virtual byte MakeFCS()
    {
        return 0;
    }

    /// <summary>
    /// 当前帧是否格式合法
    /// </summary>
    public virtual bool IsValid()
    {
        return true;
    }

    /// <summary>
    /// 根据PDU数据解析，并创建对应的PDU对象
    /// </summary>
    public virtual bool ParsePDU()
    {
        return false;
    }
}
