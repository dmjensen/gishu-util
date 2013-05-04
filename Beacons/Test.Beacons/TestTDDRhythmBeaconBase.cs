// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using ManiaX.Beacons;
using ManiaX.Beacons.DataStructs;
using ManiaX.Beacons.Roles;
using ManiaX.Beacons.ViewModels;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons
{
    public class TestTDDRhythmBeaconBase
    {
        protected TDDRhythmBeacon _beacon;
        protected Mock<TestRunnerFactory> _mockTestRunnerFactory;
        protected TestRunnerProvider _testRunnerProvider;

        protected Mock<Stopwatch> _mockStopwatch;

        [SetUp]
        public void CreateBeacon()
        {
            _mockTestRunnerFactory = new Mock<TestRunnerFactory>();
            _testRunnerProvider = new TestRunnerProvider(_mockTestRunnerFactory.Object);

            _mockStopwatch = new Mock<Stopwatch>();
            _mockStopwatch.Setup(sw => sw.Start());

            _beacon = new TDDRhythmBeacon(_testRunnerProvider, _mockStopwatch.Object, new MockUiUpdater());
        }

        protected void AnnotateTestRun(TestRun testRun, string note)
        {
            _beacon.SelectedTestRun = testRun;  
            _beacon.NoteForSelectedTestRun = note;
        }
    }
}