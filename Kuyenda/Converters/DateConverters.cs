using System.Globalization;
namespace Kuyenda.Converters
{
    public class DateStringToHumanReadableConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string dateString && DateTime.TryParse(dateString, out DateTime date))
            {
                // Format the date as "Tuesday, Jul 01, 2025"
                return date.ToString("dddd, MMM dd, yyyy", CultureInfo.InvariantCulture);
            }
            return string.Empty;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
