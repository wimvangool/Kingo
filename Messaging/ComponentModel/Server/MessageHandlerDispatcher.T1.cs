namespace System.ComponentModel.Server
{
    internal sealed class MessageHandlerDispatcher<TMessage> : IMessageHandler where TMessage : class, IMessage<TMessage>
    {       
        private readonly TMessage _message;
        private readonly IMessageHandler<TMessage> _handler;
        private readonly MessageProcessor _processor;

        internal MessageHandlerDispatcher(TMessage message, IMessageHandler<TMessage> handler, MessageProcessor processor)
        {
            _message = message;
            _handler = handler;
            _processor = processor;
        }

        public IMessage Message
        {
            get { return _message; }
        }

        public void Invoke()
        {
            _processor.MessagePointer.ThrowIfCancellationRequested();

            using (var scope = new UnitOfWorkScope(_processor.DomainEventBus))
            {
                HandleMessage(_message);

                scope.Complete();
            }
            _processor.MessagePointer.ThrowIfCancellationRequested();
        }

        private void HandleMessage(TMessage message)
        {
            if (_handler == null)
            {
                if (_processor.MessageHandlerFactory == null)
                {
                    return;
                }
                var source = _processor.MessagePointer.DetermineMessageSourceOf(message);

                foreach (var handler in _processor.MessageHandlerFactory.CreateMessageHandlersFor(message, source))
                {
                    HandleMessage(message, handler);
                }
            }
            else
            {
                HandleMessage(message, _handler);
            }
        }

        private void HandleMessage(TMessage message, IMessageHandler<TMessage> handler)
        {
            var messageHandler = new MessageHandlerWrapper<TMessage>(message, handler);

            _processor.BusinessLogicPipeline.ConnectTo(messageHandler).Invoke();                               
            _processor.MessagePointer.ThrowIfCancellationRequested();
        }
    }
}
