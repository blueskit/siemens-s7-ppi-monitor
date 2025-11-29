using S7PpiMonitor.Models;

namespace S7PpiMonitor.Utilities;

#pragma warning disable 8603,8618,8625

/// <summary>
/// 项目级配置参数
/// </summary>
public class AppSetting
{
    #region 单一实例
    private static readonly Lazy<AppSetting> lazy = new Lazy<AppSetting>(() => new AppSetting());
    /// <summary>
    /// 单一实例引用
    /// </summary>
    public static AppSetting Instance { get { return lazy.Value; } }
    #endregion

    /// <summary>
    /// 变量配置
    /// </summary>
    public VarConfigFile VarConfig { get; set; }

    private AppSetting()
    {
    }
}
