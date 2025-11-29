using System.Diagnostics;
using S7PpiMonitor.Models;

namespace S7PpiMonitor.Protocols;

public enum PpiVarType : byte
{
    Bit = 0x01,
    Byte = 0x02,
    Word = 0x04,
    DWord = 0x06,
    Counter = 0x1E,
    Timer = 0x1F,
    HSCounter = 0x20,
    Unknown = 0xFF
}

public enum PpiArea : byte
{
    SystemMarker = 0x04,// SM
    AnalogInput = 0x05, // AI
    AnalogOutput = 0x06,// AQ
    Counter = 0x1E,     // C
    Timer = 0x1F,       // T
    HSCounter = 0x20,   // HC
    Input = 0x81,       // I
    Output = 0x82,      // Q
    Marker = 0x83,      // M
    Variable = 0x84     // V
}

public enum PpiServiceId : byte
{
    Undefined = 0,
    Read = 0x04,
    Write = 0x05
}

public enum PpiRosCtr : byte
{
    Undefined = 0,
    Request = 0x01,
    Response = 0x03,
    ResponseEx = 0x07,
}

/// <summary>
/// 读取时的DataUnitItem为9个字节
///     数据长度    1B, 01:Bit;02:Byte;04:Word; 06:DoubleWord
///     占位符     1B, 固定为0x00
///     数据个数,   1B
///     占位符     1B, 固定为0x00
///     存储器类型  1B:  01：V存储器 00：其它
///     存储区域    1B:  04/S、05/M、06/AI、 07/AQ、 81:I、 82:Q、 83:M、84:V、 1F:T
///     偏移量     3B： 起始字节地址*8, 高字节在前
///     校验码     1B
///     终止符     1B
/// 写入时的DataUnitItem为11个字节
///     数据长度    1B, 01:Bit;02:Byte;04:Word; 06:DoubleWord
///     占位符     1B, 固定为0x00
///     数据个数,   1B
///     占位符     1B, 固定为0x00
///     存储器类型  1B:  01：V存储器 00：其它
///     存储区域    1B:  04/S、05/M、06/AI、 07/AQ、 81:I、 82:Q、 83:M、84:V、 1F:T
///     偏移量     3B： 起始字节地址*8, 高字节在前
///     数据形式,   2B: 如果写入的是bit为03，其它则为04
///     数据位数,   2B: 01=1Bit; 08:1Byte; 10H=1Word; 20H=1DoubleWord
///     数据值,    1~4B: 根据数据位数不同，长度不同
///     校验码     1B
///     终止符     1B
/// </summary>
public class PpiVarElement
{
    public PpiVarType Type { get; set; }

    public PpiArea Area { get; set; }

    /// <summary>
    /// 在 PPI 报文的 UD（Data Unit）区里，地址偏移量 一律按“位”计算，与要读/写的数据类型（VB/VW/VD）无关。
    /// 偏移量（bit）＝ 起始字节地址 × 8
    /// </summary>
    public int Offset { get; set; }
    /// <summary>
    /// 变量数量(常见的以BYTE为单位）
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 如果是读反馈、或写命令，则这里是数据值，数量同Count描述一致
    /// </summary>
    public List<int> Values { get; set; }

    /// <summary>
    /// 将读取到或待写入的数据元素视作WORD类型，重建其数组，数量 Count/2
    /// </summary>
    public List<short> WordValues { get; private set; }

    /// <summary>
    /// 将读取到或待写入的数据元素视作DWORD类型，重建其数组，数量 Count/4
    /// </summary>
    public List<int> DWordValues { get; private set; }

    /// <summary>
    /// 将读取到或待写入的数据元素视作REAL类型，重建其数组，数量 Count/4
    /// </summary>
    public List<float> RealValues { get; private set; }

    /// <summary>
    /// 临时变量名，调试用。实际使用时应根据 Area/Offset/Type 计算
    /// </summary>
    public string VarName => GetVarName();

    public PpiVarElement()
    {
        Type = PpiVarType.Word;
        Offset = 0;
        Area = PpiArea.Variable;
        Values = new List<int>();
        WordValues = new List<short>();
        DWordValues = new List<int>();
        RealValues = new List<float>();
    }

    /// <summary>
    /// 根据区域类型、偏移量、数据类型，获取变量宽度（单位：Bit）
    /// </summary>
    public int GetVarWidth()
    {
        return this.Type switch {
            PpiVarType.Bit => 1,
            PpiVarType.Byte => 8,
            PpiVarType.Word => 16,
            PpiVarType.DWord => 32,
            _ => 8,
        };
    }

    /// <summary>
    /// 根据区域类型、偏移量、数据类型，生成变量名
    /// </summary>
    public string GetVarName(int index = 0)
    {
        var dbName = "DB1";
        int addrIdx = (Offset + index * GetVarWidth()) / 8;
        int bitOffset = (Offset + index) % 8;

        switch (Area) {
        case PpiArea.SystemMarker:
            return $"SM{addrIdx}";
        case PpiArea.AnalogInput:
            return $"AI{addrIdx}";
        case PpiArea.AnalogOutput:
            return $"AQ{addrIdx}";
        case PpiArea.Counter:
            return $"C{addrIdx}";
        case PpiArea.Timer:
            return $"T{addrIdx}";
        case PpiArea.HSCounter:
            return $"HC{addrIdx}";
        case PpiArea.Input:
            return $"I{addrIdx}.{bitOffset}";
        case PpiArea.Output:
            return $"Q{addrIdx}.{bitOffset}";
        case PpiArea.Marker:
            return $"M{addrIdx}.{bitOffset}";
        case PpiArea.Variable:
            return this.Type switch {
                PpiVarType.Bit => $"{dbName}.DBX{addrIdx}.{bitOffset}",
                PpiVarType.Byte => $"{dbName}.DBB{addrIdx}",
                PpiVarType.Word => $"{dbName}.DBW{addrIdx}",
                PpiVarType.DWord => $"{dbName}.DBD{addrIdx}",
                _ => $"VB{addrIdx}",
            };
        default:
            return $"U{addrIdx}";
        }
    }

    /// <summary>
    /// 根据参数构建对应的变量列表
    /// </summary>
    public bool TryGetValue<T>(VarInfoList varInfoList)
    {
        varInfoList = new VarInfoList();

        for (int i = 0; i < Count; i++) {
            var v = new VarInfoModel();
            v.PathName = GetVarName(i);
            v.Title = v.PathName;
            v.AddedAt = DateTime.Now;
            v.AddedAt = DateTime.Now;

            varInfoList.Add(v);
        }

        return varInfoList is not null && varInfoList.Count > 0;
    }

    public override string ToString()
    {
        var valueText = string.Empty;
        if (Values is not null && Values.Count > 0)
            valueText = string.Join(",", Values);

        return $"Type={Type},Area={Area},Offset={Offset},Count={Count},Values={valueText}";
    }
}

public partial class PpiSD2PDU
{
    /// <summary>
    /// 从PLC或HMI收到的原始数据，从FC之后的字节开始，直到FCS之前
    /// </summary>
    public byte[] _rawData { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// 解析后的元素列表
    /// </summary>
    public virtual List<PpiVarElement> Elements { get; protected set; } = new List<PpiVarElement>();

    /// <summary>
    /// PDU_REF, 读写命令的引用号，请求与反馈成对出现
    /// </summary>
    public int PduRef { get; set; } = 0;

    /// <summary>
    /// 远程控制。01读写请求; 02无字段相应; 03读写响应; 07异常响应
    /// </summary>
    public PpiRosCtr RosCTR { get; set; } = PpiRosCtr.Undefined;

    public PpiServiceId ServiceId { get; set; } = PpiServiceId.Undefined;

    public PpiSD2PDU()
        : this(Array.Empty<byte>())
    {
    }

    public PpiSD2PDU(byte[] rawData)
    {
        _rawData = rawData;
    }

    /// <summary>
    /// 在完整 PPI 响应帧里定位 DU 区起始索引
    /// 响应格式：68 L L 68 DA SA FC [DSAP SSAP]... DU FCS 16
    /// DU 从索引 25 开始（固定 33 字节读响应）
    /// </summary>
    public int FindDuStart(byte[] frame)
    {
        if (frame.Length < 33)
            throw new ArgumentException("帧长度不足");
        if (frame[0] != 0x68 || frame[3] != 0x68)
            throw new ArgumentException("帧头错误");
        return 25;
    }

    public bool TryGetVarInfoList(out bool writeMode, out VarInfoList list)
    {
        writeMode = false;
        list = new VarInfoList();

        return list is not null && list.Count > 0;
    }

    public override string ToString()
    {
        return $"RosCTR={RosCTR},ServiceId={ServiceId},PduRef={PduRef}";
    }
}
