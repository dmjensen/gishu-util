// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System.Collections.ObjectModel;
using ManiaX.Beacons.DataStructs;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.RhythmBeaconTests
{
    [TestFixture]
    public class TestTDDRhythmBeaconDefaults : TestTDDRhythmBeaconBase
    {
        [Test]
        public void ExposesTestResultsAsAnObservableCollection()
        {
            Assert.That(_beacon.TestResultsStack, Is.TypeOf(typeof(ObservableCollection<TestRun>)));
        }

        [Test]
        public void RhythmIsBlankByDefault()
        {
            Assert.That(_beacon.TestResultsStack.Count, Is.EqualTo(0));
        }

        [Test]
        public void HasRefactoringHatOffByDefault()
        {
            Assert.That(!_beacon.IsRefactoringHatOn, "By default, adding function hat must be on,not the refactoring hat");
        }

        [Test]
        public void SupportsChangingHatsAtWill()
        {
            _beacon.ToggleRefactoringHat.Execute(null);
            Assert.That(_beacon.IsRefactoringHatOn, "should have my refactoring hat on now");

            _beacon.ToggleRefactoringHat.Execute(null);
            Assert.That(!_beacon.IsRefactoringHatOn, "should have my refactoring hat off now");
        }
    }
}