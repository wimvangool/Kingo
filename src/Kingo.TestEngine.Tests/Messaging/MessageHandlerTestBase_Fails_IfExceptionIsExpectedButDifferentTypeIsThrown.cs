using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class MessageHandlerTestBase_Fails_IfExceptionIsExpectedButDifferentTypeIsThrown : MessageHandlerTestBaseTest<object>
    {                
        protected override object When() =>
            new object();

        protected override IMessageHandler<object> CreateMessageHandler()
        {
            return MessageHandlerDecorator<object>.Decorate((message, context) =>
            {
                throw new BusinessRuleException("Test");
            });
        }

        [TestMethod]        
        [ExpectedException(typeof(MetaAssertFailedException))]
        public override async Task ThenAsync()
        {
            try
            {
                await ThenResult().IsExceptionOfTypeAsync<BadRequestException>();
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("Expected exception of type 'BadRequestException' but exception of type 'InternalServerErrorException' was thrown instead. See inner exception for details.", exception.Message);
                throw;
            }
            finally
            {
                await base.ThenAsync();
            }
        }        
    }
}
