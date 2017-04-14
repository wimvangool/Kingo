using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class WriteScenario_Fails_IfExceptionIsExpectedButDifferentTypeIsThrown : WriteScenarioTest<object>
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
            try
            {
                await Result.IsExceptionAsync<BadRequestException>();
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("Expected exception of type 'BadRequestException' but exception of type 'InternalServerErrorException' was thrown instead. See inner exception for details.", exception.Message);
                throw;
            }
        }        
    }
}
