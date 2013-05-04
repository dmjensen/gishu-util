// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ManiaX.Beacons.ValueConverters;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.ValueConverters
{
    [TestFixture]
    public class TestRunsPerHourToColorConverter
    {
        private IValueConverter _converter;

        [SetUp]
        public void CreateConverter()
        {
            _converter = new RunsPerHourToColorConverter();
        }

        [Test]
        public void ConvertReturnsNullIfInputIsNotAnInteger()
        {
            Assert.That(_converter.Convert("abc", typeof(string), null, CultureInfo.InvariantCulture),
                Is.Null, "Input should be Runs per hour - an integer");
        }

        [Test]
        public void ConvertReturnsNullIfTargetTypeIsNotABrush()
        {
            Assert.That(_converter.Convert("23", typeof(string), null, CultureInfo.InvariantCulture),
                Is.Null, "Converter only knows how to convert to SolidColorBrush");
        }

        [Test]
        public void ConvertsRunsPerHourValueToCorrespondingColorRange()
        {
            var inputs = new[] {"2", "5", 
                                    "6", "19",
                                    "20", "55"};
            var expectedColors = new[] {Colors.Red, Colors.Red, 
                                        Colors.Yellow, Colors.Yellow,
                                        Colors.Lime, Colors.Lime};
            for (int i = 0; i < inputs.Length; i++ )
            {
                var brush = (SolidColorBrush)_converter.Convert(inputs[i], typeof(Brush), null, CultureInfo.InvariantCulture);
                Assert.That(brush, Is.Not.Null);
                Assert.That(brush.Color, Is.EqualTo(expectedColors[i]), 
                    String.Format("Expected Color for {0} is incorrect", inputs[i]));
            }
        }

        [ExpectedException(ExpectedException=typeof(NotImplementedException))]
        [Test]
        public void ConvertBackIsNotImplemented()
        {
            _converter.ConvertBack("asd", typeof (string), null, CultureInfo.InvariantCulture);
        }
    }
}
