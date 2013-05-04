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
    public class TestCodebaseStateTrackerBase
    {
        protected CodebaseStateTracker _codebaseStateTracker;
        protected Mock<IDE> _mockIDE;
        protected Mock<StateRepository> _mockStateRepository;
        protected Mock<TransitionTimer> _mockTimer;
        protected Mock<TestRunner> _mockTestRunner;

        [SetUp]
        public void BeforeEachTest()
        {
            _mockStateRepository = new Mock<StateRepository>();
            _mockTimer = new Mock<TransitionTimer>();

            _mockIDE = new Mock<IDE>(); 
            _mockTestRunner = new Mock<TestRunner>();

            _codebaseStateTracker = new CodebaseStateTracker(_mockIDE.Object,
                                                                _mockStateRepository.Object, _mockTimer.Object);
            _codebaseStateTracker.SetTestRunner(_mockTestRunner.Object);
        }

        public static void SetUpBuildBrokenState(Mock<IDE> mockIde)
        {
            mockIde.Raise(m => m.BuildInitiated += null, EventArgs.Empty);
            mockIde.Raise(m => m.BuildFailed += null, EventArgs.Empty);
        }

        protected void ExpectLogEntry(CodebaseState previousState, long durationInMilliSec)
        {
            _mockTimer.Setup(timer => timer.GetElapsedMillisecAndRestart()).Returns(durationInMilliSec);
            var time = DateTime.Now;
            _mockTimer.Setup(timer => timer.StartedAt()).Returns(time);
            _mockStateRepository.Setup(repository => repository.LogTransition(time, previousState, durationInMilliSec));
        }

        public static void SetUpTestFailedState(Mock<IDE> mockIDE, Mock<TestRunner> mockTestRunner)
        {
            mockIDE.Raise(ide => ide.SolutionOpened += null, EventArgs.Empty);
            SimulateA.TestFailure(mockIDE, mockTestRunner);
        }
    }
}