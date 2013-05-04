// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Windows.Data;
using ManiaX.Beacons.ValueConverters;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.ValueConverters
{
    [TestFixture]
    public class TestRphGaugeMaxScaleToMajorTicksConverter
    {
        private IValueConverter _converter;

        [SetUp]
        public void CreateAConverter()
        {
            _converter = new RphGaugeMaxScaleToMajorTicksConverter();
        }

        [Test]
        public void DividesMaxValueByTen()
        {
            Assert.That(_converter.Convert(120, typeof(int), null, null), Is.EqualTo(12),
                "converter should divide input value by 10");
        }

        [Test]
        public void ReturnsNullForUnsupportedCases()
        {
            Assert.That(_converter.Convert("string", typeof(int), null, null), Is.Null,
                "should return null for non-int input");
            Assert.That(_converter.Convert(120, typeof(string), null, null), Is.Null,
                "should return null for non-int target type");
        }

        [ExpectedException(ExpectedException = typeof(NotImplementedException))]
        [Test]
        public void ConvertBackIsNotImplemented()
        {
            _converter.ConvertBack(12, typeof (int), null, null);
            Assert.Fail("is not implemented. should have thrown an exception");
        }
    }
}
