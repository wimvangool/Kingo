using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class WriteScenario_Fails_IfAssertEventCallbackFails : WriteScenarioTest<object>
    {
        private readonly object _event = new object();

        protected override object When() =>
            new object();

        protected override IMessageHandler<object> CreateMessageHandler()
        {
            return MessageHandler<object>.FromDelegate((message, context) =>
            {
                context.OutputStream.Publish(_event);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public override async Task ThenAsync()
        {
            var exceptionToThrow = new MetaAssertFailedException(string.Empty, null);

            try
            {
                await Result.IsEventStreamAsync(1, stream =>
                {
                    AssertEvent<object>(stream, 0, @event =>
                    {
                        throw exceptionToThrow;
                    });
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
