namespace System.ComponentModel.Server.Modules
{
    internal sealed class MessageDispatcherModule<TMessage> : MessageHandlerPipelineModule<TMessage> where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _handler;
        private readonly MessageProcessor _processor;

        internal MessageDispatcherModule(IMessageHandler<TMessage> handler, MessageProcessor processor)
        {
            _handler = handler;
            _processor = processor;
        }

        protected override IMessageHandler<TMessage> Handler
        {
            get { return _handler; }
        }

        protected override void Handle(TMessage message)
        {
            _processor.MessagePointer.ThrowIfCancellationRequested();

            using (var scope = new UnitOfWorkScope(_processor.DomainEventBus))
            {
                HandleMessage(message);

                scope.Complete();
            }
            _processor.MessagePointer.ThrowIfCancellationRequested();
        }

        private void HandleMessage(TMessage message)
        {
            if (Handler == null)
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
            _processor.CreatePerMessageHandlerPipeline<TMessage>().CreateMessageHandlerPipeline(handler).Handle(message);
            _processor.MessagePointer.ThrowIfCancellationRequested();
        }
    }
}
