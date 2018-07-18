using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class UnitTestBase_Fails_IfExceptionIsExpectedButDifferentTypeIsThrown : UnitTestBaseTest<object>
    {                
        protected override object WhenMessageIsHandled() =>
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
                await Result.IsExceptionOfTypeAsync<BadRequestException>();
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
