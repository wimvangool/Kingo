using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class UnitTestBase_Succeeds_IfStreamIsCorrectlyVerified : UnitTestBaseTest<object>
    {
        private readonly object _event = new object();

        protected override object WhenMessageIsHandled() =>
            new object();

        protected override IMessageHandler<object> CreateMessageHandler()
        {
            return MessageHandlerDecorator<object>.Decorate((message, context) =>
            {
                context.OutputStream.Publish(_event);
            });
        }

        [TestMethod]        
        public override async Task ThenAsync()
        {
            try
            {
                await Result.IsEventStreamAsync(1, stream =>
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
