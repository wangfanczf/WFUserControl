using System;
using System.Windows;

namespace Demo
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public TreeData ViewModel;

        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = ViewModel;
        }

        private void treeView_ItemClick(object sender, RoutedEventArgs e)
        {
            //TreeViewItem treeViewItem = sender as TreeViewItem;
            //PropertyNodeItem propertyNodeItem = treeViewItem.DataContext as PropertyNodeItem;
            //MessageBox.Show(propertyNodeItem.DisplayName);
        }

        private void treeView_ItemDoubleClick(object sender, RoutedEventArgs e)
        {
            //TreeViewItem treeViewItem = sender as TreeViewItem;
            //PropertyNodeItem propertyNodeItem = treeViewItem.DataContext as PropertyNodeItem;
            //MessageBox.Show(propertyNodeItem.DisplayName);
        }

        private void treeView_ItemExpanded(object sender, RoutedEventArgs e)
        {

        }

        private void treeView_ItemCollapsed(object sender, RoutedEventArgs e)
        {

        }

        private void treeView_ContextMenuOpening(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            PropertyNodeItem propertyNodeItem = treeView.SelectedItem as PropertyNodeItem;

            // 没有节点被选中或选中的节点没有子级，则不弹出右键菜单
            if (propertyNodeItem == null || !propertyNodeItem.HasChildren)
            {
                // 将路由事件标记为已处理，右键菜单就不会弹出
                e.Handled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            listBox.ItemsSource = treeView.CheckedItems;
        }

        Random random = new Random(1);
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            PropertyNodeItem propertyNodeItem = treeView.SelectedItem as PropertyNodeItem;

            int position = random.Next(1, 10);

            PropertyNodeItem newItem = new PropertyNodeItem() { ID = DateTime.Now.ToString(), ParentID = propertyNodeItem.ID, Position = position, Text = $"新增子节点{position}", ExpandedIcon = @"\Images\do.png", CollapsedIcon = @"\Images\do.png" };
            ViewModel.Nodes.Add(newItem);
        }
    }
}
