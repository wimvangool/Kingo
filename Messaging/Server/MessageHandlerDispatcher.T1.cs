namespace System.ComponentModel.Server
{
    internal sealed class MessageHandlerDispatcher<TMessage> : IMessageHandler where TMessage : class, IMessage<TMessage>
    {
        #region [====== Nested Types ======]

        private sealed class SelfValidatingMessage<T> : IMessage where T : class, IMessage<T>
        {
            private readonly T _message;
            private readonly IMessageValidator<T> _validator;

            internal SelfValidatingMessage(T message, IMessageValidator<T> validator)
            {
                _message = message;
                _validator = validator;
            }

            string IMessage.TypeId
            {
                get { return _message.TypeId; }
            }

            public IMessage Copy()
            {
                return _message.Copy();
            }

            public T Message
            {
                get { return _message; }
            }

            bool IMessage.TryGetValidationErrors(out InvalidMessageException exception)
            {
                ValidationErrorTree errorTree;

                if (TryGetValidationErrors(out errorTree) && errorTree.TryCreateInvalidMessageException(this, out exception))
                {
                    return true;
                }
                exception = null;
                return false;
            }

            bool IMessage.TryGetValidationErrors(out ValidationErrorTree errorTree)
            {
                return TryGetValidationErrors(out errorTree);
            }           

            private bool TryGetValidationErrors(out ValidationErrorTree errorTree)
            {
                if (_validator == null)
                {
                    return _message.TryGetValidationErrors(out errorTree);
                }
                return _validator.TryGetValidationErrors(_message, out errorTree);
            }
        }

        #endregion

        private readonly SelfValidatingMessage<TMessage> _message;
        private readonly IMessageHandler<TMessage> _handler;
        private readonly MessageProcessor _processor;

        internal MessageHandlerDispatcher(TMessage message, IMessageValidator<TMessage> validator, IMessageHandler<TMessage> handler, MessageProcessor processor)
        {
            _message = new SelfValidatingMessage<TMessage>(message, validator);
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
                HandleMessage(_message.Message);

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

            _processor.SpecificMessageHandlerPipeline.ConnectTo(messageHandler).Invoke();                               
            _processor.MessagePointer.ThrowIfCancellationRequested();
        }
    }
}
