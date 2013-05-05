/*
 * Copyright (c) 2013 Gishu Pillai (gishu AT hotmail DOT com)
 * 
 * You should have received a copy of the license - see license.txt
 * If not, please refer to it online at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCaseSourceExtension
{
    public class ParamTestInvoker : ITestMethodInvoker
    {
        private readonly TestMethodInvokerContext _context;
        private int _nextAvailableTestCaseId = 0;
        public ParamTestInvoker(TestMethodInvokerContext context)
        {
            _context = context;
        }

        public TestMethodInvokerResult Invoke(params object[] parameters)
        {
            var testContext = _context.TestContext;

            var attributes = _context.TestMethodInfo.GetCustomAttributes(typeof (TestCaseSourceAttribute), false);
            if (attributes.Length == 0)
                return _context.InnerInvoker.Invoke(null);

            var testCaseCollection = GetValuesFromSpecifiedSourceMethod(attributes[0] as TestCaseSourceAttribute);

            var compositeTestResult = new CompositeTestResult();
            foreach (var item in testCaseCollection)
            {
                _nextAvailableTestCaseId++;

                TestMethodInvokerResult result;
                var testCase = GetTestCase(item); 
                //if (param is object[])
                //{
                //    var args = (object[]) param;
                //    result = _context.InnerInvoker.Invoke(args);

                //}
                //else
                //{
                //    result = _context.InnerInvoker.Invoke(param);
                //}
                result = _context.InnerInvoker.Invoke(testCase.Arguments);
                
                compositeTestResult.AddResult(result, testCase);
            }
            
            testContext.WriteLine(compositeTestResult.Result.ExtensionResult.ToString());
            return compositeTestResult.Result;

        }

        private TestCase GetTestCase(object param)
        {
            var testCase = param as TestCase;
            if (testCase != null)
                return testCase;

            var name = "TestCase#" + _nextAvailableTestCaseId;
            if (param is object[])
            {
                return new TestCase((object[]) param).Called(name);
            }
            else
            {
                return new TestCase(param).Called(name);
            }
        }

        private IEnumerable GetValuesFromSpecifiedSourceMethod(TestCaseSourceAttribute paramTestAttribute)
        {
            var methodName = (paramTestAttribute).Source;

            var classType = _context.TestMethodInfo.DeclaringType;

            var methodInfo = classType.GetMethod(methodName, BindingFlags.Default
                                                             | BindingFlags.InvokeMethod | BindingFlags.Public
                                                             | BindingFlags.Instance | BindingFlags.Static |
                                                             BindingFlags.DeclaredOnly);

            var objectOfTestClass = Activator.CreateInstance(classType);
            var testCases = methodInfo.Invoke(objectOfTestClass, null) as IEnumerable;
            return testCases;
        }
    }
}
