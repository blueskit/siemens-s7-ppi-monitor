namespace S7PpiMonitor.Models;

public class VarInfoList
    : List<VarInfoModel>
{

    /// <summary>
    /// 按变量名查找元素
    /// </summary>
    public bool TryGetByPathName(string pathName, out VarInfoModel? varInfo)
    {
        varInfo = this.FirstOrDefault(v => v.PathName.Equals(pathName, StringComparison.OrdinalIgnoreCase));
        return varInfo != null;
    }

    /// <summary>
    /// 按变量名排序
    /// </summary>
    public void SortByVarName()
    {
        this.Sort((x, y) =>
            string.Compare(x.PathName, y.PathName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 按加入时间排序
    /// </summary>
    public void SortByAddedTimestamp()
    {
        this.Sort((x, y) => x.AddedAt.CompareTo(y.AddedAt));
    }
}
