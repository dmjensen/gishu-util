using System.Collections.Generic;

namespace ManiaX.Beacons.Roles
{
    public class FailedTestRunEventArgs : TestResultEventArgs
    {
        public IEnumerable<FailureDetails> Failures { get; private set; }

        public FailedTestRunEventArgs(int testCount, IEnumerable<FailureDetails> failedTests) : base(testCount)
        {
            Failures = failedTests;
        }
    }

    public class FailureDetails
    {
        public string TestName { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public FailureDetails(string testName, string message, string stacktrace)
        {
            TestName = testName;
            Message = message;
            StackTrace = stacktrace;
        }
    }


}