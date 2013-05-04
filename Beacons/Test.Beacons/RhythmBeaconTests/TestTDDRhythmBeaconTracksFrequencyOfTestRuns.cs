// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System.Collections.Generic;
using ManiaX.Beacons.Roles;
using ManiaX.Test.Beacons.Infrastructure;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.RhythmBeaconTests
{
    [TestFixture]
    public class TestTDDRhythmBeaconTracksFrequencyOfTestRuns : TestTDDRhythmBeaconBase
    {
        private Mock<TestRunner> _mockTestRunner;
        private static readonly int SECS_IN_ONE_HOUR = 3600;

        [SetUp]
        public void GivenThatTestRunnerIsConfigured()
        {
            _mockTestRunner = GivenThat.TestRunnerIsConfigured(_testRunnerProvider, _mockTestRunnerFactory);
        }

        [Test]
        public void UpdatesFrequencyOnSuccessfulTestRun()
        {
            _mockStopwatch.Setup(sw => sw.GetElapsedMillisecAndRestart()).Returns(200 * 1000);
            SimulateA.SuccessfulTestRun(_mockTestRunner);

            Assert.That(_beacon.RunsPerHour, Is.EqualTo(SECS_IN_ONE_HOUR / 200), "should have computed the test run frequency as 3 ");
            _mockStopwatch.VerifyAll();
        }

        [Test]
        public void UpdatesFrequencyOnFailedTestRun()
        {
            _mockStopwatch.Setup(sw => sw.GetElapsedMillisecAndRestart()).Returns(200 * 1000);
            SimulateA.TestFailure(_mockTestRunner);

            Assert.That(_beacon.RunsPerHour, Is.EqualTo(SECS_IN_ONE_HOUR / 200), "should have computed the test run frequency as 3 ");
            _mockStopwatch.VerifyAll();
        }

        [Test]
        public void ComputesAverageTimeBetweenRuns()
        {
            const int NUMBER_OF_RUNS = 2;
            
            _mockStopwatch.Setup(sw => sw.GetElapsedMillisecAndRestart()).Returns(200 * 1000);
            SimulateA.TestFailure(_mockTestRunner);
            _mockStopwatch.Setup(sw => sw.GetElapsedMillisecAndRestart()).Returns(400 * 1000);

            SimulateA.SuccessfulTestRun(_mockTestRunner);

            Assert.That(_beacon.RunsPerHour, Is.EqualTo(SECS_IN_ONE_HOUR / (200+400) * NUMBER_OF_RUNS ), "should have computed the test run frequency as 12 runs/hr  <= 2 runs in 10 secs ");
        }

        [Test]
        public void NotifiesChangeInFrequencyOfTestRuns()
        {
            var propertyChangeNotifications = new List<string>();
            _beacon.PropertyChanged += (sender,args) => propertyChangeNotifications.Add(args.PropertyName);

            SimulateA.TestFailure(_mockTestRunner);

            Assert.That(propertyChangeNotifications, Contains.Item("RunsPerHour"), "Should have been notified in change in frequency value");
        }

        [Test]
        public void PauseFrequencyTracking()
        {
            _beacon.Pause();

            _mockStopwatch.Verify(sw => sw.Pause());
        }

        [Test]
        public void ResumeFrequencyTracking()
        {
            _beacon.Resume();
            _mockStopwatch.Verify( sw => sw.Resume());
        }

        [Test]
        public void DoesNotRecordTestResultsWhenPaused()
        {
            _beacon.Pause();
            SimulateA.TestFailure(_mockTestRunner);
            SimulateA.SuccessfulTestRun(_mockTestRunner);

            Assert.That(_beacon.TestResultsStack, Is.Empty, "Should not record test results since beacon is paused, rph would be incorrect");
        }

        [Test]
        public void RecordsTestResultsWhenResumed()
        {
            _beacon.Pause();

            _beacon.Resume();
            SimulateA.TestFailure(_mockTestRunner);
            SimulateA.SuccessfulTestRun(_mockTestRunner);

            Assert.That(_beacon.TestResultsStack.Count, Is.EqualTo(2), "Should now have 2 results since we have unpaused");
        }

        [Test]
        public void DefaultsRphGaugeMaxValueTo80()
        {
            Assert.That(_beacon.RphGaugeMaxScale, Is.EqualTo(80), "default max scale should be 80");
        }

        [Test]
        public void UpdatesRphGaugeMaxValueToMaxOfRphAnd80_AfterEveryTestRun()
        {
            _mockStopwatch.Setup(sw => sw.GetElapsedMillisecAndRestart()).Returns(30 * 1000);
            SimulateA.TestFailure(_mockTestRunner);

            Assert.That(_beacon.RphGaugeMaxScale, Is.EqualTo(120));

            _mockStopwatch.Setup(sw => sw.GetElapsedMillisecAndRestart()).Returns(10*60 * 1000);
            SimulateA.TestFailure(_mockTestRunner);
            Assert.That(_beacon.RphGaugeMaxScale, Is.EqualTo(80));
        }

        [Test]
        public void RoundsMaxValueToMultipleOfTenIfRequired()
        {
            _mockStopwatch.Setup(sw => sw.GetElapsedMillisecAndRestart()).Returns(25 * 1000);
            SimulateA.TestFailure(_mockTestRunner);

            Assert.That(_beacon.RphGaugeMaxScale, Is.EqualTo(150), "should round 144 to 150");
            Assert.That(_beacon.RunsPerHour, Is.EqualTo(144));
        }
        [Test]
        public void NotifiesChangeIn_RphGaugeMaxValue()
        {
            var listener = new PropertyChangeListener(_beacon);
            
            _mockStopwatch.Setup(sw => sw.GetElapsedMillisecAndRestart()).Returns(30 * 1000);
            SimulateA.TestFailure(_mockTestRunner);

            Assert.That(listener.HasReceivedChangeNotificationFor("RphGaugeMaxScale"), "should have notified change in gauge scale max value");
        }

        [Test]
        public void DoesNotRaisePropertyChangeNotificationIfRphGaugeMaxValueHasNotChanged()
        {
            _mockStopwatch.Setup(sw => sw.GetElapsedMillisecAndRestart()).Returns(30 * 1000);
            SimulateA.TestFailure(_mockTestRunner);
            var listener = new PropertyChangeListener(_beacon);
            
            _mockStopwatch.Setup(sw => sw.GetElapsedMillisecAndRestart()).Returns(30 * 1000);
            SimulateA.TestFailure(_mockTestRunner);

            Assert.That(!listener.HasReceivedChangeNotificationFor("RphGaugeMaxScale"),
                        "should not raise changed notifications if property has not changed");
        }
    }
}
