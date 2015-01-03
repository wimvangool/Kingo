using System.Threading;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a handler of arbitrary messages.
    /// </summary>    
    public abstract class MessageProcessor : IMessageProcessor
    {        
        #region [====== MessageDispatcherPipeline ======]

        private sealed class MessageDispatcherPipeline<TMessage> : IMessageHandler<TMessage> where TMessage : class
        {            
            private readonly IMessageHandler<TMessage> _handler;
            private readonly MessageProcessor _processor;

            internal MessageDispatcherPipeline(IMessageHandler<TMessage> handler, MessageProcessor processor)
            {                
                _handler = handler;
                _processor = processor;
            } 
           
            public void Handle(TMessage message)
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
                if (_handler == null)
                {
                    if (_processor.MessageHandlerFactory == null)
                    {
                        return;
                    }
                    foreach (var handler in _processor.MessageHandlerFactory.CreateMessageHandlersFor(message))
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
                _processor.CreatePerMessageHandlerPipeline(handler).Handle(message);
                _processor.MessagePointer.ThrowIfCancellationRequested();
            }
        }

        #endregion        

        #region [====== QueryDispatcherPipeline ======]

        internal sealed class QueryDispatcherPipeline<TMessageIn, TMessageOut> : IMessageHandler<TMessageIn>
            where TMessageIn : class, IRequestMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            private readonly IQuery<TMessageIn, TMessageOut> _query;
            private readonly MessageProcessor _processor;

            internal QueryDispatcherPipeline(IQuery<TMessageIn, TMessageOut> query, MessageProcessor processor)
            {
                if (query == null)
                {
                    throw new ArgumentNullException("query");
                }
                _query = query;
                _processor = processor;
            }

            internal TMessageOut Result
            {
                get;
                private set;
            }

            void IMessageHandler<TMessageIn>.Handle(TMessageIn message)
            {
                using (var scope = new UnitOfWorkScope(_processor.DomainEventBus))
                {
                    Result = Execute(message);

                    scope.Complete();
                }                
            }

            private TMessageOut Execute(TMessageIn message)
            {
                return _processor.CreateQueryPipeline(_query).Execute(message);
            }
        }

        #endregion

        private readonly IMessageProcessorBus _domainEventBus;                
        private readonly ThreadLocal<MessagePointer> _currentMessagePointer;                            

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>        
        protected MessageProcessor()
        {
            _domainEventBus = new MessageProcessorBus(this);
            _currentMessagePointer = new ThreadLocal<MessagePointer>();
        }        

        /// <inheritdoc />
        public IMessageProcessorBus DomainEventBus
        {
            get { return _domainEventBus; }
        }

        /// <inheritdoc />
        public MessagePointer MessagePointer
        {
            get { return _currentMessagePointer.Value; }
            private set { _currentMessagePointer.Value = value; }
        }

        /// <summary>
        /// Returns the <see cref="MessageHandlerFactory" /> of this processor.
        /// </summary>
        protected abstract MessageHandlerFactory MessageHandlerFactory
        {
            get;
        }

        #region [====== Commands ======]

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message) where TCommand : class, IRequestMessage<TCommand>
        {
            Handle(message, message);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, CancellationToken? token) where TCommand : class, IRequestMessage<TCommand>
        {
            Handle(message, message, token);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, Action<TCommand> handler) where TCommand : class, IRequestMessage<TCommand>
        {
            Handle(message, message, handler);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, Action<TCommand> handler, CancellationToken? token) where TCommand : class, IRequestMessage<TCommand>
        {
            Handle(message, message, handler, token);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, IMessageHandler<TCommand> handler) where TCommand : class, IRequestMessage<TCommand>
        {
            Handle(message, message, handler);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, IMessageHandler<TCommand> handler, CancellationToken? token) where TCommand : class, IRequestMessage<TCommand>
        {
            Handle(message, message, handler, token);
        }

        #endregion

        #region [====== Queries ======]

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IRequestMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return Execute(message, new FuncDecorator<TMessageIn, TMessageOut>(query));
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IRequestMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            PushMessage(ref message, null);

            try
            {
                var pipeline = new QueryDispatcherPipeline<TMessageIn, TMessageOut>(query, this);

                CreatePerMessagePipeline(pipeline, message).Handle(message);

                return pipeline.Result;
            }
            finally
            {
                PopMessage();
            }              
        }

        #endregion

        #region [====== Events ======]

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, null, NoMessageHandler<TMessage>(), null);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, validator, NoMessageHandler<TMessage>(), null);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, validator, NoMessageHandler<TMessage>(), token);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, Action<TMessage> handler) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, validator, ToMessageHandler(handler), null);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, Action<TMessage> handler, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, validator, ToMessageHandler(handler), token);             
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, IMessageHandler<TMessage> handler) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, validator, handler, null);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, IMessageHandler<TMessage> handler, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            PushMessage(ref message, token);

            try
            {
                handler = new MessageDispatcherPipeline<TMessage>(handler, this);
                handler = CreatePerMessagePipeline(handler, validator);
                handler.Handle(message);
            }
            finally
            {
                PopMessage();
            }                   
        }

        #endregion

        #region [====== Pipelines ======]

        private void PushMessage<TMessage>(ref TMessage message, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {                                    
            MessagePointer = MessagePointer == null ?
                new MessagePointer(message = message.Copy(), token) :
                MessagePointer.CreateChildPointer(message = message.Copy(), token);                        
        }

        private void PopMessage()
        {
            MessagePointer = MessagePointer.ParentPointer;
        }        

        /// <summary>
        /// Creates and returns a <see cref="IMessageHandler{TMessage}" /> pipeline on top of all handlers that will
        /// be invoked for a specific message.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <param name="handler">The handler to decorate.</param>
        /// <param name="validator">Optional validator of the message.</param>
        /// <returns>A pipeline that will handle a message.</returns>        
        protected virtual IMessageHandler<TMessage> CreatePerMessagePipeline<TMessage>(IMessageHandler<TMessage> handler, IMessageValidator<TMessage> validator) where TMessage : class
        {
            return new MessageValidationPipeline<TMessage>(handler, validator);
        }

        /// <summary>
        /// Creates and returns a <see cref="IMessageHandler{TMessage}" /> pipeline on top of each handler that will be
        /// invoked for a specific message.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <param name="handler">The handler to decorate.</param>
        /// <returns>A pipeline that will handle a message.</returns>
        /// <remarks>
        /// The default implementation simply returns the specified <paramref name="handler"/>.
        /// </remarks>
        protected virtual IMessageHandler<TMessage> CreatePerMessageHandlerPipeline<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class
        {
            return handler;
        }

        /// <summary>
        /// Creates and returns a <see cref="IQuery{TMessageIn, TMessageOut}"/> pipeline on top of each query that will be executed.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>       
        /// <param name="query">The query to execute.</param>
        /// <returns>A query pipeline.</returns>
        /// <remarks>
        /// The default implementation simply returns the specified <paramref name="query"/>.
        /// </remarks>
        protected virtual IQuery<TMessageIn, TMessageOut> CreateQueryPipeline<TMessageIn, TMessageOut>(IQuery<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IRequestMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return query;
        }

        private static IMessageHandler<TMessage> ToMessageHandler<TMessage>(Action<TMessage> handler) where TMessage : class
        {
            return handler == null ? null : new ActionDecorator<TMessage>(handler);
        }

        private static IMessageHandler<TMessage> NoMessageHandler<TMessage>() where TMessage : class
        {
            return null;
        }

        #endregion
    }
}
