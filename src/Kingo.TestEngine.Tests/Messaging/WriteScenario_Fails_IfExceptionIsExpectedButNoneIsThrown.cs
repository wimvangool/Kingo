using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class WriteScenario_Fails_IfExceptionIsExpectedButNoneIsThrown : WriteScenarioTest<object>
    {        
        protected override object When() =>
            new object();

        protected override IMessageHandler<object> CreateMessageHandler()
        {
            return MessageHandler<object>.FromDelegate((message, context) => { });
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public override async Task ThenAsync()
        {
            try
            {
                await Result.IsExceptionOfTypeAsync<InternalServerErrorException>();
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
