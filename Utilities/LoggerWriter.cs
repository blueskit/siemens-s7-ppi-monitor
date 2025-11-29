using Microsoft.Extensions.Logging;
using NLog;

namespace S7PpiMonitor.Utilities;

#pragma warning disable 8603,8618,8625

public class LoggerWriter : IDisposable
{
    #region 单例模式
    private static LoggerWriter _Instance;

    public static LoggerWriter Instance
    {
        get {
            if (_Instance == null) {
                _Instance = new LoggerWriter();
            }
            return _Instance;
        }
    }
    #endregion

    private static Microsoft.Extensions.Logging.ILogger _logger = null;

    /// <summary>
    /// 外部只读
    /// </summary>
    public static Microsoft.Extensions.Logging.ILogger Logger
    {
        get {
            if (_logger == null || _Instance == null) {
                _Instance = new LoggerWriter();
            }
            return _logger;
        }
    }

    private LoggerWriter()
    {
        var factory = new NLog.Extensions.Logging.NLogLoggerFactory();
        _logger = factory.CreateLogger("Default");
    }

    /// <summary>
    /// 指定日志级别的日志
    /// </summary>
    public void Write(Microsoft.Extensions.Logging.LogLevel level, string format, params object[] args)
    {
        _logger.Log(level, string.Format(format, args));
    }


    public void Flush()
    {
        LogManager.Flush();
    }

    public void Write(string format, params object[] args)
    {
        _logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, string.Format(format, args));
    }

    public void WriteLine(string format, params object[] args)
    {
        _logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, string.Format(format, args));
    }

    public void WriteLine(string text)
    {
        _logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, text);
    }

    public void WriteFull(string logLevel, string category, string subject, string detail, params object[] args)
    {
        _logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, category + "|" + subject + "|" + string.Format(detail, args));
    }

    #region IDisposable Members

    public void Dispose()
    {
        if (_logger != null) {
            _logger = null;
            LogManager.Shutdown();
        }
        GC.SuppressFinalize(this);
    }

    #endregion
}
