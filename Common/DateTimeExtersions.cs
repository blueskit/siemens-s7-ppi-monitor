namespace S7PpiMonitor.Common;

public static class DateTimeExtersions
{
    private static readonly DateTime epoch = DateTime.UnixEpoch;    //  1970-01-01,Utc);
    private static readonly DateTime _emptyDateTime = new DateTime();

    private static readonly TimeZoneInfo _tzi = TimeZoneInfo.Local;

    public static bool HasValue(this DateTime dt)
    {
        return dt.Year >= 2000;
    }
    public static bool IsEmpty(this DateTime dt)
    {
        return dt.Year <= 2000;
    }
    public static bool IsEmpty(this DateTime? dt)
    {
        return !dt.HasValue || dt.Value.Year <= 2000;
    }

    /// <summary>
    /// 无效日期，用“最小允许日期值”表示，以替换 MinValue
    /// </summary>
    public static DateTime GetZero(this DateTime dt) => DateTime.UnixEpoch;


    public static DateTime Min(this DateTime date, DateTime other)
    {
        return date < other ? date : other;
    }

    public static DateTime Max(this DateTime date, DateTime other)
    {
        return date > other ? date : other;
    }

    /// <summary>
    /// 返回给定日期的年代（如果非法、或空白则返回2000）
    /// </summary>
    public static int Year(this DateTime? date)
    {
        if (date == null || !date.HasValue)
            return 2000;

        return date.Value.Year;
    }

    /// <summary>
    /// 返回带分的日期时刻，例如“yyyy-MM-dd HH:mm”
    /// </summary>
    public static DateTime CreateWith(this DateTime date, int year, int month, int day)
    {
        return new DateTime(year, month, day);
    }

    /// <summary>
    /// 返回带分的日期时刻，例如“yyyy-MM-dd HH:mm”
    /// </summary>
    public static DateTime CreateWith(this DateTime date, int year, int month, int day, int hour, int minute, int second, DateTimeKind kind = DateTimeKind.Unspecified)
    {
        return new DateTime(year, month, day, hour, minute, second, kind);
    }

    /// <summary>
    /// 返回带分的日期时刻，例如“yyyy-MM-dd HH:mm”
    /// </summary>
    public static string ToDateTimeStdString(this DateTime date)
    {
        return date.ToString("yyyy-MM-dd HH:mm");
    }

    /// <summary>
    /// 转换到 DateOnly  格式
    /// </summary>
    public static DateOnly ToDateOnly(this DateTime datetime)
    {
        return DateOnly.FromDateTime(datetime);
    }

    /// <summary>
    /// 转换到 TimeOnly  格式
    /// </summary>
    public static TimeOnly ToTimeOnly(this DateTime datetime)
    {
        return TimeOnly.FromDateTime(datetime);
    }

    /// <summary>
    /// 将本地日期时间修改为UTC时间，但仍然保持原有的 年月日、时分秒
    /// </summary>
    public static DateTime ConvertToUniversalTime(this DateTime datetime)
    {
        return new DateTime(datetime.Year, datetime.Month, datetime.Day,
                datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond,
                DateTimeKind.Utc);
    }

    /// <summary>
    /// 将指定时区的日期时间修改为UTC时间
    /// </summary>
    public static DateTime ConvertToUniversalTime(this DateTime datetime, TimeZoneInfo tzi)
    {
        return new DateTime(datetime.Add(tzi.BaseUtcOffset).Ticks, DateTimeKind.Utc);
    }

    /// <summary>
    /// 将UTC时间修改为本地日期时间，但仍然保持原有的 年月日、时分秒
    /// </summary>
    public static DateTime ConvertToLocalTime(this DateTime utcDateTime)
    {
        return new DateTime(utcDateTime.Year, utcDateTime.Month, utcDateTime.Day,
                utcDateTime.Hour, utcDateTime.Minute, utcDateTime.Second, utcDateTime.Millisecond,
                DateTimeKind.Local);
    }

    /// <summary>
    /// 从unix时间戳（单位：毫秒）转换到日期时间类型
    /// </summary>
    public static DateTime FromUnixTimeMilliseconds(this long unixMilliseconds)
    {
        return epoch.AddMilliseconds(unixMilliseconds).ToLocalTime();
    }

    /// <summary>
    /// 转换到unix时间戳（单位：毫秒）
    /// </summary>
    public static long ConvertToUnixTimeMilliseconds(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
            return new DateTimeOffset(dateTime, TimeSpan.Zero).ToUnixTimeMilliseconds();
        else
            return new DateTimeOffset(dateTime, _tzi.BaseUtcOffset).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 转换到unix时间戳（单位：秒）
    /// </summary>
    public static long ConvertToUnixTimeSeconds(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
            return new DateTimeOffset(dateTime, TimeSpan.Zero).ToUnixTimeSeconds();
        else
            return new DateTimeOffset(dateTime, _tzi.BaseUtcOffset).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 将给定 datetime，返回合适的 右开区间值，用于条件比较时的偏移
    /// </summary>
    /// <param name="datetime">如果输入是日期，无时刻，则加1天返回。否则返回原值</param>
    public static DateTime AdjustRightOpenOffset(this DateTime datetime)
    {
        if (datetime.Hour != 0 || datetime.Minute != 0 || datetime.Second != 0 || datetime.Millisecond != 0)
            return datetime;
        else
            return datetime.AddDays(1);
    }

    /// <summary>
    /// 计算从给定 datetime 到 DateTime.Now 之间消逝的小时数
    /// </summary>
    /// <param name="datetime">起始时刻，如果输入无效，则返回0或错误值</param>
    public static double ElapsedHours(this DateTime datetime)
    {
        return DateTime.Now.Subtract(datetime).TotalHours;
    }

    /// <summary>
    /// 计算从给定 datetime 到 DateTime.Now 之间消逝的分钟数
    /// </summary>
    /// <param name="datetime">起始时刻，如果输入无效，则返回0或错误值</param>
    public static double ElapsedMinutes(this DateTime datetime)
    {
        return DateTime.Now.Subtract(datetime).TotalMinutes;
    }

    /// <summary>
    /// 计算从给定 datetime 到 DateTime.Now 之间消逝的秒数数
    /// </summary>
    /// <param name="datetime">起始时刻，如果输入无效，则返回0或错误值</param>
    public static double ElapsedSeconds(this DateTime datetime)
    {
        return DateTime.Now.Subtract(datetime).TotalSeconds;
    }

}
