using System;
using System.Globalization;
using System.Windows.Data;

namespace ManiaX.Beacons.ValueConverters
{
    public class HeightToScaleYConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double height;
            if (!Double.TryParse(value.ToString(), out height))
                return null;

            const int TwiceTheBorderThickness = 4;
            return (height - TwiceTheBorderThickness) / 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}