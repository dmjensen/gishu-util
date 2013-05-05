using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCaseSourceExtension
{
    public class CompositeTestResult 
    {
        private int _counter = 0;
        private bool _hasTestFailed = false;
        private StringBuilder _buffer = new StringBuilder();

        public CompositeTestResult()
        {
            Log("Description", "Outcome");
        }

        private void Log(string description, string outcome)
        {
            _buffer.AppendFormat("{0} | {1}", description, outcome);
            _buffer.AppendLine();
        }

        public void AddResult(TestMethodInvokerResult result, TestCase testCase)
        {
            _counter++;
            string outcome;
            if (result.Exception != null)
            {
                _hasTestFailed = true;
                outcome = GetFailureMessage(result.Exception);
            }
            else
            {
                outcome = "Passed";
            }

            

            Log(testCase.ToString(), outcome);
        }

        public TestMethodInvokerResult Result
        {
            get
            {
                var result = new TestMethodInvokerResult {ExtensionResult = _buffer.ToString()};

                if (_hasTestFailed)
                    result.Exception = new AssertFailedException("Click on the Output link below for details..");

                return result;
            }
        }

        private string GetFailureMessage(Exception exception)
        {
            var ex = exception;
            while (ex.InnerException != null)
                ex = ex.InnerException;

            return "Failed. => " + ex;
        }

        
    }

    
}