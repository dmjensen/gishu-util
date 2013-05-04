// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using ManiaX.Test.Beacons.Infrastructure;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateTrackerTests
{
    [TestFixture]
    public class TestCodebaseStateTracker : TestCodebaseStateTrackerBase
    {
        [Test]
        public void DefaultsToUnknownOnCreation()
        {
            Assert.AreEqual(CodebaseState.Unknown, _codebaseStateTracker.CurrentState);
        }
        [Test]
        public void StartsTimerOnCreation()
        {
            var timer = new Mock<TransitionTimer>();
            timer.Setup(t => t.Start());

            new CodebaseStateTracker(_mockIDE.Object, _mockStateRepository.Object, timer.Object);

            timer.VerifyAll();
        }

        [Test]
        public void NotifiesListenersIfStateChanges()
        {
            var observer = new NotificationListener();
            _codebaseStateTracker.StateChanged += observer.Handler;

            _mockIDE.Raise(m => m.BuildInitiated += null, EventArgs.Empty);
            long someDuration = 1000;
            ExpectLogEntry(CodebaseState.Compiling, someDuration);

            _mockIDE.Raise(m => m.BuildFailed += null, EventArgs.Empty);

            Assert.That(observer.NotificationReceived, Is.True, "Codebase state tracker should have raised the State Changed notification");
        }

        [Test]
        public void RelaysElapsedTimeUpdates()
        {
            var elapsedTimeInCurrentState = TimeSpan.MinValue;
            _codebaseStateTracker.ElapsedTimeInCurrentState +=
                (sender, args) => elapsedTimeInCurrentState = args.Duration;

            _mockTimer.Raise(timer => timer.ElapsedTimeInCurrentState += null, new DurationEventArgs(TestConstants.SOME_DURATION));
            
            Assert.That(elapsedTimeInCurrentState, Is.EqualTo(TestConstants.SOME_DURATION), "Did not receive the elapsed 12.5secs in current state notification");
        }
        
        [Test]
        public void UnsubscribesForNotifications_WhenNewTestRunnerIsSet()
        {
            SimulateA.SuccessfulBuild(_mockIDE);
            bool stateTrackerStillSubscribedToOldRunner = false;
            _codebaseStateTracker.StateChanged += delegate { stateTrackerStillSubscribedToOldRunner = true; };

            _codebaseStateTracker.ClearTestRunner(_mockTestRunner.Object);
            _codebaseStateTracker.SetTestRunner(new Mock<TestRunner>().Object);
            SimulateA.TestFailure(_mockTestRunner);
            
            Assert.That(stateTrackerStillSubscribedToOldRunner, Is.False, "old runner should be unsubscribed - so that we don't recieve any notifications and it is disposed");
        }
        
        [Test]
        public void DoesNotNotifyObserversOfBuildStateChanged_WhenPaused()
        {
            var anObserver = new NotificationListener();
            
            _mockIDE.Raise(m => m.BuildInitiated += null, EventArgs.Empty);
            _codebaseStateTracker.StateChanged += anObserver.Handler; 
            _mockTimer.Setup(timer => timer.Pause());

            _codebaseStateTracker.Pause();
            _mockIDE.Raise(m => m.BuildFailed += null, EventArgs.Empty);

            Assert.That(anObserver.NotificationReceived, Is.False, "should NOT raise state changed events in paused state");
        }

        [Test]
        public void DoesNotNotifyObserversOfTestStateChanged_WhenPaused()
        {
            var anObserver = new NotificationListener();
            GivenThat.TheCodebaseHasNoBuildErrors(_mockIDE);
            _codebaseStateTracker.StateChanged += anObserver.Handler;
            _mockTimer.Setup(timer => timer.Pause());

            _codebaseStateTracker.Pause();
            SimulateA.TestFailure(_mockIDE, _mockTestRunner);

            Assert.That(!anObserver.NotificationReceived, "should NOT raise state changed events in paused state");
        }

        [Test]
        public void ResumesStateChangedNotification_WhenResumed()
        {
            var anObserver = new NotificationListener();
            GivenThat.TheCodebaseHasNoBuildErrors(_mockIDE);
            _codebaseStateTracker.Pause();
            _mockTimer.Setup(timer => timer.Resume());

            _codebaseStateTracker.StateChanged += anObserver.Handler;

            _codebaseStateTracker.Resume();
            SimulateA.TestFailure(_mockIDE, _mockTestRunner);

            Assert.That(anObserver.NotificationReceived, "should now be posting notifications since we have unpaused");
        }

        [Test]
        public void StopsListeningToIdeBuildEventsOnceDisposed()
        {
            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Unknown));
            
            _codebaseStateTracker.Dispose();
            SimulateA.SuccessfulBuild(_mockIDE);
            SimulateA.BuildFailure(_mockIDE);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Unknown), "should not still be listening to Ide events");
        }
        [Test]
        public void StopsListeningToIdeSolutionEventsOnceDisposed()
        {
            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Unknown));

            SimulateA.SuccessfulBuild(_mockIDE);
            _codebaseStateTracker.Dispose();
            _mockIDE.Raise(ide => ide.SolutionClosed += null, EventArgs.Empty);

            Assert.That(_codebaseStateTracker.CurrentState, Is.Not.EqualTo(CodebaseState.Unknown), "should not still be listening to Ide events");
        }
    }
}