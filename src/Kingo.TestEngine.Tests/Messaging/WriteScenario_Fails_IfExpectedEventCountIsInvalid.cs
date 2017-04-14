using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class WriteScenario_Fails_IfExpectedEventCountIsInvalid : WriteScenarioTest<object>
    {
        protected override object When() =>
            new object();

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public override async Task ThenAsync()
        {
            try
            {
                await Result.IsEventStreamAsync(-1);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("The number of expected events cannot be negative: -1."));
                throw;
            }
        }        
    }
}
