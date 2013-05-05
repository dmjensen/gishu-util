using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCaseSourceExtension
{
    public class ParamTestExecution : TestExtensionExecution
    {
        public override void Initialize(TestExecution execution)
        {}

        public override ITestMethodInvoker CreateTestMethodInvoker(TestMethodInvokerContext context)
        {
            return new ParamTestInvoker(context);
        }

        public override void Dispose()
        {}
    }

    
}