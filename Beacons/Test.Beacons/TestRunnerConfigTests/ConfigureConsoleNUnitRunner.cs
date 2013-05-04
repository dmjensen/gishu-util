using System;
using System.Collections.Generic;
using System.Linq;
using ManiaX.Beacons.ViewModels;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.TestRunnerConfigTests
{
    [TestFixture]
    public class ConfigureConsoleNUnitRunner : TestSpecificRunnerConfigBase
    {
        protected override RunnerType GetRunnerType()
        {
            return RunnerType.NUnitConsole;
        }

        protected override string GetConfigArguments()
        {
            return @"c:\program files\nunit\nunit-console Some.dll";
        }

        protected override string GetAnotherSetOfConfigArguments()
        {
            return @"c:\program files\nunit\nunit-console SomeOther.dll";
        }

        protected override IEnumerable<string> GetFiveValidConfigurations()
        {
            const string COMMAND_FORMATSTRING = @"c:\program files\nunit\nunit-console Some{0}.dll";
            return new[] {1, 2, 3, 4, 5}.Select(i => String.Format(COMMAND_FORMATSTRING, i));
        }

        protected override string GetValidationErrorIfRunnerIsNotConfigured()
        {
            return "Enter command to run tests via console runner";
        }
    }
}