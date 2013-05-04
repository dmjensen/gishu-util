// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;

namespace ManiaX.Beacons.Roles
{
    public interface TestRunner: IDisposable
    {
        event EventHandler<TestResultEventArgs> TestsPassed;
        event EventHandler<FailedTestRunEventArgs> TestsFailed;
        void RunTests();
    }

    public class TestResultEventArgs : EventArgs
    {
        private readonly int _testCount;

        public TestResultEventArgs(int testCount)
        {
            _testCount = testCount;
        }

        public int TestCount
        {
            get { return _testCount; }
        }
    }
}