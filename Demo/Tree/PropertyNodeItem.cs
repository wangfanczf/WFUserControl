using WFUserControl;

namespace Demo
{
    /// <summary>
    /// 节点类
    /// </summary>
    public class PropertyNodeItem : TreeViewItemViewModelBase
    {
        public override string Text
        {
            get
            {
                return text + "(重写)";
            }

            set
            {
                 base.Text = value;
            }
        }

        public PropertyNodeItem()
        {
            CollapsedIcon = "/Demo;component/Tree/Images/TreeView_FolderClosed.png";
            ExpandedIcon = "/Demo;component/Tree/Images/TreeView_FolderOpen.png";
            ShowIcon = true;
        }
    }
}