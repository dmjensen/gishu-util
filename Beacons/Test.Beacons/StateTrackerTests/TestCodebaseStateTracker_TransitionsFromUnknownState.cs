// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using ManiaX.Beacons;
using ManiaX.Test.Beacons.Infrastructure;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateTrackerTests
{
    [TestFixture]
    public class TestCodebaseStateTracker_TransitionsFromUnknownState : TestCodebaseStateTrackerBase
    {
        [Test]
        public void DoesNotChangeStateIfNewSolutionIsOpened()
        {
            _mockIDE.Raise(m => m.SolutionOpened += null, EventArgs.Empty);
 
            Assert.AreEqual(CodebaseState.Unknown, _codebaseStateTracker.CurrentState);
        }

        [Test]
        public void DoesNotChangeStateIfSolutionIsClosed()
        {
            _mockIDE.Raise(m => m.SolutionClosed += null, EventArgs.Empty);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Unknown));
        }

        [Test]
        public void EntersCompilingStateIfBuildInitiated()
        {
            _mockIDE.Raise(m => m.BuildInitiated += null, EventArgs.Empty);
            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Compiling));
        }

        [Test]
        public void IgnoresTestsPassedUpdate()
        {
            SimulateA.SuccessfulTestRun(_mockTestRunner);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Unknown));
        }

        [Test]
        public void IgnoresTestsFailedUpdate()
        {
            SimulateA.TestFailure(_mockTestRunner);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Unknown));
        }
    }
}