using System.Windows;
using System.Windows.Controls;

namespace WFUserControl
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserProgressBar : UserControl
    {
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(UserProgressBar), new PropertyMetadata(100.0, MaximumChangedCallback));

        public static void MaximumChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UserProgressBar userProgressBar = d as UserProgressBar;
            if (e.NewValue is double newValue)
            {
                userProgressBar.progressBar.Maximum = newValue;
            }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(UserProgressBar), new PropertyMetadata(0.0, ValueChangedCallback));

        public static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UserProgressBar userProgressBar = d as UserProgressBar;
            if (e.NewValue is double newValue)
            {
                userProgressBar.progressBar.Value = newValue;
                userProgressBar.label.Content = $"{userProgressBar.progressBar.Value / userProgressBar.progressBar.Maximum * 100}%";
            }
        }

        public UserProgressBar()
        {
            InitializeComponent();
        }
    }
}
