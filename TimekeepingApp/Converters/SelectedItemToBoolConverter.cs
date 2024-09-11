using System;
using System.Globalization;
using System.Windows.Data;

namespace TimekeepingApp.Converters
{
    public class SelectedItemToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Example logic: return true if the selected item is not null
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
