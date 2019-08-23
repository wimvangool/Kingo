using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerInstance<TMessage> : MessageHandler, IMessageHandler<TMessage>
    {        
        private readonly IMessageHandler<TMessage> _messageHandler;        

        public MessageHandlerInstance(Action<TMessage, MessageHandlerOperationContext> messageHandler) :
            this(MessageHandlerDecorator<TMessage>.Decorate(messageHandler)) { }

        public MessageHandlerInstance(Func<TMessage, MessageHandlerOperationContext, Task> messageHandler) :
            this(MessageHandlerDecorator<TMessage>.Decorate(messageHandler)) { }

        private MessageHandlerInstance(IMessageHandler<TMessage> messageHandler) :
            base(MessageHandlerType.FromInstance(messageHandler), MessageHandlerInterface.FromType<TMessage>())
        {
            _messageHandler = messageHandler;            
        }

        public override bool HandlesExternalMessages =>
            false;

        public override bool HandlesInternalMessages =>
            true;

        public Task HandleAsync(TMessage message, MessageHandlerOperationContext context) =>
            _messageHandler.HandleAsync(message, context);

        internal override object ResolveMessageHandler(IServiceProvider serviceProvider) =>
            _messageHandler;

        #region [====== Equals, GetHashCode & ToString ======]

        public override bool Equals(MicroProcessorComponent other) =>
            Equals(other as MessageHandlerInstance<TMessage>);

        public bool Equals(MessageHandlerInstance<TMessage> other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return _messageHandler.Equals(other._messageHandler);          
        }

        public override int GetHashCode() =>
            _messageHandler.GetHashCode();

        public override string ToString() =>
            $"{_messageHandler} ({MessageHandlerAttribute.ToString(this)})";

        #endregion
    }
}
