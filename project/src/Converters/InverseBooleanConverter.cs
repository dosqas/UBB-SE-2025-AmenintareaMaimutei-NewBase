using System;
using Microsoft.UI.Xaml.Data;

namespace CourseApp.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolean)
            {
                return !boolean;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolean)
            {
                return !boolean;
            }
            return value;
        }
    }
}
