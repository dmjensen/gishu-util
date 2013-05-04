// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using ManiaX.Beacons;
using ManiaX.Test.Beacons.Infrastructure;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateTrackerTests
{
    [TestFixture]
    public class TestCodebaseStateTracker_TransitionsFromNoCompileErrorsState : TestCodebaseStateTrackerBase
    {
        [SetUp]
        public void GivenTheCodebaseHasNoCompileErrors()
        {
            GivenThat.TheCodebaseHasNoBuildErrors(_mockIDE);
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
        public void EntersGreenStateIfTestsPassed()
        {
            SimulateA.SuccessfulTestRun(_mockTestRunner);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Green));
        }

        [Test]
        public void EntersRedStateIfTestsFailed()
        {
            SimulateA.TestFailure(_mockTestRunner);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Red));
        }
    }
}