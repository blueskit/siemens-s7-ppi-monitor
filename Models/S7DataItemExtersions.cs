using FCP.Common.Data;
using FCP.Common.Extensions;
using S7Types = S7.Net.Types;

namespace S7PpiMonitor.Models;

/// <summary>
/// Siemens S7系列单元地址格式
///     I(过程映像输入)       Iy.x, IBx, IWx, IDx
///     Q(过程映像输出)       Qy.x, QBx, QWx, QDx
///     V(变量存储器)         Vy.x, VBx, VWx, VDx
///       *在Smart200下,需转换到如下类型*
///                         DB1.DBy.x, DB1.DBBx, DB1.DBWx, DB1.DBDx
///     M(标志存储器)         My.x, MBx, MWx, MDx
///     T(定时器存储器)       Tx
///     C(计数器存储器)       Cx
///     HC(高速计数器存储器)   HCx
///     AC(累加器)           ACx
///     L(局部存储区)         Lx
///     S(顺序控制继电器存储区) Sy.x, SBx
/// 扩展的S7系列单元地址格式
///     “DBnnn.DBRnnn”，     REAL，例如“DB1.DBR1234”
///     “DBnnn.DBLRnnn”，    LREAL，例如“DB1.DBLR1234”
/// </summary>
public static class S7DataItemExtersions
{
    /// <summary>
    /// 将address转换到正确的DataItem类型并返回
    /// 1、如果出现 “DBnnn.DBRnnn”，表示REAL浮点型数据
    /// 2、如果出现 “DBnnn.DBLRnnn”，表示LREAL浮点型数据
    /// </summary>
    public static S7Types.DataItem FromAddress(string address)
    {
        try {
            if (address.Contains(".DBR")) {
                var item = S7Types.DataItem.FromAddress(address.Replace(".DBR", ".DBD"));
                item.VarType = S7.Net.VarType.Real;
                return item;
            } else if (address.Contains(".DBLR")) {
                var item = S7Types.DataItem.FromAddress(address.Replace(".DBLR", ".DBD"));
                item.VarType = S7.Net.VarType.LReal;
                return item;
            } else {
                var item = S7Types.DataItem.FromAddress(address);
                return item;
            }
        } catch (Exception ex) {
            throw new Exception($"S7 PLC变量地址 {address} 格式错误");
        }
    }

    /// <summary>
    /// 将address、以及IOT平台指定的字节序综合判断转换到正确的DataItem类型并返回
    /// 1、如果byteEndian是浮点数，则返回为 Real、或 LReal 类型
    /// 2、如果byteEndian是整数，主要区分有符号、无符号
    /// </summary>
    public static S7Types.DataItem FromAddress(string address, ByteEndianConst byteEndian)
    {
        try {
            var item = S7DataItemExtersions.FromAddress(address);

            if (byteEndian.IsDefault()) {
                // DO NOTHING
            } else if (byteEndian.IsFloat() && (item.VarType == S7.Net.VarType.DWord
                                             || item.VarType == S7.Net.VarType.DInt)) {
                item.VarType = S7.Net.VarType.Real;
            } else if (byteEndian.IsDouble() && (item.VarType == S7.Net.VarType.DWord
                                             || item.VarType == S7.Net.VarType.DInt)) {
                item.VarType = S7.Net.VarType.LReal;
            }

            return item;
        } catch (Exception ex) {
            throw new Exception($"Address={address},byteEndian={byteEndian};", ex);
        }
    }

    /// <summary>
    /// 将address转换到正确的DataItem类型并返回
    /// </summary>
    public static S7Types.DataItem TryFromAddressAndValue<T>(string address, T value)
    {
        return S7Types.DataItem.FromAddressAndValue<T>(address, value);
    }

    /// <summary>
    /// 将address转换到正确的DataItem类型并返回
    /// </summary>
    public static S7Types.DataItem TryFromAddressAndValue(string address, object value)
    {
        var item = S7Types.DataItem.FromAddressAndValue(address, value);
        CorrectingValueType(item);
        return item;
    }

    /// <summary>
    /// 按照 dataItem 的变量类型，将 Value 转换到正确的类型上
    /// </summary>
    public static void CorrectingValueType(this S7Types.DataItem dataItem)
    {
        CorrectingValueType(dataItem, dataItem.Value);
    }

    /// <summary>
    /// 按照 dataItem 的变量类型，将 Value 转换到正确的类型上
    /// </summary>
    public static void CorrectingValueType(this S7Types.DataItem dataItem, object value)
    {
        switch (dataItem.VarType) {
        case S7.Net.VarType.Bit:
            dataItem.Value = value.ToDbBool() ? true : false;
            break;
        case S7.Net.VarType.Byte:
            dataItem.Value = value.ToDbByte();
            break;
        case S7.Net.VarType.Word:
            dataItem.Value = value.ToDbShort();
            break;
        case S7.Net.VarType.DWord:
            dataItem.Value = value.ToDbUInt32();
            break;
        case S7.Net.VarType.DInt:
            dataItem.Value = value.ToDbInt();
            break;
        case S7.Net.VarType.Real:
            dataItem.Value = value.ToDbFloat();
            break;
        case S7.Net.VarType.LReal:
            dataItem.Value = value.ToDbDouble();
            break;
        default:
            break;
        }
    }
}
