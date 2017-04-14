using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class WriteScenario_Fails_IfExceptionIsExpectedButAssertCallbackFails : WriteScenarioTest<object>
    {        
        protected override object When() =>
            new object();

        protected override IMessageHandler<object> CreateMessageHandler()
        {
            return MessageHandler<object>.FromDelegate((message, context) =>
            {
                throw new IllegalOperationException("Test");
            });
        }

        [TestMethod]        
        [ExpectedException(typeof(MetaAssertFailedException))]
        public override async Task ThenAsync()
        {
            var exceptionToThrow = new MetaAssertFailedException(string.Empty, null);

            try
            {
                await Result.IsExceptionAsync<InternalServerErrorException>(e =>
                {
                    throw exceptionToThrow;
                });
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
        }        
    }
}
