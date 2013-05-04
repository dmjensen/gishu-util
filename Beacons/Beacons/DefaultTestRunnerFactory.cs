// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using ManiaX.Beacons.Roles;
﻿using ManiaX.Beacons.ViewModels;

namespace ManiaX.Beacons
{
    public class DefaultTestRunnerFactory : TestRunnerFactory
    {
        public TestRunner CreateTestRunner(RunnerType runnerType, string configParameters)
        {
            if (runnerType == RunnerType.NUnitResultsFileWatcher)
                return new NUnitResultsWatcher(configParameters);

            return new NUnitConsoleRunner(configParameters);
        }
    }
}