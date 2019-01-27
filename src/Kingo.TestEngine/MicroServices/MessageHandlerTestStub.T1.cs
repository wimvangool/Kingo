using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerTestStub<TMessage> : MessageHandlerTest<TMessage>
    {
        private readonly TMessage _message;
        private readonly IMessageHandler<TMessage> _handler;

        public MessageHandlerTestStub(TMessage message, Func<TMessage, MessageHandlerContext, Task> handler) :
            this(message, MessageHandlerDecorator<TMessage>.Decorate(handler)) { }

        public MessageHandlerTestStub(TMessage message, IMessageHandler<TMessage> handler)
        {
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }
            _message = message;
            _handler = handler;
        }

        protected override Task WhenAsync(IMessageProcessor<TMessage> processor, MicroProcessorTestContext context) =>
            processor.HandleAsync(_message, _handler);

        protected override void Then(TMessage message, IMessageHandlerResult result, MicroProcessorTestContext context) =>
            result.IsEventStream();
    }
}
