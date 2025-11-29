using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using S7PpiMonitor.Models;

namespace S7PpiMonitor.Forms
{
    public partial class ucVarInfoListView : ListView
    {
        /// <summary>
        /// 外部可读写的变量列表
        /// </summary>
        [Required]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public VarInfoList VarList { get; set; } = new VarInfoList();

        [AllowNull]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public EventHandler<VarInfoModel> VarInfoSelectionChanged { get; set; }

        public ucVarInfoListView()
        {
            InitializeComponent();

            SetStyle(ControlStyles.DoubleBuffer |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            initializeListView();
            RefreshVarListView();

            this.ItemSelectionChanged += UcVarInfoListView_ItemSelectionChanged;
        }

        public void initializeListView()
        {
            this.LabelEdit = true;
            this.LabelWrap = false;
            this.VirtualMode = false;

            this.View = View.Details;
            this.Columns.Clear();
            this.Columns.Add("标题", 250, HorizontalAlignment.Left);
            this.Columns.Add("变量名", 150, HorizontalAlignment.Left);
            this.Columns.Add("值", 100, HorizontalAlignment.Left);
            this.Columns.Add("WORD", 120, HorizontalAlignment.Left);
            this.Columns.Add("DWORD", 150, HorizontalAlignment.Left);
            this.Columns.Add("REAL", 180, HorizontalAlignment.Left);
            this.Columns.Add("时刻", 120);
        }

        /// <summary>
        /// 刷新列表（新增或减少元素、排序后调用）
        /// </summary>
        public void UpdateVarListView()
        {
            var topItem = this.TopItem;

            this.BeginUpdate();
            foreach (var item in this.Items) {
                var lvi = item as ListViewItem;
                var varInfo = lvi.Tag as VarInfoModel;

                if (topItem is not null &&
                    lvi.Index < topItem.Index)
                    continue;

                lvi.SubItems[1].Text = varInfo.PathName;
                lvi.SubItems[2].Text = varInfo.Value?.ToString() ?? string.Empty;
                lvi.SubItems[3].Text = varInfo.WordValue?.ToString() ?? string.Empty;
                lvi.SubItems[4].Text = varInfo.DWordValue?.ToString() ?? string.Empty;
                lvi.SubItems[5].Text = varInfo.RealValue?.ToString("0.00") ?? string.Empty;
                lvi.SubItems[6].Text = varInfo.ReadedAt.ToString("HH:mm:ss");

                if (varInfo.IsChanging()) {
                    lvi.ForeColor = Color.Red;
                } else if (varInfo.IsAdding()) {
                    lvi.ForeColor = Color.OrangeRed;
                } else if (varInfo.IsChanged()) {
                    lvi.ForeColor = Color.Green;
                } else {
                    lvi.ForeColor = Color.Black;
                }
            }

            this.EndUpdate();
        }

        /// <summary>
        /// 刷新列表（新增或减少元素、排序后调用）
        /// </summary>
        public void RefreshVarListView()
        {
            var topItem = this.TopItem;

            this.BeginUpdate();
            this.Items.Clear();
            foreach (var varInfo in VarList) {
                var lvi = new ListViewItem(varInfo.Title);
                lvi.Tag = varInfo;

                lvi.SubItems.Add(varInfo.PathName);
                lvi.SubItems.Add(varInfo.Value?.ToString() ?? string.Empty);
                lvi.SubItems.Add(varInfo.WordValue?.ToString() ?? string.Empty);
                lvi.SubItems.Add(varInfo.DWordValue?.ToString() ?? string.Empty);
                lvi.SubItems.Add(varInfo.RealValue?.ToString("0.00") ?? string.Empty);
                lvi.SubItems.Add(varInfo.ReadedAt.ToString("HH:mm:ss"));
                this.Items.Add(lvi);
            }

            if (topItem is not null) {
                int index = topItem.Index;
                if (index >= 0 && this.Items.Count > index)
                    this.TopItem = this.Items[index];
            }

            this.EndUpdate();
        }

        /// <summary>
        ///  当前行改变时触发提示（状态行）
        /// </summary>
        private void UcVarInfoListView_ItemSelectionChanged(object? sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (this.SelectedItems is null || this.SelectedItems.Count < 1)
                return;

            var item = this.SelectedItems[0];

            var varInfo = item.Tag as VarInfoModel;
            if (varInfo is not null && VarInfoSelectionChanged is not null)
                VarInfoSelectionChanged.Invoke(sender, varInfo);
        }
    }
}
