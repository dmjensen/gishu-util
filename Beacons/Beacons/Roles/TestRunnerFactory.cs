// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using ManiaX.Beacons.ViewModels;

namespace ManiaX.Beacons.Roles
{
    public interface TestRunnerFactory
    {
        TestRunner CreateTestRunner(RunnerType runnerType, string configParameters);
    }
}