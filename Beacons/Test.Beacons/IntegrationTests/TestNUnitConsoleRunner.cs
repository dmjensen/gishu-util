// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System.Linq;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.IntegrationTests
{
    [Category("Integration Tests Slow")]
    [TestFixture]
    public class TestNUnitConsoleRunner
    {
        private int _passedTestCount;
        private FailedTestRunEventArgs _failureEventArgs;

        [SetUp]
        public void BeforeEachTest()
        {
            _passedTestCount = 0;
            _failureEventArgs = null;
        }
        [Test]
        public void RaisesPassedEventIfAllTestsPass()
        {
            TestRunner runner = new NUnitConsoleRunner(GetCommandLine("StringCalcTests.dll"));
            runner.TestsPassed += CachePassedCount;

            runner.RunTests();

            Assert.That(_passedTestCount, Is.EqualTo(13), "Should have received 13 tests passed event");
        }

        [Test]
        public void RaisesFailedEventIfSomeTestsFail()
        {
            TestRunner runner = new NUnitConsoleRunner(GetCommandLine("StringCalcTests-Fail.dll"));
            runner.TestsFailed += CacheFailedCount;

            runner.RunTests();

            Assert.That(_failureEventArgs.TestCount, Is.EqualTo(1), "Should have received 1 tests failed event");
        }

        [Test]
        public void RaisesFailedEventIfSomeTestsFailedDueToErrors()
        {
            TestRunner runner = new NUnitConsoleRunner(GetCommandLine("StringCalcTests-Error.dll"));
            runner.TestsFailed += CacheFailedCount;

            runner.RunTests();

            Assert.That(_failureEventArgs.TestCount, Is.EqualTo(13), "Should have received 13 tests failed event");
        }

        [Test]
        public void ExcludesIgnoredTestsFromNumberOfPassingTests()
        {
            _passedTestCount = 0;
            TestRunner runner = new NUnitConsoleRunner(GetCommandLine("StringCalcTests-Ignore.dll"));
            runner.TestsPassed += CachePassedCount;

            runner.RunTests();
            Assert.That(_passedTestCount, Is.EqualTo(11), "Ignored tests should be excluded");
        }

        [Test]
        public void IncludesFailureDetailsWhenTestsFail()
        {
            TestRunner runner = new NUnitConsoleRunner(GetCommandLine("StringCalcTests-Fail.dll"));
            runner.TestsFailed += CacheFailedCount;

            runner.RunTests();

            Assert.That(_failureEventArgs.Failures.Count(), Is.EqualTo(1));
            var firstFailure = _failureEventArgs.Failures.First();
            Assert.That(firstFailure.TestName, Is.EqualTo("TestStringCalculator.AddOutputsTheResultToDisplay"));
            Assert.That(firstFailure.Message, Is.EqualTo("Expected: \"6\" | But was:  null"));
            Assert.That(firstFailure.StackTrace, Is.EqualTo(@"at StringCalcTests.TestStringCalculator.AddOutputsTheResultToDisplay()" + "\n"));
        }

        [TestCase("abc", TestName = "Junk")]
        [TestCase(@"k:\kaboom\nunit-console k:\kaboom\a.dll", TestName = "Invalid filepaths")]
        public void DoesNothingIfRunnerHasNotBeenConfiguredCorrectly(string commandLine)
        {
            // just should not throw an exception - need to improve this with upstream validation
            var runner = new NUnitConsoleRunner(commandLine);
            runner.RunTests();
        }

        private static string GetCommandLine(string testAssemblyName)
        {
            return FileIOHelper.GetResourceFilePath(@"NUnitDistrib\nunit-console") + @" " +
                   FileIOHelper.GetResourceFilePath(@"NunitDistrib\testasm\" + testAssemblyName);
        }

        private void CacheFailedCount(object sender, FailedTestRunEventArgs e)
        {
            _failureEventArgs = e;
        }

        private void CachePassedCount(object sender, TestResultEventArgs e)
        {
            _passedTestCount = e.TestCount;
        }
    }
}
