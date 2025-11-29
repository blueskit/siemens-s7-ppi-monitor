namespace S7PpiMonitor.Models;

public class VarConfigItem
{
    /// <summary>
    /// 路径名称/起始地址
    /// </summary>
    public string PathName { get; set; }

    /// <summary>
    /// 可选的“终止地址”
    /// </summary>
    public string PathName2 { get; set; }
    /// <summary>
    /// 标题/备注
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// 最新获取到的值.一般不需要
    /// </summary>
    public object Value { get; set; }

    private S7.Net.Types.DataItem _PathNameAddrInfo;
    public S7.Net.Types.DataItem PathNameAddrInfo
    {
        get {
            if (_PathNameAddrInfo is null)
                _PathNameAddrInfo = S7DataItemExtersions.FromAddress(this.PathName);
            return _PathNameAddrInfo;
        }
    }

    private S7.Net.Types.DataItem _PathName2AddrInfo;
    public S7.Net.Types.DataItem PathName2AddrInfo
    {
        get {
            if (_PathName2AddrInfo is null)
                _PathName2AddrInfo = S7DataItemExtersions.FromAddress(this.PathName2);
            return _PathName2AddrInfo;
        }
    }

    public VarConfigItem()
    {
        this.PathName = string.Empty;
        this.PathName2 = string.Empty;
        this.Title = string.Empty;
        this.Value = null;

        _PathNameAddrInfo = null;
        _PathName2AddrInfo = null;
    }

}
