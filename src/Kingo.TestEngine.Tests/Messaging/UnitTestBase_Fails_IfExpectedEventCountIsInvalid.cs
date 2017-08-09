using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class UnitTestBase_Fails_IfExpectedEventCountIsInvalid : UnitTestBaseTest<object>
    {
        protected override object WhenMessageIsHandled() =>
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
