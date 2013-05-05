/*
 * Copyright (c) 2013 Gishu Pillai (gishu AT hotmail DOT com)
 * 
 * You should have received a copy of the license - see license.txt
 * If not, please refer to it online at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestCaseSourceExtension;

namespace SampleTest
{
    [ParamTestClass]
    public class ExampleTests
    {
        [TestMethod]
        [TestCaseSource("GetInputs")]
        public void SingleParam(string input)
        {
            Assert.AreNotEqual(string.Empty, input);
        }

        public string[] GetInputs()
        {
            return new[] {"Tony", "Pepper", "Mandarin", "Rhodes"};
        }

        [TestMethod, TestCaseSource("DivideCases")]
        public void MultipleParams(int n, int d, int q)
        {
            Assert.AreEqual( q, n / d );
        }

        public static object[] DivideCases()
        {
            return new[]
                       {
                           new object[] {12, 3, 4},
                           new object[] {12, 2, 6},
                           new object[] {12, 4, 3}
                       };
        }

        [TestMethod, TestCaseSource("GetUsers")]
        public void NamedTestCases(string user, string password, bool isAdmin)
        {
            ImagineProductionLogin(user, password, isAdmin);
        }

        public IEnumerable<TestCase> GetUsers()
        {
            yield return new TestCase("gishu", "pillai", true)
                                .Called("test admin user");
            yield return new TestCase("tony", "stark", false)
                                .Called("test normal user");
        }
        private void ImagineProductionLogin(string user, string password, bool isAdmin)
        {
            Console.WriteLine("Logging in {0}. Admin = {1}", user, isAdmin);
            Assert.IsFalse(string.IsNullOrWhiteSpace(user));
            Assert.IsFalse(string.IsNullOrWhiteSpace(password));
        }

    }
}
