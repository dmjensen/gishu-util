using System;

namespace TestCaseSourceExtension
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TestCaseSourceAttribute : Attribute
    {
        public TestCaseSourceAttribute(string source)
        {
            Source = source;
        }

        public string Source { get; private set; }
    }
}