// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using ManiaX.Beacons.Roles;
﻿using ManiaX.Beacons.ViewModels;

namespace ManiaX.Beacons
{
    public class TestRunnerProvider
    {
        private readonly TestRunnerFactory _testRunnerFactory;
        private readonly IDE _ide;
        private static readonly NullTestRunner NullRunner = new NullTestRunner();

        public TestRunnerProvider(TestRunnerFactory testRunnerFactory, IDE ide)
        {
            UnitTestRunner = NullRunner;
            _testRunnerFactory = testRunnerFactory;
            _ide = ide;
            _ide.BuildSucceeded += RunTests;
        }

        private TestRunner UnitTestRunner { get; set; }

        public void ConfigureTestRunnerFor(RunnerType type, string configParameters)
        {
            if (configParameters == null)
                throw new ArgumentException("Test Results File Path cannot be null/blank");
            configParameters = configParameters.Trim();
            if (configParameters == string.Empty)
                throw new ArgumentException("Test Results File Path cannot be null/blank");

            UnitTestRunner = _testRunnerFactory.CreateTestRunner(type, configParameters);
            if (this.TestRunnerCreated != null)
                this.TestRunnerCreated(this, new TestRunnerEventArgs(UnitTestRunner));
            
        }

        public void ClearTestRunner()
        {
            if (UnitTestRunner == NullRunner) return;

            if (this.TestRunnerDisposing != null)
                this.TestRunnerDisposing(this, new TestRunnerEventArgs(UnitTestRunner));

            UnitTestRunner.Dispose();
            UnitTestRunner = NullRunner;
        }

        public void Dispose()
        {
            _ide.BuildSucceeded -= RunTests;
        }

        public event EventHandler<TestRunnerEventArgs> TestRunnerDisposing;

        public event EventHandler<TestRunnerEventArgs> TestRunnerCreated;

        private void RunTests(object sender, EventArgs e)
        {
            UnitTestRunner.RunTests();
        }

        private class NullTestRunner : TestRunner
        {
            public void RunTests()
            {
            }

            public void Dispose()
            {}

            public event EventHandler<TestResultEventArgs> TestsPassed;

            public event EventHandler<FailedTestRunEventArgs> TestsFailed;
        }
    }

    public class TestRunnerEventArgs : EventArgs
    {
        public TestRunnerEventArgs(TestRunner runner)
        {
            UnitTestRunner = runner;
        }

        public TestRunner UnitTestRunner { get; private set; }
    }
}