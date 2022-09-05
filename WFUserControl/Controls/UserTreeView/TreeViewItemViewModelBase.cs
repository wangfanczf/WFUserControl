using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace WFUserControl
{
    public class TreeViewItemViewModelBase : INotifyPropertyChanged
    {
        #region 普通属性

        public string ID { get; set; }

        public string ParentID { get; set; }

        public int Position { get; set; }

        #endregion

        #region 界面绑定属性

        private bool showIcon;
        /// <summary>
        /// 是否显示图标
        /// </summary>
        public bool ShowIcon
        {
            get
            {
                return showIcon;
            }
            set
            {
                showIcon = value;
                NotifyPropertyChange(nameof(ShowIcon));
            }
        }

        /// <summary>
        /// 树节点的图标
        /// </summary>
        public string Icon
        {
            get
            {
                if (IsExpanded)
                {
                    return ExpandedIcon;
                }
                else
                {
                    return CollapsedIcon;
                }
            }
        }

        public string expandedIcon;
        /// <summary>
        /// 树节点展开时的图标
        /// </summary>
        public string ExpandedIcon
        {
            get
            {
                return expandedIcon;
            }
            set
            {
                expandedIcon = value;
                NotifyPropertyChange(nameof(Icon));
            }
        }

        private string collapsedIcon;
        /// <summary>
        /// 树节点折叠时的图标
        /// </summary>
        public string CollapsedIcon
        {
            get
            {
                return collapsedIcon;
            }
            set
            {
                collapsedIcon = value;
                NotifyPropertyChange(nameof(Icon));
            }
        }

        private bool isChecked;

        /// <summary>
        /// 树节点是否被勾选
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                NotifyPropertyChange(nameof(IsChecked));
            }
        }

        private bool isExpanded;

        /// <summary>
        /// 树节点是否已展开
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                isExpanded = value;
                NotifyPropertyChange(nameof(IsExpanded));
                NotifyPropertyChange(nameof(Icon));
            }
        }

        private bool isSelected;

        /// <summary>
        /// 树节点是否被选中
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                NotifyPropertyChange(nameof(IsSelected));
            }
        }

        private bool hasChildren;

        /// <summary>
        /// 树节点是否存在子级节点
        /// </summary>
        public bool HasChildren
        {
            get
            {
                return hasChildren || Children.Count > 0;
            }
            set
            {
                hasChildren = value;
                NotifyPropertyChange(nameof(HasChildren));
            }
        }

        private ObservableCollection<TreeViewItemViewModelBase> children;

        /// <summary>
        /// 子节点
        /// </summary>
        public ObservableCollection<TreeViewItemViewModelBase> Children
        {
            internal get
            {
                if (children == null)
                {
                    children = new ObservableCollection<TreeViewItemViewModelBase>();
                    children.CollectionChanged += Children_CollectionChanged;
                }
                return children;
            }
            set
            {
                children = value;
                NotifyPropertyChange(nameof(Children));
            }
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // 触发HasChildren变更
            NotifyPropertyChange(nameof(HasChildren));
        }

        #endregion

        #region 可被子类重写的界面绑定属性

        protected string text;

        /// <summary>
        /// 树节点的显示名称
        /// </summary>
        public virtual string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                NotifyPropertyChange(nameof(Text));
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}