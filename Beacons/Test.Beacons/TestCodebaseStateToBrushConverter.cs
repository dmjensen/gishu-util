// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Windows.Markup;
using System.Windows.Media;
using ManiaX.Beacons;
using ManiaX.Beacons.ValueConverters;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.ValueConverters
{
    [TestFixture]
    public class TestCodebaseStateToBrushConverter
    {
        private CodebaseStateToBrushConverter _converter;

        [SetUp]
        public void BeforeEachTest()
        {
            _converter = new CodebaseStateToBrushConverter();
        }

        [TestCase(CodebaseState.Unknown, "LightGray")]
        [TestCase(CodebaseState.Compiling, "LemonChiffon")]
        [TestCase(CodebaseState.NoCompileErrors, "PowderBlue")]
        [TestCase(CodebaseState.CompileErrors, "Tomato")]
        [TestCase(CodebaseState.Red, "Red")]
        [TestCase(CodebaseState.Green, "Lime")]
        public void ConvertsACodebaseStateToCorrespondingColorName(CodebaseState state, string expectedColor)
        {
            Assert.AreEqual(expectedColor,
                _converter.Convert(state, typeof(Brush), null, null));
        }

        [Test]
        public void ReturnsNullIfInputIsNotACodebaseState()
        {
            Assert.IsNull(_converter.Convert("Junk Input", typeof(Brush), null, null), "cannot convert anything except CodebaseState");
        }

        [Test]
        public void ReturnsNullIfRequiredOutputTypeIsNotBrush()
        {
            Assert.IsNull(_converter.Convert(CodebaseState.Red, typeof(Color), null,null), "cannot convert to anything except Brush");
        }

        [ExpectedException(ExpectedException = typeof(NotImplementedException))]
        [Test]
        public void DoesNotConvertBackFromBrushToCodebaseState()
        {
            _converter.ConvertBack(new SolidColorBrush(Colors.Red), typeof (CodebaseState), null, null);
            Assert.Fail("should have throw Not Impl Excep");
        }

        [Test]
        public void IsAMarkupExtension()
        {
            MarkupExtension markupExtension = _converter;
            Assert.That(markupExtension.ProvideValue(null), Is.InstanceOf(typeof(CodebaseStateToBrushConverter)));
        }

    }
}