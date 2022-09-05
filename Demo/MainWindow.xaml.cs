using Demo;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WPFUserControl
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new ViewModel();
            DataContext = viewModel;

            viewModel.Cigarettes = new ObservableCollection<CigaretteViewModel>();
            CigaretteViewModel cigarette1 = new CigaretteViewModel() { Id = "1", Name = "黄鹤楼-竹韵" };
            CigaretteViewModel cigarette2 = new CigaretteViewModel() { Id = "2", Name = "黄鹤楼-蓝楼" };
            CigaretteViewModel cigarette3 = new CigaretteViewModel() { Id = "3", Name = "黄鹤楼-红楼" };
            CigaretteViewModel cigarette4 = new CigaretteViewModel() { Id = "4", Name = "红塔山-经典100" };
            viewModel.Cigarettes.Add(cigarette1);
            viewModel.Cigarettes.Add(cigarette2);
            viewModel.Cigarettes.Add(cigarette3);
            viewModel.Cigarettes.Add(cigarette4);
        }

        private TreeData GetTreeView()
        {
            TreeData viewModel = new TreeData();

            PropertyNodeItem node1 = new PropertyNodeItem() { ID = "1", ParentID = "", Position = 1, Text = "武汉1" };
            PropertyNodeItem node11 = new PropertyNodeItem() { ID = "11", ParentID = "1", Position = 1, Text = "江夏1子节点1" };
            PropertyNodeItem node111 = new PropertyNodeItem() { ID = "111", ParentID = "11", Position = 1, Text = "藏龙岛1子节点1的子级1" };
            PropertyNodeItem node112 = new PropertyNodeItem() { ID = "112", ParentID = "11", Position = 2, Text = "梁山头1子节点1的子级2" };
            PropertyNodeItem node113 = new PropertyNodeItem() { ID = "113", ParentID = "11", Position = 3, Text = "测试1子节点1的子级3" };
            PropertyNodeItem node114 = new PropertyNodeItem() { ID = "114", ParentID = "11", Position = 4, Text = "测试1子节点1的子级4" };
            PropertyNodeItem node115 = new PropertyNodeItem() { ID = "115", ParentID = "11", Position = 5, Text = "测试1子节点1的子级5" };
            PropertyNodeItem node116 = new PropertyNodeItem() { ID = "116", ParentID = "11", Position = 6, Text = "测试1子节点1的子级6" };
            PropertyNodeItem node117 = new PropertyNodeItem() { ID = "117", ParentID = "11", Position = 7, Text = "测试1子节点1的子级7" };
            PropertyNodeItem node118 = new PropertyNodeItem() { ID = "118", ParentID = "11", Position = 8, Text = "测试1子节点1的子级8" };
            PropertyNodeItem node119 = new PropertyNodeItem() { ID = "119", ParentID = "11", Position = 9, Text = "测试1子节点1的子级9" };
            PropertyNodeItem node1110 = new PropertyNodeItem() { ID = "1110", ParentID = "11", Position = 10, IsSelected = true, Text = "测试1子节点1的子级10" };
            PropertyNodeItem node13 = new PropertyNodeItem() { ID = "13", ParentID = "1", Position = 3, Text = "测试1子节点3" };
            PropertyNodeItem node12 = new PropertyNodeItem() { ID = "12", ParentID = "1", Position = 2, HasChildren = true, Text = "测试1子节点2" };
            PropertyNodeItem node121 = new PropertyNodeItem() { ID = "121", Position = 1, ParentID = "12", Text = "测试1子节点2的子级1" };
            PropertyNodeItem node2 = new PropertyNodeItem() { ID = "2", ParentID = "", Position = 2, Text = "测试2" };

            viewModel.Nodes.Add(node1);
            viewModel.Nodes.Add(node11);
            viewModel.Nodes.Add(node13);
            viewModel.Nodes.Add(node12);
            viewModel.Nodes.Add(node121);
            viewModel.Nodes.Add(node2);
            viewModel.Nodes.Add(node111);
            viewModel.Nodes.Add(node112);
            viewModel.Nodes.Add(node113);
            viewModel.Nodes.Add(node114);
            viewModel.Nodes.Add(node115);
            viewModel.Nodes.Add(node116);
            viewModel.Nodes.Add(node117);
            viewModel.Nodes.Add(node118);
            viewModel.Nodes.Add(node119);
            viewModel.Nodes.Add(node1110);

            return viewModel;
        }

        private void comboSearchBox_SelectionChanged(object sender, object selection)
        {
            CigaretteViewModel cigaretteViewModel = selection as CigaretteViewModel;
            MessageBox.Show(cigaretteViewModel.Name);
        }

        private void buttonShowTree_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.ViewModel = GetTreeView();
            window1.Show();
        }

        /// <summary>
        /// 模拟耗时的方法
        /// </summary>
        private Task TimeConsumeFunction(IProgress<int> reportProgress)
        {
            Task task = Task.Run(() =>
            {
                for (int i = 0; i <= 100; i++)
                {
                    Thread.Sleep(100);
                    reportProgress.Report(i);
                }
            });
            return task;
        }

        private async void buttonProgressBar_Click(object sender, RoutedEventArgs e)
        {
            Progress<int> progressReporter = new Progress<int>((progress) => { progressBar.Value = progress; });
            await TimeConsumeFunction(progressReporter);
        }
    }
}