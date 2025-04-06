using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace CourseApp.Converters
{
    public partial class TextTrimmerConverter : IValueConverter
    {
        private const int DefaultTrimLength = 23;
        private const string Ellipsis = "...";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string inputText)
            {
                int trimLength = DefaultTrimLength;
                if (parameter is string paramText && int.TryParse(paramText, out int customLength))
                {
                    trimLength = customLength;
                }

                return inputText.Length > trimLength
                    ? string.Concat(inputText.AsSpan(0, trimLength), Ellipsis)
                    : inputText;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("Reverse conversion is not supported.");
        }
    }
}
