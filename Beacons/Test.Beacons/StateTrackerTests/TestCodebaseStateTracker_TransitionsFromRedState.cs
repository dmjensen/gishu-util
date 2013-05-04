// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using ManiaX.Beacons;
using ManiaX.Test.Beacons.Infrastructure;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateTrackerTests
{
    [TestFixture]
    public class TestCodebaseStateTracker_TransitionsFromRedState : TestCodebaseStateTrackerBase
    {
        [SetUp]
        public void GivenThatSomeTestsAreFailing()
        {
            SetUpTestFailedState(_mockIDE, _mockTestRunner);
        }

        [Test]
        public void EntersUnknownStateIfSolutionIsClosed()
        {
            _mockIDE.Raise(ide => ide.SolutionClosed += null, EventArgs.Empty);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Unknown));
        }

        [Test]
        public void EntersCompilingStateIfBuildInitiated()
        {
            _mockIDE.Raise(ide => ide.BuildInitiated += null, EventArgs.Empty);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Compiling));
        }

        [Test]
        public void EntersGreenStateOnTestsPassedNotification()
        {
            SimulateA.SuccessfulTestRun(_mockTestRunner);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Green));
        }

        [Test]
        public void RemainsInRedStateOnTestsPassedNotification()
        {
            SimulateA.TestFailure(_mockTestRunner);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Red));
        }
    }
}
