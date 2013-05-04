// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ManiaX.Beacons.ValueConverters
{
    public class RunsPerHourToColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int runsPerHour;
            if (!Int32.TryParse(value.ToString(), out runsPerHour) || (targetType != typeof(Brush)))
                return null;

            if (runsPerHour < 6)
                return new SolidColorBrush(Colors.Red);
            
            if(runsPerHour < 20)
                return new SolidColorBrush(Colors.Yellow);
            
            return new SolidColorBrush(Colors.Lime);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}