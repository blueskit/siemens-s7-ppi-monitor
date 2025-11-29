using FCP.Common.Extensions;

namespace S7PpiMonitor.Models;

public class VarInfoModel
{
    /// <summary>
    /// 路径名称/地址
    /// </summary>
    public string PathName { get; set; }
    /// <summary>
    /// 标题/备注
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// 读值或写值(按照Bit或Byte)
    /// </summary>
    public int? Value { get; set; }

    /// <summary>
    /// 读值或写值(对目标采用WORD数据类型判断的值）
    /// </summary>
    public int? WordValue { get; set; }

    /// <summary>
    /// 读值或写值(对目标采用DWORD数据类型判断的值）
    /// </summary>
    public int? DWordValue { get; set; }

    /// <summary>
    /// 读值或写值(对目标采用Real数据类型判断的值）
    /// </summary>
    public double? RealValue { get; set; }

    public DateTime AddedAt { get; set; }
    public DateTime ReadedAt { get; set; }
    public DateTime ChangedAt { get; set; }

    /// <summary>
    /// 最近10次不同值的变动情况（时间、值）,先进先出
    /// 以WORD为比较对象
    /// </summary>
    public Queue<object> Histrory { get; set; }

    public VarInfoModel()
    {
        this.PathName = string.Empty;
        this.Title = string.Empty;
        this.Value = null;
        this.WordValue = null;
        this.DWordValue = null;
        this.RealValue = null;

        this.AddedAt = DateTime.Now;
        this.ReadedAt = DateTime.Now;
        this.ChangedAt = DateTime.Today;

        this.Histrory = new Queue<object>();
    }

    /// <summary>
    /// 刚刚新增的变量
    /// </summary>
    public bool IsAdding() => this.AddedAt.ElapsedSeconds() < 30;

    /// <summary>
    /// 根据 ChangedAt 1分钟内，返回“改变中”状态
    /// </summary>
    public bool IsChanging() => this.ChangedAt.ElapsedMinutes() < 1;

    /// <summary>
    /// 根据 ChangedAt 1~3分钟内，返回“已改变”状态
    /// </summary>
    public bool IsChanged() => this.ChangedAt.ElapsedMinutes() < 3;

}
