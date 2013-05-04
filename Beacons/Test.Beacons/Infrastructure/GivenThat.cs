// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using Moq;
using ManiaX.Beacons.ViewModels;

namespace ManiaX.Test.Beacons.Infrastructure
{
    public static class GivenThat
    {
        public static void TheCodebaseHasNoBuildErrors(Mock<IDE> mockIDE)
        {
            mockIDE.Raise(m => m.SolutionOpened += null, EventArgs.Empty);
            SimulateA.SuccessfulBuild(mockIDE);
        }

        public static void AllTestsArePassing(Mock<IDE> mockIDE, Mock<TestRunner> mockTestRunner)
        {
            mockIDE.Raise(m => m.SolutionOpened += null, EventArgs.Empty);
            SimulateA.SuccessfulTestRun(mockIDE, mockTestRunner);
        }

        public static Mock<TestRunner> TestRunnerIsConfigured(TestRunnerProvider testRunnerProvider, Mock<TestRunnerFactory> mockTestRunnerFactory)
        {
            var testRunner = SetupMock.TestRunnerFactoryToCreateRunnerFor(RunnerType.NUnitResultsFileWatcher, 
                                                                            TestConstants.A_TESTRESULTS_FOLDERPATH, 
                                                                            mockTestRunnerFactory);
            testRunnerProvider.ConfigureTestRunnerFor(
                                RunnerType.NUnitResultsFileWatcher, TestConstants.A_TESTRESULTS_FOLDERPATH);
            return testRunner;
        }
    }
}