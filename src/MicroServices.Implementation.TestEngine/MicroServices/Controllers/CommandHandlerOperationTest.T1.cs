using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class CommandHandlerOperationTest<TMessage> : MessageHandlerOperationTest<TMessage>
    {
        private readonly IMessageHandler<TMessage> _messageHandler;
        private readonly TMessage _message;        

        public CommandHandlerOperationTest(MessageHandlerOperation<TMessage> operation, IMessageHandler<TMessage> messageHandler, TMessage message) :
            base(operation)
        {
            _messageHandler = messageHandler;
            _message = message;          
        }

        protected override Task WhenAsync(IMessageHandlerOperationRunner<TMessage> runner, MicroProcessorOperationTestContext context) =>
            runner.ExecuteCommandAsync(_messageHandler, _message);

        protected override void Then(TMessage message, IMessageHandlerOperationTestResult result, MicroProcessorOperationTestContext context) =>
            result.IsMessageStream();
    }
}
