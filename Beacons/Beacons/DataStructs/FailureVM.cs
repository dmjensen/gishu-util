namespace ManiaX.Beacons.DataStructs
{
    public class FailureVM
    {
        public string TestName { get; private set; }

        public string Message { get; private set; }

        public bool StackTraceExpanded { get; set; }

        public string StackTrace { get; private set; }

        public FailureVM(string testName, string message, bool stackTraceExpanded, string stackTrace)
        {
            TestName = testName;
            Message = message;
            StackTraceExpanded = stackTraceExpanded;
            StackTrace = stackTrace;
        }
    }
}