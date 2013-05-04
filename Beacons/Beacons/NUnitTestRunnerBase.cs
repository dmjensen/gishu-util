// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ManiaX.Beacons.Roles;

namespace ManiaX.Beacons
{
    public abstract class NUnitTestRunnerBase : TestRunner
    {
        
        public abstract void RunTests();
        public abstract void Dispose();

        public event EventHandler<TestResultEventArgs> TestsPassed;
        public event EventHandler<FailedTestRunEventArgs> TestsFailed;
        
        protected void NotifyTestResults(XElement resultRootNode)
        {
            var numberOfFailedTests = GetNumberOfFailures(resultRootNode);
            if (numberOfFailedTests == 0)
            {
                var numberOfPassingTests = GetNumberOfPassingTests(resultRootNode);

                if (this.TestsPassed != null)
                    this.TestsPassed(this, new TestResultEventArgs(numberOfPassingTests));
            }
            else
            {
                if (this.TestsFailed != null)
                    this.TestsFailed(this, 
                                     new FailedTestRunEventArgs(numberOfFailedTests, 
                                                                GetDetailsForFailedTests(resultRootNode)));
            }
        }

        protected static string GetTimeOfLastTestRun(XElement root)
        {
            return GetAttributeValue(root, "time", s => s);
        }

        private static IEnumerable<FailureDetails> GetDetailsForFailedTests(XElement root)
        {
            return root.Descendants(XName.Get("failure"))
                .Select(GetFailureInformation);
        }

        private static FailureDetails GetFailureInformation(XElement failedTest)
        {
            
            var message = failedTest.Element(XName.Get("message")).Value;
            var lines = message.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim());
            message = String.Join(" | ", lines);

            var stacktrace = failedTest.Element(XName.Get("stack-trace")).Value;

            var testName = GetAttributeValue(failedTest.Parent, "name", s => s);
            testName = String.Join(".", testName.Split('.').Reverse().Take(2).Reverse());

            return new FailureDetails(testName, message, stacktrace);
        }

        private static int GetNumberOfFailures(XElement root)
        {
            return  GetAttributeValue(root, "failures", Int32.Parse)
                    + GetAttributeValue(root, "errors", Int32.Parse);
        }

        private static int GetNumberOfPassingTests(XElement root)
        {
            return GetAttributeValue(root, "total", Int32.Parse);
        }

        private static T GetAttributeValue<T>(XElement node, string attributeName, Converter<string, T> converter)
        {
            var attributeToFetch = node.Attribute(attributeName);
            if (attributeToFetch == null)
                return default(T);
            return converter(attributeToFetch.Value);
        }
    }
}