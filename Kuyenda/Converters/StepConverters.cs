using System.Globalization;

namespace Kuyenda
{
    public class StepsToDistanceConverter : IValueConverter
    {
        // Average step length in meters (0.75m)
        private const double StepLengthInMeters = 0.75;

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int steps)
            {
                // Convert steps to kilometers
                return (steps * StepLengthInMeters) / 1000.0;
            }
            return 0.0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StepsToCaloriesConverter : IValueConverter
    {
        // Average calories burned per step
        private const double CaloriesPerStep = 0.04;

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int steps)
            {
                return steps * CaloriesPerStep;
            }
            return 0.0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
