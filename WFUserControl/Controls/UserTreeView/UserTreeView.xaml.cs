using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WFUserControl
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserTreeView : UserControl
    {
        #region 字段

        private string searchText; // 键盘快捷搜索的输入文本，用于对比两次输入的文本是否相同
        private int searchIndex; // 键盘快捷搜索的当前项
        private int searchTimestamp; // 键盘快捷搜索的触发时间
        public bool isBringIntoView; // 是否自动调整树节点在可视区域的位置

        #endregion

        #region 依赖项属性

        /// <summary>
        /// 树的数据源
        /// </summary>
        public ObservableCollection<TreeViewItemViewModelBase> ItemsSource
        {
            get { return (ObservableCollection<TreeViewItemViewModelBase>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<TreeViewItemViewModelBase>), typeof(UserTreeView), new PropertyMetadata(null, ItemsSourceChangedCallback));

        public static void ItemsSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UserTreeView userTreeView = d as UserTreeView;
            if (userTreeView.ItemsSource != null)
            {
                userTreeView.ItemsSource.CollectionChanged -= ItemsSource_CollectionChanged;
                userTreeView.ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;
            }

            // 构建树结构视图模型
            ObservableCollection<TreeViewItemViewModelBase> itemTree = new ObservableCollection<TreeViewItemViewModelBase>();
            if (e.NewValue is IEnumerable<TreeViewItemViewModelBase> items)
            {
                // 对节点按Position排序
                IOrderedEnumerable<TreeViewItemViewModelBase> orderedItems = items.OrderBy(i => i.Position);

                // 将节点集合转换成以ID为Key， 以树节点视图模型为Value 的字典
                Dictionary<string, TreeViewItemViewModelBase> itemDictionary = orderedItems.ToDictionary(m => m.ID);

                // 遍历所有节点，找出节点的父级，并将节点存入父级的Children
                foreach (var item in orderedItems)
                {
                    if (itemDictionary.TryGetValue(item.ParentID, out TreeViewItemViewModelBase parentItem))
                    {
                        if (!parentItem.Children.Contains(item))
                        {
                            parentItem.Children.Add(item);
                        }
                    }
                    else
                    {
                        // 顶层节点
                        itemTree.Add(item);
                    }
                }

                userTreeView.treeView.ItemsSource = itemTree;

                // 如果要锚定到选中的节点，需要先收起所有节点，然后从选中的节点向父级递归展开
                if (userTreeView.IsAnchoredToSelectedItem)
                {
                    // 必须先取出selectedItem，因为收起节点时，会把子节点的选中状态转移到父级
                    TreeViewItemViewModelBase selectedItem = items.FirstOrDefault(i => i.IsSelected);
                    items.Where(i => i.HasChildren).ToList().ForEach(item => item.IsExpanded = false);

                    if (selectedItem != null)
                    {
                        RecursionExpandedParent(selectedItem, itemDictionary);

                        // 收起时，子节点的选中状态转移到了父级，需要重新选中子节点
                        userTreeView.isBringIntoView = true;
                        selectedItem.IsSelected = true;
                    }
                }
            }
            else
            {
                userTreeView.treeView.ItemsSource = null;
            }
        }

        /// <summary>
        /// 树的数据源集合数据量变更
        /// </summary>
        private static void ItemsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && sender is ObservableCollection<TreeViewItemViewModelBase> items)
            {
                Dictionary<string, TreeViewItemViewModelBase> itemDictionary = items.ToDictionary(i => i.ID);
                foreach (TreeViewItemViewModelBase newItem in e.NewItems)
                {
                    if (itemDictionary.TryGetValue(newItem.ParentID, out TreeViewItemViewModelBase parentItem))
                    {
                        int insertIndex = 0;
                        TreeViewItemViewModelBase insertPositionItem = parentItem.Children.LastOrDefault(c => c.Position < newItem.Position);
                        if (insertPositionItem != null)
                        {
                            insertIndex = parentItem.Children.IndexOf(insertPositionItem) + 1;
                        }

                        // 新节点插入子集时，引发Children_CollectionChanged
                        parentItem.Children.Insert(insertIndex, newItem);
                    }
                }
            }
        }

        /// <summary>
        /// 复选框是否可见
        /// </summary>
        public bool CheckBoxVisible
        {
            get { return (bool)GetValue(CheckBoxVisibleProperty); }
            set { SetValue(CheckBoxVisibleProperty, value); }
        }

        public static readonly DependencyProperty CheckBoxVisibleProperty =
            DependencyProperty.Register("CheckBoxVisible", typeof(bool), typeof(UserTreeView), new PropertyMetadata(false, CheckBoxVisibleChangedCallback));

        public static void CheckBoxVisibleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UserTreeView userTreeView = d as UserTreeView;
            if (e.NewValue is bool checkBoxVisible && checkBoxVisible)
            {
                // 通常启用CheckBox时，树节点为不可选中状态
                userTreeView.CanItemSelected = false;
            }
        }

        /// <summary>
        /// 勾选/去勾选时，是否将勾选状态传递到子级
        /// </summary>
        public bool IsRecursionCheckSubItems
        {
            get { return (bool)GetValue(IsRecursionCheckSubItemsProperty); }
            set { SetValue(IsRecursionCheckSubItemsProperty, value); }
        }

        public static readonly DependencyProperty IsRecursionCheckSubItemsProperty =
            DependencyProperty.Register("IsRecursionCheckSubItems", typeof(bool), typeof(UserTreeView), new PropertyMetadata(false));

        /// <summary>
        /// 树节点项是否可选中
        /// </summary>
        public bool CanItemSelected
        {
            get { return (bool)GetValue(CanItemSelectedProperty); }
            set { SetValue(CanItemSelectedProperty, value); }
        }

        public static readonly DependencyProperty CanItemSelectedProperty =
            DependencyProperty.Register("CanItemSelected", typeof(bool), typeof(UserTreeView), new PropertyMetadata(true));

        /// <summary>
        /// 双击是否展开/折叠子级
        /// </summary>
        public bool IsDoubleClickExpanded
        {
            get { return (bool)GetValue(IsDoubleClickExpandedProperty); }
            set { SetValue(IsDoubleClickExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsDoubleClickExpandedProperty =
            DependencyProperty.Register("IsDoubleClickExpanded", typeof(bool), typeof(UserTreeView), new PropertyMetadata(true));

        /// <summary>
        /// 是否自动锚定到选中的节点
        /// </summary>
        public bool IsAnchoredToSelectedItem
        {
            get { return (bool)GetValue(IsAnchoredToSelectedItemProperty); }
            set { SetValue(IsAnchoredToSelectedItemProperty, value); }
        }

        public static readonly DependencyProperty IsAnchoredToSelectedItemProperty =
            DependencyProperty.Register("IsAnchoredToSelectedItem", typeof(bool), typeof(UserTreeView), new PropertyMetadata(false, IsAnchoredToSelectedItemChangedCallback));

        public static void IsAnchoredToSelectedItemChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UserTreeView userTreeView = d as UserTreeView;
            if (e.NewValue is bool isAnchoredToSelectedItem && isAnchoredToSelectedItem)
            {
                // 开启自动锚定时，需要关闭虚拟化，否则可视区域外无法锚定
                userTreeView.IsVirtualizing = false;
            }
        }

        /// <summary>
        /// 是否开启虚拟化
        /// </summary>
        public bool IsVirtualizing
        {
            get { return (bool)GetValue(IsVirtualizingProperty); }
            set { SetValue(IsVirtualizingProperty, value); }
        }

        public static readonly DependencyProperty IsVirtualizingProperty =
            DependencyProperty.Register("IsVirtualizing", typeof(bool), typeof(UserTreeView), new PropertyMetadata(true));

        #endregion

        #region 普通属性

        /// <summary>
        /// 获取用于生成 System.Windows.Controls.ItemsControl 的内容的集合。
        /// </summary>
        public ItemCollection Items
        {
            get
            {
                return treeView.Items;
            }
        }

        /// <summary>
        /// 获取与控件关联的ItemContainerGenerator
        /// </summary>
        public ItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                return treeView.ItemContainerGenerator;
            }
        }

        /// <summary>
        /// 获取树中的选定项
        /// </summary>
        public TreeViewItemViewModelBase SelectedItem
        {
            get
            {
                return treeView.SelectedItem as TreeViewItemViewModelBase;
            }
        }

        /// <summary>
        /// 获取树中所有勾选的节点
        /// </summary>
        public List<TreeViewItemViewModelBase> CheckedItems
        {
            get
            {
                if (ItemsSource != null && ItemsSource.Count > 0)
                {
                    return ItemsSource.Where(i => i.IsChecked).ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 将勾选状态递归传递到子级
        /// </summary>
        private void RecursionCheck(bool isChecked, IEnumerable<TreeViewItemViewModelBase> children)
        {
            foreach (TreeViewItemViewModelBase treeViewItemViewModel in children)
            {
                treeViewItemViewModel.IsChecked = isChecked;
                RecursionCheck(isChecked, treeViewItemViewModel.Children);
            }
        }

        /// <summary>
        /// 递归展开父级节点
        /// </summary>
        private static void RecursionExpandedParent(TreeViewItemViewModelBase item, Dictionary<string, TreeViewItemViewModelBase> itemDictionary)
        {
            if (itemDictionary.TryGetValue(item.ParentID, out TreeViewItemViewModelBase parentItem))
            {
                parentItem.IsExpanded = true;
                RecursionExpandedParent(parentItem, itemDictionary);
            }
        }

        /// <summary>
        /// 从item向父级递归查找类型为T的第一个对象
        /// </summary>
        public T FindParent<T>(TreeViewItemViewModelBase item) where T : TreeViewItemViewModelBase
        {
            if (ItemsSource != null && ItemsSource.Count > 0)
            {
                Dictionary<string, TreeViewItemViewModelBase> itemDictionary = ItemsSource.ToDictionary(m => m.ID);
                if (itemDictionary.TryGetValue(item.ParentID, out TreeViewItemViewModelBase parentItem))
                {
                    if (parentItem is T t)
                    {
                        // 如果父项是目标类型，则返回父项
                        return t;
                    }
                    else
                    {
                        // 否则继续向父级递归查找
                        return FindParent<T>(parentItem);
                    }
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        #endregion

        #region 自定义事件

        /// <summary>
        /// 单击事件
        /// </summary>
        public event RoutedEventHandler ItemClick;

        /// <summary>
        /// 单击变更了Checked状态之后发生
        /// </summary>
        public event RoutedEventHandler ItemClick_AfterChecked;

        /// <summary>
        /// 双击事件
        /// </summary>
        public event RoutedEventHandler ItemDoubleClick;

        /// <summary>
        /// 树节点展开
        /// </summary>
        public event RoutedEventHandler ItemExpanded;

        /// <summary>
        /// 树节点折叠
        /// </summary>
        public event RoutedEventHandler ItemCollapsed;

        #endregion

        #region 内部事件

        /// <summary>
        /// 数据模板提供的单击事件
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TreeViewItem treeViewItem = ControlHelper.FindParent<TreeViewItem>(button);
            if (treeViewItem == null)
            {
                return;
            }

            if (treeViewItem.DataContext is TreeViewItemViewModelBase treeViewItemViewModel)
            {
                // 响应自定义事件
                ItemClick?.Invoke(treeViewItem, e);

                // 选中当前点击的树节点
                isBringIntoView = false;
                treeViewItem.IsSelected = CanItemSelected;

                if (CheckBoxVisible)
                {
                    // 如果点击部分不是CheckBox，则设置其勾选状态
                    if (!(e.OriginalSource is CheckBox))
                    {
                        treeViewItemViewModel.IsChecked = !treeViewItemViewModel.IsChecked;
                    }

                    // 将勾选状态递归传递到子级
                    if (IsRecursionCheckSubItems)
                    {
                        RecursionCheck(treeViewItemViewModel.IsChecked, treeViewItemViewModel.Children);
                    }

                    ItemClick_AfterChecked?.Invoke(treeViewItem, e);
                }
            }
        }

        /// <summary>
        /// 右键单击时，选中树节点。主要是为了配合右键菜单使用。
        /// </summary>
        private void Button_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button button = sender as Button;
            TreeViewItem treeViewItem = ControlHelper.FindParent<TreeViewItem>(button);
            if (treeViewItem == null)
            {
                return;
            }

            // 选中当前点击的树节点
            if (CanItemSelected)
            {
                treeViewItem.IsSelected = true;
            }
        }

        /// <summary>
        /// 数据模板提供的双击事件
        /// </summary>
        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Button button = sender as Button;
            TreeViewItem treeViewItem = ControlHelper.FindParent<TreeViewItem>(button);
            if (treeViewItem == null)
            {
                return;
            }

            // 响应自定义事件
            ItemDoubleClick?.Invoke(treeViewItem, e);

            // 展开当前双击的树节点
            if (IsDoubleClickExpanded)
            {
                treeViewItem.IsExpanded = !treeViewItem.IsExpanded;
            }
        }

        /// <summary>
        /// 树节点选中事件
        /// </summary>
        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            // 如果设置了树节点不可选中，则取消选中
            if (e.Source is TreeViewItem treeViewItem)
            {
                treeViewItem.IsSelected &= CanItemSelected;

                if (treeViewItem.IsSelected)
                {
                    treeViewItem.BringIntoView();
                }
            }
        }

        /// <summary>
        /// BringIntoView之前发生
        /// </summary>
        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (!isBringIntoView)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 树节点展开事件
        /// </summary>
        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            // 响应自定义展开事件
            ItemExpanded?.Invoke(sender, e);

            e.Handled = true;
        }

        /// <summary>
        /// 树节点折叠事件
        /// </summary>
        private void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            // 响应自定义折叠事件
            ItemCollapsed?.Invoke(sender, e);

            e.Handled = true;
        }

        /// <summary>
        /// 键盘输入快捷搜索
        /// </summary>
        private void TreeViewItem_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Text))
            {
                if (ItemsSource != null && ItemsSource.Count > 0)
                {
                    string searchCondition;
                    if (e.Timestamp - searchTimestamp < 200)
                    {
                        // 快速连续输入时，多个搜索文本作为一个搜索条件
                        searchCondition = searchText + e.Text;
                    }
                    else
                    {
                        searchCondition = e.Text;
                    }

                    // 记录事件触发时间
                    searchTimestamp = e.Timestamp;

                    IEnumerable<TreeViewItemViewModelBase> searchedItems = ItemsSource.Where(i => i.Text.ToLower().Contains(searchCondition.ToLower()));
                    if (searchedItems == null || searchedItems.Count() == 0)
                    {
                        return;
                    }

                    if (searchText == searchCondition)
                    {
                        // 同一个关键词再次搜索，定位到下一项
                        if (searchIndex < searchedItems.Count() - 1)
                        {
                            searchIndex++;
                        }
                        else
                        {
                            // 返回第一项
                            searchIndex = 0;
                        }
                    }
                    else
                    {
                        // 新关键词，取第一项
                        searchIndex = 0;
                    }

                    TreeViewItemViewModelBase searchedItem = searchedItems.ElementAt(searchIndex);

                    // 选中符合搜索条件的树节点
                    if (searchedItem != null)
                    {
                        // 递归展开父节点
                        Dictionary<string, TreeViewItemViewModelBase> itemDictionary = ItemsSource.ToDictionary(m => m.ID);
                        RecursionExpandedParent(searchedItem, itemDictionary);

                        isBringIntoView = true;
                        searchedItem.IsSelected = true;
                    }

                    searchText = searchCondition;
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// 清除搜索条件
        /// </summary>
        private void buttonClearSearch_Click(object sender, RoutedEventArgs e)
        {
            textBoxSearchText.Text = string.Empty;
        }

        /// <summary>
        /// 搜索按钮
        /// </summary>
        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
        }

        private void textBoxSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxSearchText.Text))
            {
                buttonClearSearch.Visibility = Visibility.Collapsed;
            }
            else
            {
                buttonClearSearch.Visibility = Visibility.Visible;
            }
        }

        private void textBoxSearchText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // 文本框中的回车不响应窗体的回车
                e.Handled = true;
            }
        }

        #endregion

        public UserTreeView()
        {
            InitializeComponent();
        }
    }
}