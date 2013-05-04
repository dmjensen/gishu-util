// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using ManiaX.Beacons.Roles;
using ManiaX.Test.Beacons.IntegrationTests;
using Moq;

namespace ManiaX.Test.Beacons.Infrastructure
{
    public static class SimulateA
    {
        public static void BuildFailure(Mock<IDE> mockIDE)
        {
            mockIDE.Raise(ide => ide.BuildInitiated += null, EventArgs.Empty);
            mockIDE.Raise(ide => ide.BuildFailed += null, EventArgs.Empty);
        }
        public static void SuccessfulBuild(Mock<IDE> mockIDE)
        {
            mockIDE.Raise(ide => ide.BuildInitiated += null, EventArgs.Empty);
            mockIDE.Raise(ide => ide.BuildSucceeded+= null, EventArgs.Empty);
        }

        public static void TestFailure(Mock<IDE> mockIDE, Mock<TestRunner> mockTestRunner)
        {
            SuccessfulBuild(mockIDE);
            TestFailure(mockTestRunner);
        }

        public static void TestFailure(Mock<TestRunner> mockTestRunner)
        {
            TestFailureWith(mockTestRunner, new FailureDetails("MyTest", "Expected Good But was Bad", "somestacktrace"));
        }

        public static void TestFailureWith(Mock<TestRunner> mockTestRunner, params FailureDetails[] failureDetails)
        {
            mockTestRunner.Raise(runner => runner.TestsFailed += null, 
                                 new FailedTestRunEventArgs(1, failureDetails));
        }

        public static void SuccessfulTestRun(Mock<IDE> mockIDE, Mock<TestRunner> mockTestRunner)
        {
            SuccessfulBuild(mockIDE);
            SuccessfulTestRun(mockTestRunner);
        }

        public static void SuccessfulTestRun(Mock<TestRunner> mockTestRunner, int numberOfTests = 100)
        {
            mockTestRunner.Raise(tr => tr.TestsPassed += null, new TestResultEventArgs(numberOfTests));
        }
    }
}