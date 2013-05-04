// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using ManiaX.Beacons.DataStructs;
using ManiaX.Beacons.ViewModels;

namespace ManiaX.Beacons.ValueConverters
{
    public class TestResultToWPFColorConverter : MarkupExtension, IValueConverter
    {
        static readonly Dictionary<TestResult, Color> _conversionMap = new Dictionary<TestResult, Color>
                                                                           {
                                                                               {TestResult.Red, Colors.Red},
                                                                               {TestResult.Green, Colors.Lime},
                                                                               {TestResult.RefactoringWin, Colors.Gold},
                                                                           };

        private static readonly TestResultToWPFColorConverter _theOneConverter = new TestResultToWPFColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is TestResult) && (targetType == typeof(Color)))
                return _conversionMap[(TestResult) value];
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _theOneConverter;
        }
    }
}