using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MicroProcessorTestRunnerTest : MicroProcessorTestRunner
    {
        [AssemblyInitialize]
        public static void SetupAssembly(TestContext context)
        {
            MicroProcessor.Add(processor =>
            {

            }).Configure(services =>
            {

            });
        }
    }
}
