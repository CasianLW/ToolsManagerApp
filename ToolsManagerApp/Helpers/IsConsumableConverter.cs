using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using ToolsManagerApp.Models; 

namespace ToolsManagerApp.Converters
{
    public class IsConsumableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if the passed object is an instance of Consumable
            return value is Consumable;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Converting back is not supported.");
        }
    }
}
