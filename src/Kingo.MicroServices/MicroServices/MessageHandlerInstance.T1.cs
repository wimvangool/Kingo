using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerInstance<TMessage> : MessageHandler, IMessageHandler<TMessage>
    {        
        private readonly IMessageHandler<TMessage> _messageHandler;
        private readonly IMessageHandlerConfiguration _configuration;

        public MessageHandlerInstance(Action<TMessage, MessageHandlerOperationContext> messageHandler) :
            this(MessageHandlerDecorator<TMessage>.Decorate(messageHandler)) { }

        public MessageHandlerInstance(Func<TMessage, MessageHandlerOperationContext, Task> messageHandler) :
            this(MessageHandlerDecorator<TMessage>.Decorate(messageHandler)) { }

        private MessageHandlerInstance(IMessageHandler<TMessage> messageHandler, IMessageHandlerConfiguration configuration = null) :
            base(MessageHandlerType.FromInstance(messageHandler), MessageHandlerInterface.FromType<TMessage>())
        {
            _messageHandler = messageHandler;
            _configuration = configuration;
        }

        public MessageHandlerInstance<TMessage> WithConfiguration(bool handlesExternalMessages, bool handlesInternalMessages) =>
            WithConfiguration(MessageHandlerAttribute.Create(handlesExternalMessages, handlesInternalMessages));

        public MessageHandlerInstance<TMessage> WithConfiguration(IMessageHandlerConfiguration configuration) =>
            new MessageHandlerInstance<TMessage>(_messageHandler, configuration);

        public override bool HandlesExternalMessages =>
            _configuration?.HandlesExternalMessages ?? base.HandlesExternalMessages;

        public override bool HandlesInternalMessages =>
            _configuration?.HandlesInternalMessages ?? base.HandlesInternalMessages;

        public override bool Equals(MicroProcessorComponent other) =>
            Equals(other as MessageHandlerInstance<TMessage>);

        public bool Equals(MessageHandlerInstance<TMessage> other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return
                _messageHandler.Equals(other._messageHandler) &&
                Equals(_configuration, other._configuration);
        }

        private static bool Equals(IMessageHandlerConfiguration left, IMessageHandlerConfiguration right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }
            return
                left.HandlesExternalMessages == right.HandlesExternalMessages &&
                left.HandlesInternalMessages == right.HandlesInternalMessages;
        }

        public Task HandleAsync(TMessage message, MessageHandlerOperationContext context) =>
            _messageHandler.HandleAsync(message, context);

        internal override object ResolveMessageHandler(IServiceProvider serviceProvider) =>
            _messageHandler;        
    }
}
