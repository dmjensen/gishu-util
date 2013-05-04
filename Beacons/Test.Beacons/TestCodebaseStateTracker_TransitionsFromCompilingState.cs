// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using ManiaX.Beacons;
﻿using ManiaX.Test.Beacons.Infrastructure;
﻿using NUnit.Framework;

namespace ManiaX.Test.Beacons.StateTrackerTests
{
    [TestFixture]
    public class TestCodebaseStateTracker_TransitionsFromCompilingState : TestCodebaseStateTrackerBase
    {
        [SetUp]
        public void GivenThatTheCodebaseIsBuilding()
        {
            _mockIDE.Raise(m => m.BuildInitiated += null, EventArgs.Empty);
        }

        [Test]
        public void EntersNoCompileErrorsState_IfBuildSuccessNotificationReceived()
        {
            _mockIDE.Raise(m => m.BuildSucceeded += null, EventArgs.Empty);
            
            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.NoCompileErrors));
        }
        

        [Test]
        public void EntersCompileErrorsState_IfBuildSuccessNotificationReceived()
        {
            _mockIDE.Raise(m => m.BuildFailed += null, EventArgs.Empty);
            
            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.CompileErrors));
        }

        [Test]
        public void IgnoresTestsSucceededNotification()
        {
            SimulateA.SuccessfulTestRun(_mockTestRunner);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Compiling));
        }

        [Test]
        public void IgnoresTestsFailedNotification()
        {
            SimulateA.TestFailure(_mockTestRunner);

            Assert.That(_codebaseStateTracker.CurrentState, Is.EqualTo(CodebaseState.Compiling));
        }
    }
}