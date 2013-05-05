using System;

namespace TestCaseSourceExtension
{
    public class TestCase
    {
        public TestCase(params object[] arguments)
        {
            Name = string.Empty;
            Arguments = arguments;
        }

        public string Name { get; private set; }
        public object[] Arguments { get; private set; }

        public TestCase Called(string humanReadableNameForTestCase)
        {
            Name = humanReadableNameForTestCase;
            return this;
        }

        override public string ToString()
        {
            return String.Format("{0} ( {1} )", Name, String.Join(",", Arguments) );
        }
    }
}