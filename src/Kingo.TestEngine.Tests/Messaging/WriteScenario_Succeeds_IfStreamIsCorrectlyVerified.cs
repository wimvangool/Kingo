using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class WriteScenario_Succeeds_IfStreamIsCorrectlyVerified : WriteScenarioTest<object>
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
