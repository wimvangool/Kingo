using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class UnitTestBase_Fails_IfExceptionIsExpectedButAssertCallbackFails : UnitTestBaseTest<object>
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
            var exceptionToThrow = new MetaAssertFailedException(string.Empty, null);

            try
            {
                await Result.IsExceptionOfTypeAsync<InternalServerErrorException>(e =>
                {
                    throw exceptionToThrow;
                });
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
            finally
            {
                await base.ThenAsync();
            }
        }        
    }
}
