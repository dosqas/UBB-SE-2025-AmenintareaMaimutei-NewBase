using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;

namespace CourseApp.Converters
{
    public partial class AvailabilityColorConverter : IValueConverter
    {
        private static readonly Windows.UI.Color AvailableColor = Windows.UI.Color.FromArgb(255, 79, 79, 176);
        private static readonly Windows.UI.Color UnavailableColor = Windows.UI.Color.FromArgb(255, 128, 128, 128);
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isAvailable)
            {
                return new SolidColorBrush(isAvailable ? AvailableColor : UnavailableColor);
            }
            return new SolidColorBrush(UnavailableColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("Reverse conversion is not supported");
        }
    }
}