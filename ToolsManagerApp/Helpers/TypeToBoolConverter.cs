using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using ToolsManagerApp.Models;

namespace ToolsManagerApp.Converters
{
    public class TypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Consumable;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
