using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class EventHandlerOperationTestStub<TMessage> : MessageHandlerOperationTestStub<TMessage>
    {
        private readonly IMessageHandler<TMessage> _messageHandler;
        private readonly TMessage _message;        

        public EventHandlerOperationTestStub(IMessageHandler<TMessage> messageHandler, TMessage message)
        {
            _messageHandler = messageHandler;
            _message = message;            
        }

        protected override Task WhenAsync(IMessageProcessor<TMessage> processor, MicroProcessorOperationTestContext context) =>
            processor.HandleEventAsync(_messageHandler, _message);        
    }
}
