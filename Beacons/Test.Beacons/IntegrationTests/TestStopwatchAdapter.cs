// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Threading;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.IntegrationTests
{
    [Category("Integration Tests Slow")]
    [TestFixture]
    public class TestStopwatchAdapter
    {
        private TransitionTimer _timer;

        [SetUp]
        public void BeforeEachTest()
        {
            _timer = new DotNetStopwatchWithUpdates();
        }

        [TearDown]
        public void AfterEachTest()
        {
            _timer.Dispose();
        }
        [Test]
        public void CanMeasureIntervalsRepeatedly()
        {
            _timer.Start();
            Thread.Sleep(3000);
            Assert.That(_timer.GetElapsedMillisecAndRestart(), Is.InRange(2995, 3100));
            
            Thread.Sleep(1000);
            Assert.That(_timer.GetElapsedMillisecAndRestart(), Is.InRange(995, 1100));
        }

        [Test]
        public void ReturnsStartOfIntervalOnRequest()
        {
            _timer.Start();
            Thread.Sleep(1500);
            var expectedTimestamp = DateTime.Now.AddMilliseconds(-1500);
            Assert.That(DateTime.Now.Subtract(_timer.StartedAt()).TotalMilliseconds,
                Is.InRange(1500,1600));
        }

        [Test]
        public void ReturnsStartOfIntervalAsCannedValueIfTimerNotStarted()
        {
            DateTime nullDate = DateTime.Parse("0001-01-01 00:00:00.000");
            
            Assert.That(_timer.StartedAt(), Is.EqualTo(nullDate));
        }

        [Test]
        public void NotifiesResetOfElapsedTime_WhenStarted()
        {
            _timer.ElapsedTimeNotificationRateInSeconds = 60;
            var trace =
                new AsyncNotificationTrace("Should have received an update to reset elapsed time on a state change");
            _timer.ElapsedTimeInCurrentState += trace.Handler;

            _timer.Start();

            trace.VerifyAfter(TimeSpan.FromSeconds(2), trace.Received(1).Notification);
        }

        [Test]
        public void NotifiesResetOfElapsedTime_WhenRestarted()
        {
            _timer.ElapsedTimeNotificationRateInSeconds = 60;
            _timer.Start();

            var trace =
                new AsyncNotificationTrace("Should have received an update to reset elapsed time on a state change");
            _timer.ElapsedTimeInCurrentState += trace.Handler;

            _timer.GetElapsedMillisecAndRestart();

            trace.VerifyAfter(TimeSpan.FromSeconds(2), trace.Received(1).Notification);
        }
        [Test]
        public void PeriodicallyNotifiesElapsedTimeInCurrentStateOnceStarted()
        {
            using(_timer)
            {
                _timer.ElapsedTimeNotificationRateInSeconds = 1;
                _timer.Start();

                var trace = new AsyncNotificationTrace("Should have received 6 periodic updates with 1 sec delay");
                _timer.ElapsedTimeInCurrentState += trace.Handler;
                trace.VerifyAfter(TimeSpan.FromMilliseconds(6300), trace.Received(6).Notifications);

            }
        }

        [Test]
        public void DefaultsTo1MinUpdateRateForElapsedTimeUpdates()
        {
            Assert.That(_timer.ElapsedTimeNotificationRateInSeconds, Is.EqualTo(60));
        }

        [Test]
        public void SupportsPauseAndResumeForMeasuringIntervals()
        {
            _timer.Start();

            _timer.Pause();
            Thread.Sleep(4000);
            _timer.Resume();
            Thread.Sleep(1000);

            Assert.That(_timer.GetElapsedMillisecAndRestart(), Is.InRange(990, 1100));
        }

        [Test]
        public void DoesNotPostElapsedTimeUpdatesInPausedState()
        {
            using(_timer)
            {
                _timer.ElapsedTimeNotificationRateInSeconds = 2;
                _timer.Start();
                var trace = new AsyncNotificationTrace("Should have received 3 periodic updates with 1 sec delay");
                _timer.ElapsedTimeInCurrentState += trace.Handler;

                _timer.Pause();
                Thread.Sleep(3000);
                _timer.Resume();
                
                trace.VerifyAfter(TimeSpan.FromMilliseconds(2200), trace.Received(1).Notifications);
            }
            
        }
    }
}
