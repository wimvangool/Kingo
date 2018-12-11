using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MessageHandlerTestBase_Fails_IfExceptionIsExpectedButNoneIsThrown : MessageHandlerTestBaseTest<object>
    {        
        protected override object When() =>
            new object();

        protected override IMessageHandler<object> CreateMessageHandler() =>
            MessageHandlerDecorator<object>.Decorate((message, context) => { });

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public override async Task ThenAsync()
        {
            try
            {
                await ThenResult().IsExceptionOfTypeAsync<InternalServerErrorException>();
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("Expected exception of type 'InternalServerErrorException', but no exception was thrown.", exception.Message);
                throw;
            }
            finally
            {
                await base.ThenAsync();
            }
        }        
    }
}
