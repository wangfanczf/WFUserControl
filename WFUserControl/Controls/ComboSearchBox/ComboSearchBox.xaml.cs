using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WFUserControl
{
    public partial class ComboSearchBox : UserControl
    {
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(ComboSearchBox), new PropertyMetadata("", DisplayMemberPathChangedCallback));

        public static void DisplayMemberPathChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ComboSearchBox comboSearchBox = d as ComboSearchBox;
            comboSearchBox.listBox.DisplayMemberPath = e.NewValue as string;
        }

        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(ComboSearchBox), new PropertyMetadata("", SelectedValuePathChangedCallback));

        public static void SelectedValuePathChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ComboSearchBox comboSearchBox = d as ComboSearchBox;
            comboSearchBox.listBox.SelectedValuePath = e.NewValue as string;
        }

        public IEnumerable ItemSource
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(IEnumerable), typeof(ComboSearchBox), new PropertyMetadata(null, ItemSourceChangedCallback));

        public static void ItemSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ComboSearchBox comboSearchBox = d as ComboSearchBox;
            comboSearchBox.listBox.ItemsSource = e.NewValue as IEnumerable;
        }

        public object SelectedItem { get; private set; }

        public event SelectionChangedEventHandler SelectionChanged;

        public ComboSearchBox()
        {
            InitializeComponent();
        }

        private void TipTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            string propertyName = listBox.DisplayMemberPath;

            List<object> searchResult = new List<object>();
            foreach (object item in listBox.ItemsSource)
            {
                string displayMember = item.GetType().GetProperty(propertyName).GetValue(item).ToString();
                if (displayMember.Contains(textBox.Text))
                {
                    searchResult.Add(item);
                }
            }

            listBox.ItemsSource = searchResult;
            listBox.SelectedIndex = -1;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0)
            {
                return;
            }

            string propertyName = listBox.DisplayMemberPath; 
            SelectedItem = e.AddedItems[0];
            label.Content = SelectedItem.GetType().GetProperty(propertyName).GetValue(SelectedItem).ToString();

            popup.IsOpen = false;

            SelectionChanged(sender, SelectedItem);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = string.Empty;
            listBox.ItemsSource = ItemSource;
            listBox.SelectedIndex = -1;
            popup.IsOpen = true;
        }
    }
}