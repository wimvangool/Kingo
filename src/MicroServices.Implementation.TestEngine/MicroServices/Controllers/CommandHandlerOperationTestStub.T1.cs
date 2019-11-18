using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class CommandHandlerOperationTestStub<TMessage> : MessageHandlerOperationTestStub<TMessage>
    {
        private readonly IMessageHandler<TMessage> _messageHandler;
        private readonly TMessage _message;        

        public CommandHandlerOperationTestStub(IMessageHandler<TMessage> messageHandler, TMessage message)
        {
            _messageHandler = messageHandler;
            _message = message;            
        }

        protected override Task WhenAsync(IMessageProcessor<TMessage> processor, MicroProcessorOperationTestContext context) =>
            processor.ExecuteCommandAsync(_messageHandler, _message);        
    }
}
