using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class GivenEventOperation<TMessage> : GivenMessageOperation<TMessage>
    {
        private readonly IMessageHandler<TMessage> _messageHandler;

        public GivenEventOperation(GivenMessageOperation<TMessage> operation, IMessageHandler<TMessage> messageHandler) :
            base(operation)
        {
            _messageHandler = messageHandler;
        }

        public GivenEventOperation(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator, IMessageHandler<TMessage> messageHandler) :
            base(configurator)
        {
            _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        }

        public override Task RunAsync(MicroProcessorTestRunner runner, MicroProcessorTestContext context) =>
            throw new NotImplementedException();
    }
}
