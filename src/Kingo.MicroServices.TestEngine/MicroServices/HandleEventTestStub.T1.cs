using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class HandleEventTestStub<TMessage> : HandleMessageTestStub<TMessage>
    {
        private readonly IMessageHandler<TMessage> _messageHandler;
        private readonly TMessage _message;        

        public HandleEventTestStub(IMessageHandler<TMessage> messageHandler, TMessage message)
        {
            _messageHandler = messageHandler;
            _message = message;            
        }

        protected override Task WhenAsync(IMessageProcessor<TMessage> processor, MicroProcessorOperationTestContext context) =>
            processor.HandleEventAsync(_messageHandler, _message);        
    }
}
