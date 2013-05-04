// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Collections.Generic;
using System.Windows.Media;
using ManiaX.Beacons.DataStructs;
using ManiaX.Beacons.ValueConverters;
using ManiaX.Beacons.ViewModels;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.ValueConverters
{
    [TestFixture]
    public class TestTestResultToWPFColorConverter
    {
        private TestResultToWPFColorConverter _converter;

        [SetUp]
        public void BeforeEachTest()
        {
            _converter = new TestResultToWPFColorConverter();
        }
        [Test]
        public void ConvertsACodebaseStateToCorrespondingColor()
        {
            var translationMap = new Dictionary<TestResult, Color>
                             {
                                 {TestResult.Red,        Colors.Red},
                                 {TestResult.Green,      Colors.Lime},
                                 {TestResult.RefactoringWin,      Colors.Gold}
                             };

            foreach (var entry in translationMap)
            {
                Assert.AreEqual(entry.Value, _converter.Convert(entry.Key, typeof(Color), null, null),
                    string.Format("The converter returned the wrong color for {0}", entry.Key));
            }
        }

        [Test]
        public void ReturnsNullIfInputIsNotATestResult()
        {
            Assert.IsNull(_converter.Convert("Junk", typeof(Color), null, null), "should return null if input is not a TestResult");
        }

        [Test]
        public void ReturnsNullIfRequiredOutputTypeIsNotColor()
        {
            Assert.IsNull(_converter.Convert(TestResult.Red, typeof(int), null, null), "should return null for any target type other than Color");
        }

        [ExpectedException(ExpectedException = typeof(NotImplementedException))]
        [Test]
        public void DoesNotConvertBackFromColorToTestResult()
        {
            _converter.ConvertBack(Colors.Red, typeof (TestResult), null, null);
            Assert.Fail("Should have thrown NotImplExcep");
        }

        [Test]
        public void IsAMarkupExtension()
        {
            var markupExtension = _converter;
            Assert.That(markupExtension.ProvideValue(null), Is.InstanceOf(typeof(TestResultToWPFColorConverter)));
        }
    }
}
