// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System.Collections.Generic;
using System.Linq;
using ManiaX.Beacons.DataStructs;
using ManiaX.Beacons.Roles;
using ManiaX.Test.Beacons.Infrastructure;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons.TDDRhythmBeaconTests
{
    [TestFixture]
    public class TestTDDRhythmBeacon : TestTDDRhythmBeaconBase
    {
        private Mock<TestRunner> _mockTestRunner;

        [SetUp]
        public void GivenThatTestRunnerIsConfigured()
        {
            _mockTestRunner = GivenThat.TestRunnerIsConfigured(_testRunnerProvider, _mockTestRunnerFactory);
        }
        
        
        [Test]
        public void StacksMultipleTestResultsAsTheyOccur()
        {
            Assert.That(!_beacon.IsRefactoringHatOn);

            SimulateA.SuccessfulTestRun(_mockTestRunner, 100);
            SimulateA.TestFailure(_mockTestRunner);
            SimulateA.SuccessfulTestRun(_mockTestRunner, 101);
            
            Assert.IsTrue( AreTestRunCollectionsEqual(_beacon.TestResultsStack,
                                                      new TestRun[]
                                                          {
                                                              new TestRun { Result = TestResult.Green, TestCount = 101},
                                                              new TestRun { Result = TestResult.Red, TestCount = 1},
                                                              new TestRun { Result = TestResult.Green, TestCount = 100},
                                                          }));
        }

        [Test]
        public void MostRecentTestResultIsAutomaticallySelected()
        {
            SimulateA.SuccessfulTestRun(_mockTestRunner);
            
            Assert.That(_beacon.SelectedTestRun, Is.EqualTo(_beacon.TestResultsStack[0]),
                            "Latest green result not selected by default");

            SimulateA.TestFailure(_mockTestRunner);
            
            Assert.That(_beacon.SelectedTestRun, Is.EqualTo(_beacon.TestResultsStack[0]),
                            "Latest red result not selected by default");
        }

        [Test]
        public void ExpandsStackTraceIfOnlyOneTestHasFailed()
        {
            const string TESTNAME = "aTest";
            const string FAILURE_MESSAGE = "aFailureMessage";
            const string STACKTRACE = "aStackTrace";
            var dummyFailureDetails = new FailureDetails(TESTNAME, FAILURE_MESSAGE, STACKTRACE);
            SimulateA.TestFailureWith(_mockTestRunner, dummyFailureDetails);

            var lastResult = _beacon.TestResultsStack[0];
            Assert.That(lastResult.Result, Is.EqualTo(TestResult.Red));
            var failure = lastResult.Failures.First();
            Assert.That(failure.TestName, Is.EqualTo(TESTNAME));
            Assert.That(failure.Message, Is.EqualTo(FAILURE_MESSAGE));
            Assert.That(failure.StackTraceExpanded, Is.True);
            Assert.That(failure.StackTrace, Is.EqualTo(STACKTRACE));
        }

        [Test]
        public void CollapsesStackTraces_IfMultipleTestsHaveFailed()
        {
            const string TESTNAME = "aTest";
            const string FAILURE_MESSAGE = "aFailureMessage";
            const string STACKTRACE = "aStackTrace";
            var dummyFailureDetails = new FailureDetails(TESTNAME, FAILURE_MESSAGE, STACKTRACE);
            var dummyFailureDetails2 = new FailureDetails(TESTNAME+"2", FAILURE_MESSAGE+"2", STACKTRACE+"2");
            SimulateA.TestFailureWith(_mockTestRunner, dummyFailureDetails, dummyFailureDetails2);

            var lastResult = _beacon.TestResultsStack[0];
            Assert.That(lastResult.Failures.All( failure => !failure.StackTraceExpanded), Is.True,
                            "should have collapsed all stacktraces to save screen space");
        }

        [Test]
        public void StopsListeningWhenTestRunnerIsCleared()
        {
            
            _testRunnerProvider.ClearTestRunner();
            
            SimulateA.SuccessfulTestRun(_mockTestRunner);
            SimulateA.TestFailure(_mockTestRunner);

            Assert.That(_beacon.TestResultsStack.ToArray(), Is.Empty);
        }

        [Test]
        public void ReportsTestFailureAsRedIrrespectiveOfCurrentHat()
        {
            _beacon.ToggleRefactoringHat.Execute(null);
            SimulateA.TestFailure(_mockTestRunner);

            _beacon.ToggleRefactoringHat.Execute(null);
            SimulateA.TestFailure(_mockTestRunner);

            Assert.That( AreTestRunCollectionsEqual( _beacon.TestResultsStack, 
                                                     new TestRun[]
                                                         {
                                                             new TestRun { Result = TestResult.Red, TestCount = 1},
                                                             new TestRun { Result = TestResult.Red, TestCount = 1},
                                                         }));
        }

        [Test]
        public void ReportsTestSuccessAsRefactoringWinWhenRefactoringHatIsOn()
        {

            _beacon.ToggleRefactoringHat.Execute(null);
            SimulateA.SuccessfulTestRun(_mockTestRunner);

            Assert.That( AreTestRunCollectionsEqual( _beacon.TestResultsStack, 
                                                     new TestRun[]
                                                         {
                                                             new TestRun { Result = TestResult.RefactoringWin, TestCount = 100}
                                                         }));
        }

        [Test]
        public void NotifiesChangeIn_IsRefactoringHatOn()
        {
            var notificationsReceived = new List<string>();
            _beacon.PropertyChanged += (sender,args) => notificationsReceived.Add(args.PropertyName);
            
            _beacon.ToggleRefactoringHat.Execute(null);

            Assert.That(notificationsReceived, Contains.Item("IsRefactoringHatOn"), 
                        "should notify observers when IsRefactoringHatOn changes");
        }

        private bool AreTestRunCollectionsEqual(IEnumerable<TestRun> list1, IEnumerable<TestRun> list2)
        {
            if (list1.Count() != list2.Count())
                return false;
            for (int i = 0; i < list1.Count(); i++)
                if (!list1.ElementAt(i).HasSameMembersAs(list2.ElementAt(i)))
                    return false;

            return true;
        }
    }

    static class TestRunExtensions
    {
        internal static bool HasSameMembersAs(this TestRun run, TestRun anotherRun)
        {
            return (run.Result == anotherRun.Result)
                   && (run.TestCount == anotherRun.TestCount);
        } 
    }
}
