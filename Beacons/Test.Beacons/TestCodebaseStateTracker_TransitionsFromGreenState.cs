// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using ManiaX.Beacons;
using ManiaX.Test.Beacons.Infrastructure;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateTrackerTests
{
    [TestFixture]
    public class TestCodebaseStateTracker_TransitionsFromGreenState : TestCodebaseStateTrackerBase
    {
        [SetUp]
        public void GivenThatAllTestsAreGreen()
        {
            _mockIDE.Raise(ide => ide.SolutionOpened += null, EventArgs.Empty);
            _mockIDE.Raise(ide => ide.BuildInitiated += null, EventArgs.Empty);
            _mockIDE.Raise(ide => ide.BuildSucceeded += null, EventArgs.Empty);
            SimulateA.SuccessfulTestRun(_mockTestRunner);
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
        public void RemainsInGreenStateOnTestsPassedNotification()
        {
            SimulateA.SuccessfulTestRun(_mockTestRunner);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Green));
        }

        [Test]
        public void EntersRedStateOnTestsPassedNotification()
        {
            SimulateA.TestFailure(_mockTestRunner);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Red));
        }
    }
}
