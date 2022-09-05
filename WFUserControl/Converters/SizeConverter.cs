using System;
using System.Globalization;
using System.Windows.Data;

namespace WFUserControl
{
	public class SizeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double originalValue = (double)value;
			double.TryParse(parameter.ToString(), out double offset);
			return originalValue + offset;

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double originalValue = (double)value;
			double.TryParse(parameter.ToString(), out double offset);
			return originalValue - offset;
		}
	}
}
