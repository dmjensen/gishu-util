// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Collections.Generic;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using ManiaX.Beacons.ViewModels;
using ManiaX.Test.Beacons.Infrastructure;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons
{
    [TestFixture]
    public class TestMainViewModel
    {
        private ObjectMother.MockDependenciesCollection _stateTrackerDependencies;
        private MainViewModel _mainViewModel;
        private Mock<Stopwatch> _mockRhythmBeaconStopwatch;
        private Mock<TransitionTimer> _mockStateBeaconTimer;

        [SetUp]
        public void CreateMainViewModel()
        {
            var mockUiUpdater = new MockUiUpdater();
            var runnerProvider = new TestRunnerProvider(new Mock<TestRunnerFactory>().Object, new Mock<IDE>().Object);

            var stateTracker = ObjectMother.GetCodebaseStateTracker(out _stateTrackerDependencies);
            var stateBeacon= new StateBeacon(runnerProvider, stateTracker, new Mock<UserNotifier>().Object, mockUiUpdater);
            
            _mockRhythmBeaconStopwatch = new Mock<Stopwatch>();
            var tddRhythmBeacon = new TDDRhythmBeacon(runnerProvider, _mockRhythmBeaconStopwatch.Object, mockUiUpdater);
            var configVM = new TestRunnerConfiguration(runnerProvider, new Mock<FileSystem>().Object, mockUiUpdater);
            _mainViewModel = new MainViewModel(stateBeacon, tddRhythmBeacon, configVM, mockUiUpdater);

            _mockStateBeaconTimer = _stateTrackerDependencies.Get<TransitionTimer>();
        }

        [Test]
        public void DisposeDisposesBeacons()
        {
            var stateRepository = _stateTrackerDependencies.Get<StateRepository>();
            
            _mainViewModel.Dispose();

            stateRepository.Verify(
                repository => repository.LogTransition(It.IsAny<DateTime>(), 
                                                            It.IsAny<CodebaseState>(), It.IsAny<long>()),
                Times.Once(),
                "should log last state from StateBeacon on exit");
            Assert.That(_mainViewModel.TestRunnerConfigVM.ConfigArguments, Is.EqualTo(String.Empty), 
                            "should clear any existing test runner");
        }

        [Test]
        public void CanPauseAllBeaconTracking()
        {
            _mainViewModel.TogglePauseCommand.Execute(null);
            
            _mockStateBeaconTimer.Verify(timer => timer.Pause());
            _mockRhythmBeaconStopwatch.Verify(sw => sw.Pause());
        }

        [Test]
        public void CanResumeAllBeaconTracking()
        {
            _mainViewModel.TogglePauseCommand.Execute(null);

            _mainViewModel.TogglePauseCommand.Execute(null);
            
            _mockStateBeaconTimer.Verify(timer => timer.Resume());
            _mockRhythmBeaconStopwatch.Verify(sw => sw.Resume());
        }

        [Test]
        public void NotifiesChangeIn_IsPaused()
        {
            var notificationsReceived = new List<string>();
            _mainViewModel.PropertyChanged += (sender, args) => notificationsReceived.Add(args.PropertyName);

            _mainViewModel.TogglePauseCommand.Execute(null);
            Assert.That(notificationsReceived, Contains.Item("IsPaused"), 
                "should notify observers when IsPaused changes");
        }

        [Test]
        public void UpdatesTogglePauseTooltipBasedOnState()
        {
            const string expectedPauseTooltip = "Pause all beacons";
            const string expectedResumeTooltip = "Resume all beacons";
            Assert.That(_mainViewModel.TogglePauseTooltip, Is.EqualTo(expectedPauseTooltip));

            _mainViewModel.TogglePauseCommand.Execute(null);
            Assert.That(_mainViewModel.TogglePauseTooltip, Is.EqualTo(expectedResumeTooltip));

            _mainViewModel.TogglePauseCommand.Execute(null);
            Assert.That(_mainViewModel.TogglePauseTooltip, Is.EqualTo(expectedPauseTooltip));
        }

        [Test]
        public void NotifiesChangeIn_TogglePauseTooltip()
        {
            var listener = new PropertyChangeListener(_mainViewModel);

            _mainViewModel.TogglePauseCommand.Execute(null);

            Assert.That(listener.HasReceivedChangeNotificationFor("TogglePauseTooltip"));
        }
    }
}
