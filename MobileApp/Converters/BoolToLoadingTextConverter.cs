using System.Globalization;

namespace MobileApp.Converters
{
    public class BoolToLoadingTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is bool isLoading && isLoading ? "⏳" : "💾";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}