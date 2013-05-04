// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Collections.Generic;
using System.Linq;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using ManiaX.Beacons.ViewModels;
using ManiaX.Test.Beacons.Infrastructure;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateBeaconTests
{
    public class TestStateBeaconBase
    {
        protected TestRunnerProvider _testRunnerProvider;
        protected Mock<IDE> _mockIDE;
        protected CodebaseStateTracker _codebaseStateTracker;
        protected StateBeacon _stateBeacon;
        protected Mock<TestRunnerFactory> _mockTestRunnerFactory;
        protected Mock<SystemClock> _mockClock;
        protected Mock<StateRepository> _mockStateRepository;
        protected ObjectMother.MockDependenciesCollection _stateTrackerDependencies;
        protected Mock<UserNotifier> _mockNotifier;

        [SetUp]
        public void CreateStateBeacon()
        {
            _mockTestRunnerFactory = new Mock<TestRunnerFactory>();
            _testRunnerProvider = new TestRunnerProvider(_mockTestRunnerFactory.Object, new Mock<IDE>().Object);
            _codebaseStateTracker = ObjectMother.GetCodebaseStateTracker(out _stateTrackerDependencies);
            _mockClock = new Mock<SystemClock>();
            _mockNotifier = new Mock<UserNotifier>();

            _stateBeacon = new StateBeacon(_testRunnerProvider, _codebaseStateTracker, _mockNotifier.Object, new MockUiUpdater());

            _mockIDE = _stateTrackerDependencies.Get<IDE>();
            _mockStateRepository = _stateTrackerDependencies.Get<StateRepository>();

        }
    }

    [TestFixture]
    public class TestStateBeacon : TestStateBeaconBase
    {
        protected List<StateTimeSpan> _pastTransitions;

        [Test]
        public void HooksUpTestRunnerToStateTracker_WhenProviderSendsRunnerCreatedNotification()
        {
            GivenThat.TheCodebaseHasNoBuildErrors(_mockIDE);
            var testRunner = SetupMock.TestRunnerFactoryToCreateRunnerFor(
                RunnerType.NUnitResultsFileWatcher, TestConstants.A_TESTRESULTS_FOLDERPATH, _mockTestRunnerFactory);

            _testRunnerProvider.ConfigureTestRunnerFor(
                                    RunnerType.NUnitResultsFileWatcher, TestConstants.A_TESTRESULTS_FOLDERPATH);
            SimulateA.TestFailure(_mockIDE, testRunner);

            Assert.That(_codebaseStateTracker.CurrentState,Is.EqualTo(CodebaseState.Red), "should have seen a red state if test runner was hooked up correctly");
        }

        [Test]
        public void UnhooksStateTrackerFromExistingTestRunner_WhenProviderSendsRunnerDisposingNotification()
        {
            GivenThat.TheCodebaseHasNoBuildErrors(_mockIDE);
            Mock<TestRunner> testRunner = 
                GivenThat.TestRunnerIsConfigured(_testRunnerProvider, _mockTestRunnerFactory);

            _testRunnerProvider.ClearTestRunner();
            SimulateA.TestFailure(_mockIDE, testRunner);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.NoCompileErrors), 
                "should have not be listening to testrunner updates anymore");
        }

        [Test]
        public void RelaysCurrentStateInStateTracker()
        {
            Assert.That(_stateBeacon.CurrentState, Is.EqualTo(CodebaseState.Unknown));

            GivenThat.TheCodebaseHasNoBuildErrors(_mockIDE);

            Assert.That(_stateBeacon.CurrentState, Is.EqualTo(CodebaseState.NoCompileErrors));
        }


        [Test]
        public void NotifiesChangeInCurrentState()
        {
            var listener = new PropertyChangeListener(_stateBeacon);
            
            _mockIDE.Raise(ide => ide.BuildInitiated += null, EventArgs.Empty);

            Assert.That(listener.HasReceivedChangeNotificationFor("CurrentState"), "ViewModel should have notified listeners when the codebase state changes");
            Assert.That(_stateBeacon.CurrentState, Is.EqualTo(CodebaseState.Compiling));
        }

        [Test]
        public void DisposeFlushesCodeStateTracker()
        {
            _mockStateRepository.Setup(
                repository => repository.LogTransition(It.IsAny<DateTime>(), It.IsAny<CodebaseState>(), It.IsAny<long>()));

            _stateBeacon.Dispose();

            _mockStateRepository.VerifyAll();
        }

        [Test]
        public void HasNoPhaseInformationOnCreation()
        {
            Assert.That(_stateBeacon.StateDistributionSnapshots, Is.Empty);
        }


        [Test]
        public void ShowsUpdatedElapsedTime_WhenItReceivesAnElapsedTimeUpdate()
        {
            _stateTrackerDependencies.Get<TransitionTimer>().Raise(timer => timer.ElapsedTimeInCurrentState += null,
                             new DurationEventArgs(TimeSpan.FromMilliseconds(125001)));
            Assert.AreEqual("00:02:05", _stateBeacon.ElapsedTimeInCurrentState);
        }

        [Test]
        public void NotifiesChangeInElapsedTimeInCurrentState()
        {
            var lastPropertyChanged = String.Empty;
            _stateBeacon.PropertyChanged += (sender, args) => lastPropertyChanged = args.PropertyName;

            _stateTrackerDependencies.Get<TransitionTimer>().Raise(timer => timer.ElapsedTimeInCurrentState += null, new DurationEventArgs(TimeSpan.MinValue));

            Assert.That(lastPropertyChanged, Is.EqualTo("ElapsedTimeInCurrentState"), "ElapsedTimeInCurrentState should have been updated");
        }

        [Test]
        public void HasAPauseButtonToPauseStateTracking()
        {
            var mockTimer = _stateTrackerDependencies.Get<TransitionTimer>();
            
            _stateBeacon.Pause();
            
            mockTimer.Verify(timer => timer.Pause());
        }

        [Test]
        public void HasAPauseButtonThatResumesStateTrackingIfClickedTwice()
        {
            _stateBeacon.Pause();
            var mockTimer = _stateTrackerDependencies.Get<TransitionTimer>();

            _stateBeacon.Resume();

            mockTimer.Verify(timer => timer.Resume());
        }

        [Test]
        public void DefaultsToFiveMinuteThresholdForRutDetection()
        {
            Assert.That(_stateBeacon.RutThreshold, Is.EqualTo(TimeSpan.FromMinutes(5)));
        }

        protected void AppendTransitions(params Tuple<CodebaseState, int>[] pastStates)
        {
            
            var timestamp = _pastTransitions.Count == 0 
                                ? DateTime.Parse("2010-08-24 15:00:00") 
                                : _pastTransitions.Last().StartTime.AddMilliseconds(_pastTransitions.Last().DurationInMilliSecs);

            foreach(var state in pastStates)
            {    
                _pastTransitions.Add(new StateTimeSpan(state.Item1, timestamp, state.Item2 * 1000));
                timestamp = timestamp.AddSeconds(state.Item2);
            }
        }
    }
}
