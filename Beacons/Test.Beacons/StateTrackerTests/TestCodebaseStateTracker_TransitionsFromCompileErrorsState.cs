// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using ManiaX.Beacons;
using ManiaX.Test.Beacons.Infrastructure;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateTrackerTests
{
    [TestFixture]
    public class TestCodebaseStateTracker_TransitionsFromCompileErrorsState : TestCodebaseStateTrackerBase
    {
        [SetUp]
        public void GivenTheCodebaseHasCompileErrors()
        {
            SetUpBuildBrokenState(_mockIDE);
        }

        [Test]
        public void GoesBackToUnknownStateIfSolutionIsClosed()
        {
            _mockIDE.Raise(m => m.SolutionClosed += null, EventArgs.Empty);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Unknown));
        }

        [Test]
        public void EntersCompilingStateIfBuildIsInitiated()
        {
            _mockIDE.Raise(m => m.BuildInitiated += null, EventArgs.Empty);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Compiling));
        }

        [Test]
        public void IgnoresTestsPassedNotification()
        {
            SimulateA.SuccessfulTestRun(_mockTestRunner);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.CompileErrors));
        }

        [Test]
        public void IgnoresTestsFailedNotification()
        {
            SimulateA.TestFailure(_mockTestRunner);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.CompileErrors));
        }
    }
}
