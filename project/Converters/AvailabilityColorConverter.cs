
using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;

namespace CourseApp.Converters
{
    public class AvailabilityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isAvailable)
            {
                return isAvailable ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 79, 79, 176)) :
                                    new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 128, 128));
            }
            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 128, 128));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}