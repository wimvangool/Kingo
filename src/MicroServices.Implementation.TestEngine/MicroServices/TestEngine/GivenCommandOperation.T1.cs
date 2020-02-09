using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class GivenCommandOperation<TMessage> : GivenMessageOperation<TMessage>
    {
        private readonly IMessageHandler<TMessage> _messageHandler;

        public GivenCommandOperation(GivenMessageOperation<TMessage> operation, IMessageHandler<TMessage> messageHandler) :
            base(operation)
        {
            _messageHandler = messageHandler;
        }

        public GivenCommandOperation(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator, IMessageHandler<TMessage> messageHandler) :
            base(configurator)
        {
            _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        }

        public override Task RunAsync(MicroProcessorTestRunner runner, MicroProcessorTestContext context) =>
            throw new NotImplementedException();
    }
}
