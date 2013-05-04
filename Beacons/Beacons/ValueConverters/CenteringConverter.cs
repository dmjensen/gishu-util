// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Globalization;
using System.Windows.Data;

namespace ManiaX.Beacons.ValueConverters
{
    public class CenteringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return 0;
            var containerDimension = (double)values[0];
            var textDimension = (double)values[1];
            return (containerDimension - textDimension) / 2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}