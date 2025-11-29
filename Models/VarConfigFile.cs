namespace S7PpiMonitor.Models;

public class VarConfigFile
{
    public List<VarConfigItem> Items { get; set; }

    public VarConfigFile()
    {
        Items = new List<VarConfigItem>();
    }

    /// <summary>
    /// 从 VarInfoList 建立
    /// </summary>
    public void CreateWith(VarInfoList source)
    {
        Items = new List<VarConfigItem>();
        foreach (var item in source) {
            this.Items.Add(new VarConfigItem() {
                PathName = item.PathName,
                PathName2 = string.Empty,
                Title = item.Title,
                Value = item.Value ?? 0
            });
        }
    }

    /// <summary>
    /// 从 VarInfoList 更新，如果有重复以 source 中的为准
    /// </summary>
    public void UpdateWith(VarInfoList source)
    {
        foreach (var item in source) {

            var match = this.Items.FirstOrDefault(x => x.PathName == item.PathName);
            if (match is not null) {
                this.Items.Add(new VarConfigItem() {
                    PathName = item.PathName,
                    PathName2 = string.Empty,
                    Title = item.Title,
                    Value = item.Value ?? 0
                });
            } else {
            }
        }
    }



}
