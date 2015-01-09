﻿using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            where TMessageIn : class, IMessage<TMessageIn>, IRequestMessage           
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
                _processor.MessagePointer.ThrowIfCancellationRequested();

                using (var scope = new UnitOfWorkScope(_processor.DomainEventBus))
                {
                    Result = Execute(message);

                    scope.Complete();
                }
                _processor.MessagePointer.ThrowIfCancellationRequested();
            }

            private TMessageOut Execute(TMessageIn message)
            {
                var result = _processor.CreateQueryPipeline(_query).Execute(message);

                _processor.MessagePointer.ThrowIfCancellationRequested();

                return result;
            }
        }

        #endregion

        private readonly IMessageProcessorBus _domainEventBus;
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
        public virtual IMessageProcessorBus DomainEventBus
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

        /// <inheritdoc />
        public override string ToString()
        {
            var processorString = new StringBuilder(GetType().Name);

            processorString.AppendFormat(" ({0}, {1})",
                ToString(MessageHandlerFactory),
                ToString(MessagePointer));
            
            return processorString.ToString();
        }

        private static string ToString(MessageHandlerFactory factory)
        {
            return string.Format("{0} MessageHandler(s) Registered", factory == null ? 0 : factory.MessageHandlerCount);
        }

        private static string ToString(MessagePointer pointer)
        {
            if (pointer == null)
            {
                return "Idle on " + Thread.CurrentThread.Name;
            }
            if (pointer.PointsToA(typeof(IRequestMessage)))
            {
                return string.Format("Executing {0} on {1}",
                    pointer.Message.GetType().Name,
                    Thread.CurrentThread.Name);
            }
            return string.Format("Handling {0} on {1}",
                    pointer.Message.GetType().Name,
                    Thread.CurrentThread.Name);
        }

        #region [====== Commands ======]

        /// <inheritdoc />      
        public Task ExecuteAsync<TCommand>(TCommand message) where TCommand : class, IMessage<TCommand>, IRequestMessage
        {
            return ExecuteAsync(message, NullHandler<TCommand>(), null);
        }

        /// <inheritdoc />              
        public Task ExecuteAsync<TCommand>(TCommand message, CancellationToken? token) where TCommand : class, IMessage<TCommand>, IRequestMessage
        {
            return ExecuteAsync(message, NullHandler<TCommand>(), token);
        }

        /// <inheritdoc />        
        public Task ExecuteAsync<TCommand>(TCommand message, Action<TCommand> handler) where TCommand : class, IMessage<TCommand>, IRequestMessage
        {
            return ExecuteAsync(message, (ActionDecorator<TCommand>) handler, null);
        }

        /// <inheritdoc />                
        public Task ExecuteAsync<TCommand>(TCommand message, Action<TCommand> handler, CancellationToken? token) where TCommand : class, IMessage<TCommand>, IRequestMessage
        {
            return ExecuteAsync(message, (ActionDecorator<TCommand>) handler, token);
        }

        /// <inheritdoc />        
        public Task ExecuteAsync<TCommand>(TCommand message, IMessageHandler<TCommand> handler) where TCommand : class, IMessage<TCommand>, IRequestMessage
        {
            return ExecuteAsync(message, handler, null);
        }

        /// <inheritdoc />              
        public Task ExecuteAsync<TCommand>(TCommand message, IMessageHandler<TCommand> handler, CancellationToken? token) where TCommand : class, IMessage<TCommand>, IRequestMessage
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return Start(() => Execute(message, handler, token), message.GetType(), token);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message) where TCommand : class, IMessage<TCommand>, IRequestMessage
        {
            Handle(message, new RequestMessageValidator<TCommand>());
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, CancellationToken? token) where TCommand : class, IMessage<TCommand>, IRequestMessage
        {
            Handle(message, new RequestMessageValidator<TCommand>(), token);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, Action<TCommand> handler) where TCommand : class, IMessage<TCommand>, IRequestMessage
        {
            Handle(message, new RequestMessageValidator<TCommand>(), handler);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, Action<TCommand> handler, CancellationToken? token) where TCommand : class, IMessage<TCommand>, IRequestMessage
        {
            Handle(message, new RequestMessageValidator<TCommand>(), handler, token);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, IMessageHandler<TCommand> handler) where TCommand : class, IMessage<TCommand>, IRequestMessage
        {
            Handle(message, new RequestMessageValidator<TCommand>(), handler);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, IMessageHandler<TCommand> handler, CancellationToken? token) where TCommand : class, IMessage<TCommand>, IRequestMessage
        {
            Handle(message, new RequestMessageValidator<TCommand>(), handler, token);
        }

        #endregion

        #region [====== Queries ======]

        /// <inheritdoc />       
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>, IRequestMessage          
        {
            return ExecuteAsync(message, (FuncDecorator<TMessageIn, TMessageOut>) query, null);
        }

        /// <inheritdoc />       
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query, CancellationToken? token)
            where TMessageIn : class, IMessage<TMessageIn>, IRequestMessage           
        {
            return ExecuteAsync(message, (FuncDecorator<TMessageIn, TMessageOut>) query, token);
        }

        /// <inheritdoc />     
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>, IRequestMessage           
        {
            return ExecuteAsync(message, query, null);
        }

        /// <inheritdoc />     
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, CancellationToken? token)
            where TMessageIn : class, IMessage<TMessageIn>, IRequestMessage           
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return Start(() => Execute(message, query, token), message.GetType(), token);
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>, IRequestMessage           
        {
            return Execute(message, (FuncDecorator<TMessageIn, TMessageOut>) query, null);
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query, CancellationToken? token)
            where TMessageIn : class, IMessage<TMessageIn>, IRequestMessage           
        {
            return Execute(message, (FuncDecorator<TMessageIn, TMessageOut>) query, token);
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>, IRequestMessage           
        {
            return Execute(message, query, null);
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, CancellationToken? token)
            where TMessageIn : class, IMessage<TMessageIn>, IRequestMessage          
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            PushMessage(ref message, token);

            try
            {
                var pipeline = new QueryDispatcherPipeline<TMessageIn, TMessageOut>(query, this);

                CreatePerMessagePipeline(pipeline, new RequestMessageValidator<TMessageIn>()).Handle(message);

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
        public Task HandleAsync<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, null, NullHandler<TMessage>(), null);
        }

        /// <inheritdoc />       
        public Task HandleAsync<TMessage>(TMessage message, IMessageValidator<TMessage> validator) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, validator, NullHandler<TMessage>(), null);
        }

        /// <inheritdoc />               
        public Task HandleAsync<TMessage>(TMessage message, IMessageValidator<TMessage> validator, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, validator, NullHandler<TMessage>(), token);
        }

        /// <inheritdoc />      
        public Task HandleAsync<TMessage>(TMessage message, IMessageValidator<TMessage> validator, Action<TMessage> handler) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, validator, (ActionDecorator<TMessage>) handler, null);
        }

        /// <inheritdoc />                
        public Task HandleAsync<TMessage>(TMessage message, IMessageValidator<TMessage> validator, Action<TMessage> handler, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, validator, (ActionDecorator<TMessage>) handler, token);
        }

        /// <inheritdoc />        
        public Task HandleAsync<TMessage>(TMessage message, IMessageValidator<TMessage> validator, IMessageHandler<TMessage> handler) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, validator, handler, null);
        }

        /// <inheritdoc />                
        public Task HandleAsync<TMessage>(TMessage message, IMessageValidator<TMessage> validator, IMessageHandler<TMessage> handler, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return Start(() => Handle(message, validator, handler, token), message.GetType(), token);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, null, NullHandler<TMessage>(), null);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, validator, NullHandler<TMessage>(), null);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, validator, NullHandler<TMessage>(), token);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, Action<TMessage> handler) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, validator, (ActionDecorator<TMessage>) handler, null);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, Action<TMessage> handler, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, validator, (ActionDecorator<TMessage>) handler, token);             
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

        #region [====== Pipeline Factories ======]

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
            return new MessageHandlerPipelineFactory<TMessage>()
            {
                h => new ThreadNamePipeline<TMessage>(h),
                h => new MessageValidationPipeline<TMessage>(h, validator)
            }.CreateMessageHandlerPipeline(handler); 
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
            where TMessageIn : class, IMessage<TMessageIn>, IRequestMessage           
        {
            return query;
        }        

        private static IMessageHandler<TMessage> NullHandler<TMessage>() where TMessage : class
        {
            return null;
        }

        #endregion

        #region [====== Task Factories ======]

        /// <summary>
        /// Creates, starts and returns a new <see cref="Task" /> that is used to execute this command.
        /// </summary>
        /// <param name="command">The action that will be invoked on the background thread.</param>
        /// <param name="messageType">Type of the message that is being handled.</param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>The newly created task.</returns>
        /// <remarks>
        /// The default implementation uses the <see cref="TaskFactory.StartNew(Action)">StartNew</see>-method
        /// to start and return a new <see cref="Task" />. You may want to override this method to specify
        /// more options when creating this task.
        /// </remarks>
        protected virtual Task Start(Action command, Type messageType, CancellationToken? token)
        {
            return token.HasValue
                ? Task.Factory.StartNew(command, token.Value)
                : Task.Factory.StartNew(command);
        }

        /// <summary>
        /// Creates, starts and returns a new <see cref="Task{T}" /> that is used to execute this query.
        /// </summary>
        /// <param name="query">The action that will be invoked on the background thread.</param>
        /// <param name="messageType">Type of the message that is being handled.</param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>The newly created task.</returns>
        /// <remarks>
        /// The default implementation uses the <see cref="TaskFactory{T}.StartNew(Func{T})">StartNew</see>-method
        /// to start and return a new <see cref="Task{T}" />. You may want to override this method to specify
        /// more options when creating this task.
        /// </remarks>
        protected virtual Task<TMessageOut> Start<TMessageOut>(Func<TMessageOut> query, Type messageType, CancellationToken? token)
        {
            return token.HasValue
                ? Task<TMessageOut>.Factory.StartNew(query, token.Value)
                : Task<TMessageOut>.Factory.StartNew(query);
        }

        #endregion
    }
}
