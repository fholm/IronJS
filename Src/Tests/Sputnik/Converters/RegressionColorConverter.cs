using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace IronJS.Tests.Sputnik.Converters
{
    public class RegressionColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool?)value ?? false
                ? new LinearGradientBrush(SystemColors.WindowColor, Color.FromRgb(0xDB, 0xA6, 0xA4), 90.0)
                : new LinearGradientBrush(SystemColors.WindowColor, Color.Multiply(SystemColors.WindowColor, 0.8f), 90.0);
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
