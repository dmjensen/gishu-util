// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace ManiaX.Beacons.ValueConverters
{
    public class CodebaseStateToBrushConverter: MarkupExtension, IValueConverter
    {
        private static CodebaseStateToBrushConverter _theOneConverter;
        private static Dictionary<CodebaseState, string> _colorMap;

        static CodebaseStateToBrushConverter()
        {
            _theOneConverter = new CodebaseStateToBrushConverter();
            _colorMap = new Dictionary<CodebaseState, string>
                            {
                                {CodebaseState.Compiling,       "LemonChiffon"},
                                {CodebaseState.NoCompileErrors, "PowderBlue"},
                                {CodebaseState.CompileErrors,   "Tomato"},
                                {CodebaseState.Red,             "Red"},
                                {CodebaseState.Green,           "Lime"}
                            };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value.GetType() != typeof(CodebaseState)) || (targetType != typeof(Brush)))
                return null;

            return _colorMap.ContainsKey((CodebaseState) value) ? _colorMap[(CodebaseState) value] : "LightGray";
            
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