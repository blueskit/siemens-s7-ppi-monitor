using System.Runtime.InteropServices;

namespace S7PpiMonitor.Common;

#pragma warning disable 0618

/// <summary>
/// OpcItem/Mod-Bus采样点数据类型
/// 常用的有：
///     整形3、浮点5、日期7、字符串8; 布尔型11(一般用整型3代替)、自动类型12
/// 对于S7/MPI
///     从数据类型本身得到数据长度、有无符号等信息
/// 针对Modbus
///     主要从字节序获取实际类型
/// </summary>
[Flags]
public enum SamplingPointDataTypeConst : int
{
    Undefined = VarEnum.VT_EMPTY,

    Int8Data = VarEnum.VT_I1,
    Int16Data = VarEnum.VT_I2,
    Int32Data = VarEnum.VT_I4,
    Int64Data = VarEnum.VT_I8,
    FloatData = VarEnum.VT_R4,      // 就是 Float1234
    DoubleData = VarEnum.VT_R8,     // 默认浮点（一般采用 ByteEndian 区分长度、顺序）

    VT_NULL = VarEnum.VT_NULL,

    VT_BOOL = VarEnum.VT_BOOL,

    VT_CY = VarEnum.VT_CY,

    VT_I1 = VarEnum.VT_I1,
    VT_I2 = VarEnum.VT_I2,
    VT_UI2 = VarEnum.VT_UI2,
    VT_I4 = VarEnum.VT_I4,
    VT_UI4 = VarEnum.VT_UI4,
    VT_I8 = VarEnum.VT_I8,
    VT_UI8 = VarEnum.VT_UI8,
    VT_R8 = VarEnum.VT_R8,
    VT_ARRAY = VarEnum.VT_ARRAY,

    VT_VARIANT = VarEnum.VT_VARIANT,    // 自动类型

    STRING = VarEnum.VT_BSTR,           // 字符串

    DATE = VarEnum.VT_DATE,             // 日期，ISO8601:2004
    DATETIME = VarEnum.VT_FILETIME,     // 日期+时刻

    // WORD的几种类型
    Word = VarEnum.VT_UI2,              // 低字节在前（常用、小端、Intel字节序），相当于Word12
    WordReverse = VT_UserDefined + 24,  // 高字节在前（不常有）

    // DWORD的几种类型
    DWord = VarEnum.VT_UI4,             // 低字在前，低字节在前，Little-endian,最常用

    // 扩展数据类型（支持Modbus/Opc/...)
    VT_UserDefined = 100,
}

/// <summary>
/// 数据正负符号(0未知、1无正负符号位；-1有正负符号位)
/// </summary>
public enum SamplingPointDataSignedConst
{
    /// <summary>
    /// 0未知/默认。
    /// 对于WORD/DWORD一般等同于 Signed
    /// </summary>
    Default = 0,
    /// <summary>
    /// -1有(正/负)符号位
    /// </summary>
    Signed = -1,
    /// <summary>
    /// 1无(正/负)符号位
    /// </summary>
    Unsigned = 1,
}

/// <summary>
/// 操作类型（无/读/写/订阅)
/// </summary>
[Flags]
public enum SamplingOperationType
{
    /// <summary>
    /// 未定义/无
    /// </summary>
    Nothing = 0,

    /// <summary>
    /// 读
    /// </summary>
    Read = 1,
    /// <summary>
    /// 写
    /// </summary>
    Write = 2,

    /// <summary>
    /// 表示可读、可写
    /// </summary>
    ReadAndWrite = Read | Write,

    /// <summary>
    /// 订阅（默认读，如果同时有Write、则表示可以写）
    /// </summary>
    Subscribe = 4,

    /// <summary>
    /// 订阅+写（表示可读、可写）
    /// </summary>
    SubscribeWithWrite = Subscribe | Write,
}


/// <summary>
/// 数据采样频率类型，与CategoryAttrKind兼容，目前主要区分是否高频。
///    0x02测量/实时/状态；0x40高频;
/// </summary>
public enum SamplingFrequencyType
{
    /// <summary>
    /// 0x02 测量/实时/状态（CategoryAttrKindConst.Transient）
    /// </summary>
    Normal = 0x02,

    /// <summary>
    /// 0x40高频(CategoryAttrKindConst.HighFrequency)
    /// </summary>
    HighFrequency = 0x40,
}

/// <summary>
/// 介质类型(0默认/物理数据单元;1虚拟IO点)
/// </summary>
public enum SamplingMediumType
{
    /// <summary>
    /// 物理数据单元
    /// </summary>
    PhysicsPoint = 0,

    /// <summary>
    /// 虚拟IO点(针对虚拟IO点，不允许设置停止、活动等状态）
    /// 如果虚拟IO点同时属于虚拟通道，则意味着该IO点是一个需要跨IoTHub边界才能计算的IO点
    /// </summary>
    VirtualPoint = 1,

    /// <summary>
    /// 缓存数据单元(用于暂存来自其它控制设备的重要参数，例如温度定值、时长定值等)
    /// </summary>
    CachedPoint = 2,
}

/// <summary>
/// 调谐/整定/滤波模式(0无;100均值滤波;101中值滤波;120高斯滤波;170卡尔曼滤波)
/// </summary>
public enum TuningMode
{
    None = 0,

    /// <summary>
    /// 100均值滤波，参数
    ///     initialSeconds  初始化秒数，范围 0..60; 默认0s,表示不进行自动初始化，
    ///     windowSize      队列窗口大小，范围 1..64, 默认8
    ///     LValue,UValue   下限值,上限值; 仅保留在上下限范围以内的值,其它丢弃
    /// </summary>
    MeanFilter = 100,

    /// <summary>
    /// 101中值滤波
    ///     initialSeconds  初始化秒数，范围 0..60; 默认0s,表示不进行自动初始化，
    ///     windowSize      队列窗口大小，范围 1..64, 默认7(总是取奇数)
    ///     LValue,UValue   下限值,上限值; 仅保留在上下限范围以内的值,其它丢弃
    /// </summary>
    MedianFilter = 101,

    /// <summary>
    /// 102四分位+中值滤波
    ///     initialSeconds  初始化秒数，范围 0..60; 默认0s,表示不进行自动初始化，
    ///     windowSize      队列窗口大小，范围 1..64, 默认16
    ///     Q1              第一四分位点，默认 0.25
    ///     Q2              第二四分位点，默认 0.50
    ///     Q3              第三四分位点，默认 0.75
    /// </summary>
    QuarterbackMedianFilter = 102,

    /// <summary>
    /// 103四分位+均值滤波
    ///     initialSeconds  初始化秒数，范围 0..60; 默认0s,表示不进行自动初始化，
    ///     windowSize      队列窗口大小，范围 1..64, 默认16
    ///     Q1              第一四分位点，默认 0.25
    ///     Q2              第二四分位点，默认 0.50
    ///     Q3              第三四分位点，默认 0.75
    /// </summary>
    QuarterbackMeanFilter = 103,

    /// <summary>
    /// 120高斯滤波
    ///     r               高斯模板的大小推荐奇数,范围 1..10, 默认 3
    ///     sigma           标准差，默认 1
    /// </summary>
    GaussFilter = 120,

    /// <summary>
    /// 170卡尔曼滤波
    ///     dt              采样时间(deltaT)
    ///     Q_angle         噪声协方差;
    ///     Q_bias          噪声协方差
    ///     R_measure       测量噪声协方差
    ///     P               误差协方差矩阵
    /// </summary>
    KalmanFilter = 170,
}

/// <summary>
/// 字节序常量
/// </summary>
public enum ByteEndianConst
{
    /// <summary>
    /// 默认
    /// </summary>
    DEFAULT = 0,

    /// <summary>
    /// 开关量-线圈，对应Modbus的01读命令
    /// </summary>
    Coil = 1,

    /// <summary>
    /// 开关量-输入离散量，对应Modbus的02读命令
    /// </summary>
    DiscreteInput = 2,

    /// <summary>
    /// 低字节在前（常用、小端、Intel字节序），相当于Word12
    /// </summary>
    Word = 2001,
    /// <summary>
    ///  高字节在前（不常有）
    /// </summary>
    WordReverse = 2002,  // 高字节在前（不常有）

    /// <summary>
    /// 低位在前
    /// </summary>
    DWord = 4001,
    /// <summary>
    /// 低字在前，低字节在前，不常用
    /// </summary>
    DWord1234 = DWord,
    /// <summary>
    /// 低字在前、高字节在前,Little-endian,最常用
    /// </summary>
    DWord2143 = 4002,
    /// <summary>
    /// 高字在前、低字节在前(Big-endian)
    /// </summary>
    DWord3412 = 4003,
    /// <summary>
    /// 高字在前、高字节在前（Big-endian+swap不常用）
    /// </summary>
    DWord4321 = 4004,

    /// <summary>
    /// LITTLE_ENDI_SWAP 浮点数存储格式（外部数据源/Modbus中的格式）
    /// </summary>
    Float1234 = 4101,
    /// <summary>
    /// LITTLE_ENDI 浮点数存储格式（外部数据源/Modbus中的格式）
    /// </summary>
    Float2143 = 4102,
    /// <summary>
    /// BIG_ENDI_SWAP 浮点数存储格式（外部数据源/Modbus中的格式）
    /// </summary>
    Float3412 = 4103,
    /// <summary>
    /// BIG_ENDI 浮点数存储格式（外部数据源/Modbus中的格式）
    /// </summary>
    Float4321 = 4104,

    /// <summary>
    /// LITTLE_ENDI_SWAP 双精度浮点数存储格式（外部数据源(如Modbus)中的格式）
    /// </summary>
    Double1234 = 8001,
    /// <summary>
    /// LITTLE_ENDI 双精度浮点数存储格式（外部数据源(如Modbus)中的格式）
    /// </summary>
    Double2143 = 8002,
    /// <summary>
    /// BIG_ENDI_SWAP 双精度浮点数存储格式（外部数据源(如Modbus)中的格式）
    /// </summary>
    Double3412 = 8003,
    /// <summary>
    ///  BIG_ENDI  双精度浮点数存储格式（外部数据源(如Modbus)中的格式）
    /// </summary>
    Double4321 = 8004,

    // 扩展数据类型（支持Modbus/Opc/...)
    VT_UserDefined = 10000,
    BCDValue1B = VT_UserDefined + 0,
    BCDValue2B = VT_UserDefined + 1,
    BCDValue3B = VT_UserDefined + 2,
    BCDValue4B = VT_UserDefined + 3,
    BCDValue5B = VT_UserDefined + 4,
    BCDValue6B = VT_UserDefined + 5,
    BCDValue7B = VT_UserDefined + 6,
    BCDValue8B = VT_UserDefined + 7,
}

/// <summary>
/// 数据类型扩展函数
/// </summary>
public static class SamplingPointDataTypeExtersions
{
    /// <summary>
    /// 返回数据类型的有无符号类别
    /// </summary>
    public static SamplingPointDataSignedConst ToDataSigned(this SamplingPointDataTypeConst dt)
    {
        return dt switch {
            SamplingPointDataTypeConst.VT_UI2 => SamplingPointDataSignedConst.Unsigned,
            SamplingPointDataTypeConst.VT_UI4 => SamplingPointDataSignedConst.Unsigned,
            SamplingPointDataTypeConst.VT_UI8 => SamplingPointDataSignedConst.Unsigned,
            SamplingPointDataTypeConst.VT_I2 => SamplingPointDataSignedConst.Signed,
            SamplingPointDataTypeConst.VT_I4 => SamplingPointDataSignedConst.Signed,
            SamplingPointDataTypeConst.VT_I8 => SamplingPointDataSignedConst.Signed,
            _ => SamplingPointDataSignedConst.Default,
        };
    }

    /// <summary>
    /// 返回数据类型的是否“BOOL类型”
    /// </summary>
    public static bool IsBoolean(this SamplingPointDataTypeConst dt)
    {
        return dt switch {
            SamplingPointDataTypeConst.VT_BOOL => true,
            _ => false,
        };
    }

    /// <summary>
    /// 返回数据类型的是否“无符号整形”
    /// </summary>
    public static bool IsDataUnsigned(this SamplingPointDataTypeConst dt)
    {
        return dt switch {
            SamplingPointDataTypeConst.VT_UI2 => true,
            SamplingPointDataTypeConst.VT_UI4 => true,
            SamplingPointDataTypeConst.VT_UI8 => true,
            _ => false,
        };
    }

    /// <summary>
    /// 返回数据类型的是否“16位整形”
    /// </summary>
    public static bool Is16BitsWordType(this SamplingPointDataTypeConst dt)
    {
        return dt switch {
            SamplingPointDataTypeConst.VT_I2 => true,
            SamplingPointDataTypeConst.VT_UI2 => true,
            _ => false,
        };
    }

    /// <summary>
    /// 返回数据类型的是否“32位整形”
    /// </summary>
    public static bool Is32BitsDWordType(this SamplingPointDataTypeConst dt)
    {
        return dt switch {
            SamplingPointDataTypeConst.VT_I4 => true,
            SamplingPointDataTypeConst.VT_UI4 => true,
            _ => false,
        };
    }

    /// <summary>
    /// 返回数据类型的常用名称
    /// </summary>
    public static string ToCommonlyDataTypeName(this SamplingPointDataTypeConst dt)
    {
        var map = GetCommonlyDataTypeMap();
        if (map.ContainsKey(dt))
            return map[dt];
        return dt.ToString();
    }

    /// <summary>
    /// 返回数据类型的标准长度（对于Modbus，以WORD的长度为1）
    /// </summary>
    public static int ToCommonlyDataLength(this SamplingPointDataTypeConst dt)
    {
        var dataTypeName = dt.ToCommonlyDataTypeName();

        if (string.IsNullOrEmpty(dataTypeName))
            return 1;

        if (dataTypeName.StartsWith("Word", StringComparison.OrdinalIgnoreCase))
            return 1;
        else if (dataTypeName.StartsWith("DWord", StringComparison.OrdinalIgnoreCase))
            return 2;
        else if (dataTypeName.StartsWith("Float", StringComparison.OrdinalIgnoreCase))
            return 2;
        else if (dataTypeName.StartsWith("Double", StringComparison.OrdinalIgnoreCase))
            return 4;
        //else if (dataTypeName.StartsWith("BCD", StringComparison.OrdinalIgnoreCase)) {
        //    return (dt - SamplingPointDataTypeConst.BCDValue1B) + 1;
        //}

        return 1;
    }

    /// <summary>
    /// 返回最常用的数据类型及其别名
    /// </summary>
    public static Dictionary<SamplingPointDataTypeConst, string> GetCommonlyDataTypeMap()
    {
        return new Dictionary<SamplingPointDataTypeConst, string>() {
            { SamplingPointDataTypeConst.Undefined,"未定义" },
            { SamplingPointDataTypeConst.VT_BOOL ,"BOOL" },
            { SamplingPointDataTypeConst.VT_I2 ,"Short" },
            { SamplingPointDataTypeConst.VT_UI2 ,"Word" },
            { SamplingPointDataTypeConst.VT_I4 ,"Int" },
            { SamplingPointDataTypeConst.DWord,"DWord" },
            { SamplingPointDataTypeConst.FloatData,"Float" },
            { SamplingPointDataTypeConst.DoubleData,"Double" },
        };
    }

    /// <summary>
    /// 返回最常用的数据类型及其别名
    /// </summary>
    public static Dictionary<SamplingPointDataTypeConst, string> GetCommonlyDataTypeMap(IList<SamplingPointDataTypeConst> typeList)
    {
        var result = new Dictionary<SamplingPointDataTypeConst, string>();
        foreach (var kv in GetCommonlyDataTypeMap()) {
            if (typeList.Contains(kv.Key))
                result.Add(kv.Key, kv.Value);
        }
        return result;
    }

    /// <summary>
    /// 返回最常用的数据类型(或别名）及其对应的类型编号
    /// </summary>
    public static Dictionary<string, SamplingPointDataTypeConst> GetCommonlyDataTypeNameMap(bool forceKeyToUpper = false)
    {
        var kvs = new Dictionary<string, SamplingPointDataTypeConst>() {
            { "BOOL" ,SamplingPointDataTypeConst.VT_BOOL },
            { "Short" ,SamplingPointDataTypeConst.VT_I2},
            { "WORD" ,SamplingPointDataTypeConst.VT_UI2 },
            { "Int",SamplingPointDataTypeConst.VT_I4  },
            { "DWORD" ,SamplingPointDataTypeConst.VT_UI4 },
            { "Float" ,SamplingPointDataTypeConst.FloatData },		// 别名
            { "Double" ,SamplingPointDataTypeConst.DoubleData},
        };

        if (forceKeyToUpper) {
            var tmp = new Dictionary<string, SamplingPointDataTypeConst>();
            foreach (var kv in kvs) {
                tmp.Add(kv.Key.ToUpper(), kv.Value);
            }
            return tmp;
        } else {
            return kvs;
        }
    }
}

/// <summary>
/// 字节序类型扩展函数
/// </summary>
public static class ByteEndianConstExtersions
{
    /// <summary>
    /// 返回字节序的常用名称
    /// </summary>
    public static string ToCommonlyEndianName(this ByteEndianConst endian)
    {
        var map = GetCommonlyEndianMap();
        if (map.ContainsKey(endian))
            return map[endian];
        return endian.ToString();
    }

    /// <summary>
    /// 返回指定字节序的标准长度（对于Modbus，以WORD的长度为1）
    /// </summary>
    public static int ToCommonlyDataLength(this ByteEndianConst endian)
    {
        var dataTypeName = endian.ToCommonlyEndianName();

        if (string.IsNullOrEmpty(dataTypeName))
            return 1;

        if (dataTypeName.StartsWith("Word", StringComparison.OrdinalIgnoreCase))
            return 1;
        else if (dataTypeName.StartsWith("DWord", StringComparison.OrdinalIgnoreCase))
            return 2;
        else if (dataTypeName.StartsWith("Float", StringComparison.OrdinalIgnoreCase))
            return 2;
        else if (dataTypeName.StartsWith("Double", StringComparison.OrdinalIgnoreCase))
            return 4;
        else if (dataTypeName.StartsWith("BCD", StringComparison.OrdinalIgnoreCase)) {
            return (endian - ByteEndianConst.BCDValue1B) + 1;
        }
        return 1;
    }

    public static bool IsDefault(this ByteEndianConst endian)
    {
        return endian == ByteEndianConst.DEFAULT;
    }

    public static bool IsWord(this ByteEndianConst endian)
    {
        return new List<ByteEndianConst>() {
                    ByteEndianConst.DEFAULT,
                    ByteEndianConst.Word ,
                    ByteEndianConst.WordReverse }
                .Contains(endian);
    }

    public static bool IsDWord(this ByteEndianConst endian)
    {
        return new List<ByteEndianConst>() {
                    ByteEndianConst.DWord1234 ,
                    ByteEndianConst.DWord3412 ,
                    ByteEndianConst.DWord2143 ,
                    ByteEndianConst.DWord4321 }
                .Contains(endian);
    }

    public static bool IsFloat(this ByteEndianConst endian)
    {
        return new List<ByteEndianConst>() {
                    ByteEndianConst.Float1234 ,
                    ByteEndianConst.Float3412 ,
                    ByteEndianConst.Float2143 ,
                    ByteEndianConst.Float4321 }
                .Contains(endian);
    }

    public static bool IsDouble(this ByteEndianConst endian)
    {
        return new List<ByteEndianConst>() {
                    ByteEndianConst.Double1234 ,
                    ByteEndianConst.Double3412 ,
                    ByteEndianConst.Double2143 ,
                    ByteEndianConst.Double4321 }
                .Contains(endian);
    }

    public static int ToByteLength(this ByteEndianConst endian)
    {
        return ToWordLength(endian) * 2;
    }

    public static int ToWordLength(this ByteEndianConst endian)
    {
        switch (endian) {
        case ByteEndianConst.Word:
        case ByteEndianConst.WordReverse:
            return 1;
        case ByteEndianConst.DWord1234:
        case ByteEndianConst.DWord3412:
        case ByteEndianConst.DWord2143:
        case ByteEndianConst.DWord4321:
            return 2;
        case ByteEndianConst.Float1234:
        case ByteEndianConst.Float3412:
        case ByteEndianConst.Float2143:
        case ByteEndianConst.Float4321:
            return 2;
        case ByteEndianConst.Double1234:
        case ByteEndianConst.Double3412:
        case ByteEndianConst.Double2143:
        case ByteEndianConst.Double4321:
            return 4;
        default:
            return 1;
        }
    }

    /// <summary>
    /// 返回最常用的字节序及其别名
    /// </summary>
    public static Dictionary<ByteEndianConst, string> GetCommonlyEndianMap()
    {
        return new Dictionary<ByteEndianConst, string>() {
            { ByteEndianConst.DEFAULT,"默认" },
            { ByteEndianConst.Word,"Word" },
            { ByteEndianConst.WordReverse,"WordReverse" },
            { ByteEndianConst.DWord1234,"DWord1234" },
            { ByteEndianConst.DWord4321 ,"DWord4321" },
            { ByteEndianConst.DWord3412 ,"DWord3412" },
            { ByteEndianConst.DWord2143 ,"DWord2143" },
            { ByteEndianConst.Float1234 ,"Float1234" },
            { ByteEndianConst.Float3412 ,"Float3412" },
            { ByteEndianConst.Float2143 ,"Float2143" },
            { ByteEndianConst.Float4321 ,"Float4321" },
            { ByteEndianConst.Double1234 ,"Double1234" },
            { ByteEndianConst.Double4321 ,"Double4321" },
            { ByteEndianConst.Double3412 ,"Double3412" },
            { ByteEndianConst.Double2143 ,"Double2143" },
            { ByteEndianConst.BCDValue1B ,"BCDValue 1B" },
            { ByteEndianConst.BCDValue2B ,"BCDValue 2B" },
            { ByteEndianConst.BCDValue4B ,"BCDValue 4B" },
            { ByteEndianConst.BCDValue6B ,"BCDValue 6B" },
            { ByteEndianConst.BCDValue8B ,"BCDValue 8B" },
        };
    }

    /// <summary>
    /// 返回最常用的字节序及其别名
    /// </summary>
    public static Dictionary<ByteEndianConst, string> GetCommonlyEndianMap(IList<ByteEndianConst> endianList)
    {
        var result = new Dictionary<ByteEndianConst, string>();
        foreach (var kv in GetCommonlyEndianMap()) {
            if (endianList.Contains(kv.Key))
                result.Add(kv.Key, kv.Value);
        }
        return result;
    }

    /// <summary>
    /// 返回最常用的字节序(或别名）及其对应的类型编号
    /// </summary>
    public static Dictionary<string, ByteEndianConst> GetCommonlyEndianNameMap(bool forceKeyToUpper = false)
    {
        var kvs = new Dictionary<string, ByteEndianConst>() {
            { "Word" ,ByteEndianConst.Word},
            { "WordReverse" ,ByteEndianConst.WordReverse},
            { "DWord1234" ,ByteEndianConst.DWord1234 },
            { "DWord4321" ,ByteEndianConst.DWord4321 },
            { "DWord3412" ,ByteEndianConst.DWord3412 },
            { "DWord2143" ,ByteEndianConst.DWord2143 },
            { "Float" ,ByteEndianConst.Float1234},		// 别名
			{ "Float1234" ,ByteEndianConst.Float1234 },
            { "Float3412",ByteEndianConst.Float3412  },
            { "Float2143" ,ByteEndianConst.Float2143 },
            { "Float4321",ByteEndianConst.Float4321  },
            { "Double" ,ByteEndianConst.Double1234 },
            { "Double1234" ,ByteEndianConst.Double1234 },
            { "Double4321" ,ByteEndianConst.Double4321 },
            { "Double2143" ,ByteEndianConst.Double2143 },
            { "Double3412" ,ByteEndianConst.Double3412 },
            { "BCDValue 1B" ,ByteEndianConst.BCDValue1B },
            { "BCDValue 2B",ByteEndianConst.BCDValue2B  },
            { "BCDValue 4B" ,ByteEndianConst.BCDValue4B },
            { "BCDValue 6B" ,ByteEndianConst.BCDValue6B },
            { "BCDValue 8B",ByteEndianConst.BCDValue8B  },
        };

        if (forceKeyToUpper) {
            var tmp = new Dictionary<string, ByteEndianConst>();
            foreach (var kv in kvs) {
                tmp.Add(kv.Key.ToUpper(), kv.Value);
            }
            return tmp;
        } else {
            return kvs;
        }
    }
}
