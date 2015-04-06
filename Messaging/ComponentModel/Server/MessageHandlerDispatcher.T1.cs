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
            _processor.Message.ThrowIfCancellationRequested();

            using (var scope = _processor.CreateUnitOfWorkScope())
            {
                HandleMessage(_message);

                scope.Complete();
            }
            _processor.Message.ThrowIfCancellationRequested();
        }

        private void HandleMessage(TMessage message)
        {
            if (_handler == null)
            {
                if (_processor.MessageHandlerFactory == null)
                {
                    return;
                }
                var source = _processor.Message.DetermineMessageSourceOf(message);

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
            _processor.Message.ThrowIfCancellationRequested();
        }
    }
}
