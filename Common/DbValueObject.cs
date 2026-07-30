using System.Dynamic;

namespace S7PpiMonitor.Common;

/// <summary>
/// 针对数据库值转换、以及其它数值转换的共同需求的扩展
/// 目前支持 Mysql、MSSQL、Postgres、S7...
/// </summary>
public static class DbValueObject
{
    public static bool ToDbBool(this object o)
    {
        if (o is DBNull || o == null) {
            return (false);
        } else {
            if (o.GetType() == typeof(bool))
                return (bool)o;
            else if (o.GetType() == typeof(int))
                return (bool)((int)o != 0);
            else if (o.GetType() == typeof(long))
                return (bool)((long)o != 0);
            else if (o.GetType() == typeof(byte))
                return (bool)((byte)o != 0);
            else if (o.GetType() == typeof(string)) {
                string s = (string)o;

                if (float.TryParse(s, out var f) && f > 0)
                    return true;
                else {
                    var keys = new List<string>() {
                        "TRUE","YES","On","1","1.0"
                    };
                    return keys.Exists(x => x.Equals(s, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 读取数据库中的byte值
    /// </summary>
    public static byte ToDbByte(this object o)
    {
        return (unchecked((byte)ConvertToInt32(o)));
    }

    public static short ToDbShort(this object o)
    {
        return (unchecked((short)ConvertToInt16(o)));
    }

    /// <summary>
    /// 将给定参数转换为对应的 Int 类型值返回
    /// 允许将数据库中的 Long/Int/Byte 统一转换为 Int 类型返回
    /// </summary>
    public static int ToDbInt(this object o)
    {
        return (unchecked((int)ConvertToInt32(o)));
    }

    /// <summary>
    /// 将给定参数转换为对应的 uint 类型值返回
    /// </summary>
    public static ushort ToDbUInt16(this object o)
    {
        return ConvertToUInt16(o);
    }

    /// <summary>
    /// 将给定参数转换为对应的 uint 类型值返回
    /// </summary>
    public static uint ToDbUInt32(this object o)
    {
        return (unchecked((uint)ToDbInt64(o)));
    }

    /// <summary>
    /// 将给定参数转换为对应的 Int32 类型值返回
    /// </summary>
    public static Int32 ToDbInt32(this object o)
    {
        return (unchecked((Int32)ConvertToInt32(o)));
    }

    /// <summary>
    /// 将给定参数转换为对应的 Int32 类型值返回
    /// </summary>
    public static Int64 ToDbInt64(this object o)
    {
        return (unchecked((Int64)ConvertToInt64(o)));
    }

    public static float ToDbFloat(this object o)
    {
        return unchecked((float)ConvertToDouble(o));
    }

    public static float ToDbFloat(this object o, int digits)
    {
        var d = (int)(unchecked(ConvertToDouble(o)) * Math.Pow(10, digits));
        return (float)(d * 1f) / 100;
    }

    public static double ToDbDouble(this object o)
    {
        return ConvertToDouble(o);
    }

    public static double ToDbDouble(this object o, int digits)
    {
        return Math.Round(ConvertToDouble(o), digits);
    }

    public static double ToDbDouble(this ExpandoObject o, int digits)
    {
        return Math.Round(ConvertToDouble(o), digits);
    }

    public static DateOnly ToDbDateOnly(this object o)
    {
        if (o is null || o is DBNull)
            return DateOnly.MinValue;
        else if (o.GetType() == typeof(DateOnly))
            return (DateOnly)o;
        else if (o.GetType() == typeof(DateTime)) {
            var dt = (DateTime)o;
            return new DateOnly(dt.Year, dt.Month, dt.Day);
        } else {
            DateOnly.TryParse(o.ToString(), out var dt);        // 其它不支持的类型。有可能出错
            return dt;
        }
    }

    public static DateTime ToDbDateTime(this object o)
    {
        if (o is null || o is DBNull)
            return DateTime.MinValue;
        else if (o.GetType() == typeof(DateTime))
            return (DateTime)o;
        else if (o.GetType() == typeof(DateOnly)) {
            var d = (DateOnly)o;
            return new DateTime(d.Year, d.Month, d.Day);
        } else {
            DateTime.TryParse(o.ToString(), out var dt);        // 其它不支持的类型。有可能出错
            return dt;
        }
    }

    public static TimeOnly ToDbTimeOnly(this object o)
    {
        if (o is null || o is DBNull)
            return TimeOnly.MinValue;
        else if (o.GetType() == typeof(TimeOnly))
            return (TimeOnly)o;
        else if (o.GetType() == typeof(TimeSpan)) {
            var ts = (TimeSpan)o;
            return new TimeOnly(ts.Hours, ts.Minutes, ts.Seconds);
        } else {
            TimeOnly.TryParse(o.ToString(), out var tm);        // 其它不支持的类型。有可能出错
            return tm;
        }
    }

    public static TimeSpan ToDbTimeSpan(this object o)
    {
        if (o is null || o is DBNull)
            return TimeSpan.MinValue;
        else if (o.GetType() == typeof(TimeSpan))
            return (TimeSpan)o;
        else if (o.GetType() == typeof(TimeOnly)) {
            var ts = (TimeOnly)o;
            return new TimeSpan(ts.Hour, ts.Minute, ts.Second);
        } else {
            TimeSpan.TryParse(o.ToString(), out var dt);        // 其它不支持的类型。有可能出错
            return dt;
        }
    }

    public static Guid ToDbGuid(this object o)
    {
        if (o is null || o is DBNull)
            return Guid.Empty;
        else if (o.GetType() == typeof(Guid))
            return (Guid)o;
        else {
            if (Guid.TryParse(o.ToString(), out var guid))
                return guid;
            else
                return Guid.Empty;
        }
    }

    public static string ToDbString(this object o)
    {
        if (o is null || o is DBNull)
            return String.Empty;
        else if (o.GetType() == typeof(string))
            return (string)o;
        else {
            return o.ToString();
        }
    }

    /// <summary>
    /// 将给定参数转换为对应的 short 类型值返回(主要用于PLC、Modbus)
    /// </summary>
    /// <param name="o">支持 int/short/long/byte/uint/ushort/ 等数据类型</param>
    private static short ConvertToInt16(object o)
    {
        if (o is null || o is DBNull)
            return (0);
        else if (o.GetType() == typeof(short))
            return unchecked((short)o);
        else if (o.GetType() == typeof(ushort))
            return unchecked((short)(ushort)o);
        else if (o.GetType() == typeof(int))
            return unchecked((short)(int)o);
        else if (o.GetType() == typeof(uint))
            return unchecked((short)(uint)o);
        else if (o.GetType() == typeof(long))
            return unchecked((short)(long)o);
        else if (o.GetType() == typeof(byte))
            return (short)(byte)o;
        else if (short.TryParse(o.ToString(), out var vi))
            return vi;
        else {
            double.TryParse(o.ToString(), out var vd);
            return (short)vd;
        }
    }

    /// <summary>
    /// 将给定参数转换为对应的 ushort 类型值返回
    /// </summary>
    /// <param name="o">支持 int/short/long/byte/uint/ushort/ 等数据类型</param>
    private static ushort ConvertToUInt16(object o)
    {
        if (o is null || o is DBNull)
            return (0);
        else if (o.GetType() == typeof(short))
            return unchecked((ushort)(short)o);
        else if (o.GetType() == typeof(ushort))
            return unchecked((ushort)o);
        else if (o.GetType() == typeof(int))
            return unchecked((ushort)(int)o);
        else if (o.GetType() == typeof(long))
            return unchecked((ushort)(long)o);
        else if (o.GetType() == typeof(byte))
            return (ushort)(byte)o;
        else if (ushort.TryParse(o.ToString(), out var vi))
            return vi;
        else {
            double.TryParse(o.ToString(), out var vd);
            return (ushort)vd;
        }
    }

    /// <summary>
    /// 允许将数据库中的 Long/Int/Byte 统一转换为 Int 类型返回
    /// 注意1：Sqlite内部仅支持long一种类型
    /// 注意2：如果是字符串类型，则自动转换为int
    /// </summary>
    private static int ConvertToInt32(object o)
    {
        if (o is null || o is DBNull)
            return (0);
        else if (o.GetType() == typeof(int))
            return (int)o;
        else if (o.GetType() == typeof(long))
            return unchecked((int)(long)o);
        else if (o.GetType() == typeof(short))
            return (int)(short)o;
        else if (o.GetType() == typeof(ushort))
            return unchecked((int)(ushort)o);
        else if (o.GetType() == typeof(byte))
            return (int)(byte)o;
        else if (o.GetType() == typeof(bool))
            return (int)((bool)o ? 1 : 0);
        else if (o.GetType() == typeof(Boolean))
            return (int)((bool)o ? 1 : 0);
        else if (int.TryParse(o.ToString(), out var vi))
            return vi;
        else if (double.TryParse(o.ToString(), out var vd))
            return (int)vd;
        else
            return 0;
    }

    /// <summary>
    /// 将给定参数转换为对应的 Int64 类型值返回
    /// </summary>
    /// <param name="o">支持 int/short/long/byte/uint/ushort/ 等数据类型</param>
    private static Int64 ConvertToInt64(object o)
    {
        if (o is null || o is DBNull)
            return (0);
        else if (o.GetType() == typeof(int))
            return unchecked((Int64)(int)o);
        else if (o.GetType() == typeof(uint))
            return unchecked((Int64)(uint)o);
        else if (o.GetType() == typeof(short))
            return (Int64)(short)o;
        else if (o.GetType() == typeof(ushort))
            return (Int64)(ushort)o;
        else if (o.GetType() == typeof(long))
            return unchecked((Int64)(long)o);
        else if (o.GetType() == typeof(ulong))
            return unchecked((Int64)(ulong)o);
        else if (o.GetType() == typeof(byte))
            return (Int64)(byte)o;
        else if (Int64.TryParse(o.ToString(), out var vi))
            return vi;
        else if (double.TryParse(o.ToString(), out var vd))
            return (Int64)vd;
        else
            return 0;
    }

    /// <summary>
    /// 转换为 Double 类型
    /// </summary>
    private static double ConvertToDouble(object o)
    {
        if (o is null || o is DBNull)
            return 0d;
        else if (o.GetType() == typeof(double))
            return (double)o;
        else if (o.GetType() == typeof(float))
            return (double)(float)o;
        else if (o.GetType() == typeof(int))
            return (double)(int)o;
        else if (o.GetType() == typeof(short))
            return (double)(short)o;
        else if (o.GetType() == typeof(long))
            return (double)(long)o;
        else if (o.GetType() == typeof(byte))
            return (double)(byte)o;
        else if (double.TryParse(o.ToString(), out var vd)) // 其它不支持的类型。有可能出错
            return vd;
        else
            return 0d;
    }
}
