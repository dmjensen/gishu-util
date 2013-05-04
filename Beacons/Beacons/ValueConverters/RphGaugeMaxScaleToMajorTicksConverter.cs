// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace ManiaX.Beacons.ValueConverters
{
    public class RphGaugeMaxScaleToMajorTicksConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value.GetType() != typeof(int)) || (targetType != typeof(int)))
                return null;

            return (int) value/10;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}