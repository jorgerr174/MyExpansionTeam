using System.Globalization;

namespace MobileApp.Converters
{
    public class StringEqualsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is string stringValue && parameter is string parameterValue && stringValue.Equals(parameterValue, StringComparison.OrdinalIgnoreCase);
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool isChecked && isChecked && parameter is string parameterValue
                ? parameterValue : null;
    }

    public class IsNotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is not null;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool isUserTeam && isUserTeam ? Colors.LightGreen : Colors.LightBlue;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}