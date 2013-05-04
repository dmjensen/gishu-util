// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.IO;
using System.Linq;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.IntegrationTests
{
    [Category("Integration Tests Slow")]
    [TestFixture]
    public class TestNUnitResultsWatcher
    {
        private NUnitResultsWatcher _watcher;
        private string _testResultsFilePath;

        [SetUp]
        public void BeforeEachTest()
        {
            FileIOHelper.CreateEmptyTempFolder();
            _testResultsFilePath = Path.Combine(FileIOHelper.TempFolderPath, "TestResult.xml");
            _watcher = new NUnitResultsWatcher(FileIOHelper.TempFolderPath);
        }

        [TearDown]
        public void AfterEachTest()
        {
            _watcher.Dispose();
        }
        [TestFixtureTearDown]
        public void AfterAllTests()
        {
            FileIOHelper.DeleteTempFolderIfItExists();
        }

        [Test]
        public void RaisesPassedEventIfAllTestsPassed()
        {
            var trace = new AsyncNotificationTrace("should contain exactly one *tests passed* notification");
            _watcher.TestsPassed += trace.Handler;

            File.Copy(FileIOHelper.GetResourceFilePath("Pass.xml"), _testResultsFilePath);

            trace.VerifyAfter(TimeSpan.FromSeconds(2), trace.Received(1).Notification);
            Assert.That(trace.VerifyEventArgsAt(0), Has.Property("TestCount").EqualTo(33));
        }
        [Test]
        public void RaisesFailedEventIfSomeTestsFailed()
        {
            var trace = new AsyncNotificationTrace("should contain exactly one *tests failed* notification");
            _watcher.TestsFailed += trace.Handler;

            File.Copy(FileIOHelper.GetResourceFilePath("Fail.xml"), _testResultsFilePath);

            trace.VerifyAfter(TimeSpan.FromSeconds(2), trace.Received(1).Notification);
            Assert.That(trace.VerifyEventArgsAt(0), Has.Property("TestCount").EqualTo(1));
        }

        [Test]
        public void ReturnsDetailsOfAllFailedTests()
        {
            var trace =
                new AsyncNotificationTrace("should contain exactly one *test failed* notification with all failure details ");
            _watcher.TestsFailed += trace.Handler;

            SimulateNewTestRun("MultipleFailures.xml");

            trace.VerifyAfter(TimeSpan.FromSeconds(2), trace.Received(1).Notification);
            var eventArgs = trace.VerifyEventArgsAt(0) as FailedTestRunEventArgs;

            Assert.That(eventArgs, Is.Not.Null);
            var firstFailure = eventArgs.Failures.First();
            Assert.That(firstFailure.TestName, Is.EqualTo("TestMath.Adds"));
            Assert.That(firstFailure.Message, Is.EqualTo("Expected: 10 | But was:  11"));
            Assert.That(firstFailure.StackTrace, Is.EqualTo(@"at StringCalc.TestMath.Adds() in T:\code\csharp\Kata-StringCalc\StringCalc\TestStack.cs:line 13" + "\n"));

            var secondFailure = eventArgs.Failures.Skip(1).First();
            Assert.That(secondFailure.TestName, Is.EqualTo("TestMath.AddsAgain"));
            Assert.That(secondFailure.Message, Is.EqualTo("System.Exception : Exception of type 'System.Exception' was thrown."));
            Assert.That(secondFailure.StackTrace,
                        Is.EqualTo(
                            @"at StringCalc.TestMath.Kaboom() in T:\code\csharp\Kata-StringCalc\StringCalc\TestStack.cs:line 23"                            + "\n"
                            + @"at StringCalc.TestMath.AddsAgain() in T:\code\csharp\Kata-StringCalc\StringCalc\TestStack.cs:line 18" 
                            + "\n"));

        }

        [Test]
        public void RaisesFailedEventIfSomeTestsFailedDueToErrors()
        {
            var trace = new AsyncNotificationTrace("should contain exactly one *tests failed* notification");
            _watcher.TestsFailed += trace.Handler;

            File.Copy(FileIOHelper.GetResourceFilePath("Errors.xml"), _testResultsFilePath);

            trace.VerifyAfter(TimeSpan.FromSeconds(2), trace.Received(1).Notification);
            Assert.That(trace.VerifyEventArgsAt(0), Has.Property("TestCount").EqualTo(1));
        }

        [Test]
        public void ExcludesIgnoredTestsFromNumberOfPassingTests()
        {
            var trace = new AsyncNotificationTrace("should contain exactly one *tests passed* notification");
            _watcher.TestsPassed += trace.Handler;

            File.Copy(FileIOHelper.GetResourceFilePath("PassWithIgnores.xml"), _testResultsFilePath);

            trace.VerifyAfter(TimeSpan.FromSeconds(2), trace.Received(1).Notification);
            Assert.That(trace.VerifyEventArgsAt(0), Has.Property("TestCount").EqualTo(60));
        }

        [Test]
        public void SuppressXmlExceptionsCausedByTestRunnerStillWritingResultsInParallel()
        {
            var trace =
                new AsyncNotificationTrace(
                    "should not contain any notifications since file is malformed / still being written to by NUnit");
            _watcher.TestsPassed += trace.Handler;
            _watcher.TestsFailed += trace.Handler;

            File.Copy(FileIOHelper.GetResourceFilePath("HalfWrittenResultsFile.xml"), _testResultsFilePath);

            trace.VerifyAfter(TimeSpan.FromSeconds(2), trace.Received(0).Notifications);
        }

        // sometimes it has been seen that the filewatcher raises multiple callbacks as nunit writes out the results file
        // first I tried a timer that waits for 10 secs before reading the file. Subsequent callbacks reset the timer
        // however this has a side effect that if the user runs a small suite multiple times within 10 secs, only 1 event is raised
        // Also on some slow machines, filewatcher gave multiple callbacks spaced more than 10 secs apart
        // MORAL OF THE STORY : Dumping timers, eating up exceptions caused by malformed xml
        // and ignoring subsequent callbacks if an event has already been raised for the test run timestamp in the nunit results

        [Test]
        public void DoesNotRaiseMultipleEventsPerTestRun()
        {
            var trace = new AsyncNotificationTrace("should contain exactly one *tests failed* notification even if file watcher callsback multiple times");
            _watcher.TestsFailed += trace.Handler;

            File.Copy(FileIOHelper.GetResourceFilePath("Fail.xml"), _testResultsFilePath);
            System.Threading.Thread.Sleep(2000);
            File.Copy(FileIOHelper.GetResourceFilePath("Fail.xml"), _testResultsFilePath, true);

            trace.VerifyAfter(TimeSpan.FromSeconds(2), trace.Received(1).Notification);
            Assert.That(trace.VerifyEventArgsAt(0), Has.Property("TestCount").EqualTo(1));
        }

        private void SimulateNewTestRun(string resultsFileName)
        {
            File.Copy(FileIOHelper.GetResourceFilePath(resultsFileName), _testResultsFilePath);
        }
    }

    
}
