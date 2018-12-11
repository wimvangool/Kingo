using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MessageHandlerTestBase_Succeeds_IfStreamIsCorrectlyVerified : MessageHandlerTestBaseTest<object>
    {
        private readonly object _event = new object();

        protected override object When() =>
            new object();

        protected override IMessageHandler<object> CreateMessageHandler()
        {
            return MessageHandlerDecorator<object>.Decorate((message, context) =>
            {
                context.EventBus.Publish(_event);
            });
        }

        [TestMethod]        
        public override async Task ThenAsync()
        {
            try
            {
                await ThenResult().IsEventStreamAsync(1, stream =>
                {
                    AssertEvent<object>(stream, 0, @event =>
                    {
                        Assert.AreSame(_event, @event);
                    });
                });
            }
            finally
            {
                await base.ThenAsync();
            }
        }        
    }
}
