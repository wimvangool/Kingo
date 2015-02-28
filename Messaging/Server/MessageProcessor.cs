using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a handler of arbitrary messages.
    /// </summary>    
    public class MessageProcessor : IMessageProcessor
    {        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ThreadLocal<MessagePointer> _currentMessagePointer;                 
        private readonly IMessageProcessorBus _domainEventBus;

        private readonly Lazy<MessageHandlerPipeline> _genericMessageHandlerPipeline;
        private readonly Lazy<MessageHandlerPipeline> _specificMessageHandlerPipeline;
        private readonly Lazy<QueryPipeline> _queryPipeline;               

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>        
        protected MessageProcessor()
        {           
            _currentMessagePointer = new ThreadLocal<MessagePointer>();
            _domainEventBus = new MessageProcessorBus(this);
            
            _genericMessageHandlerPipeline = new Lazy<MessageHandlerPipeline>(CreateGenericMessageHandlerPipeline, true);
            _specificMessageHandlerPipeline = new Lazy<MessageHandlerPipeline>(CreateSpecificMessageHandlerPipeline, true);
            _queryPipeline = new Lazy<QueryPipeline>(CreateQueryPipeline, true);
        }

        internal MessageHandlerPipeline GenericMessageHandlerPipeline
        {
            get { return _genericMessageHandlerPipeline.Value; }
        }

        internal MessageHandlerPipeline SpecificMessageHandlerPipeline
        {
            get { return _specificMessageHandlerPipeline.Value; }
        }

        internal QueryPipeline QueryPipeline
        {
            get { return _queryPipeline.Value; }
        }

        #region [====== Dispose ======]

        /// <summary>
        /// Indicates whether not this instance has been disposed.
        /// </summary>
        protected bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if the method was called by the application explicitly (<c>true</c>), or by the finalizer
        /// (<c>false</c>).
        /// </param>
        /// <remarks>
        /// If <paramref name="disposing"/> is <c>true</c>, this method will dispose any managed resources immediately.
        /// Otherwise, only unmanaged resources will be released.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                if (_genericMessageHandlerPipeline.IsValueCreated)
                {
                    _genericMessageHandlerPipeline.Value.Dispose();
                }
                if (_specificMessageHandlerPipeline.IsValueCreated)
                {
                    _specificMessageHandlerPipeline.Value.Dispose();
                }
                if (_queryPipeline.IsValueCreated)
                {
                    _queryPipeline.Value.Dispose();
                }
            }
            IsDisposed = true;
        }

        /// <summary>
        /// Creates and returns a new <see cref="ObjectDisposedException" />.
        /// </summary>
        /// <returns>A new <see cref="ObjectDisposedException" />.</returns>
        protected ObjectDisposedException NewObjectDisposedException()
        {
            return new ObjectDisposedException(GetType().Name);
        }

        #endregion

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
        protected internal virtual MessageHandlerFactory MessageHandlerFactory
        {
            get { return null; }
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
            return string.Format("Handling {0} on {1}",
                    pointer.Message.GetType().Name,
                    Thread.CurrentThread.Name);
        }

        #region [====== Commands & Events ======]

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, IMessageValidator<TMessage> validator = null, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, NullHandler<TMessage>(), validator, token);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, Action<TMessage> handler, IMessageValidator<TMessage> validator = null, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, (ActionDecorator<TMessage>) handler, validator, token);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler, IMessageValidator<TMessage> validator = null, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var callerContext = SynchronizationContext.Current;

            return Start(() =>
            {
                var previousContext = SynchronizationContext.Current;

                SynchronizationContext.SetSynchronizationContext(callerContext);

                try
                {
                    Handle(message, handler, validator, token);
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(previousContext);
                }                
            }, message.GetType(), token);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator = null, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, NullHandler<TMessage>(), validator, token);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, Action<TMessage> handler, IMessageValidator<TMessage> validator = null, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, (ActionDecorator<TMessage>) handler, validator, token);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageHandler<TMessage> handler, IMessageValidator<TMessage> validator = null, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            PushMessage(ref message, token);

            try
            {
                var dispatcher = new MessageHandlerDispatcher<TMessage>(message, validator, handler, this);
                var pipeline = GenericMessageHandlerPipeline.ConnectTo(dispatcher);

                pipeline.Invoke();              
            }
            finally
            {
                PopMessage();
            }
        }

        #endregion      
  
        #region [====== Queries ======]

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query, IMessageValidator<TMessageIn> validator = null, QueryExecutionOptions options = QueryExecutionOptions.Default, CancellationToken? token = null)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return ExecuteAsync(message, (FuncDecorator<TMessageIn, TMessageOut>) query, validator, options, token);
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, IMessageValidator<TMessageIn> validator = null, QueryExecutionOptions options = QueryExecutionOptions.Default, CancellationToken? token = null)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var callerContext = SynchronizationContext.Current;

            return Start(() =>
            {
                var previousContext = SynchronizationContext.Current;

                SynchronizationContext.SetSynchronizationContext(callerContext);

                try
                {
                    return Execute(message, query, validator, options, token);
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(previousContext);
                }                
            }, message.GetType(), token);
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query, IMessageValidator<TMessageIn> validator = null, QueryExecutionOptions options = QueryExecutionOptions.Default, CancellationToken? token = null)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return Execute(message, (FuncDecorator<TMessageIn, TMessageOut>) query, validator, options, token);
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, IMessageValidator<TMessageIn> validator = null, QueryExecutionOptions options = QueryExecutionOptions.Default, CancellationToken? token = null)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            PushMessage(ref message, token);

            try
            {
                var handler = new QueryDispatcherModule<TMessageIn, TMessageOut>(message, query, options, this);
                
                GenericMessageHandlerPipeline.ConnectTo(handler).Invoke();

                return handler.MessageOut;
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

        private MessageHandlerPipeline CreateGenericMessageHandlerPipeline()
        {
            return new MessageHandlerPipeline(CreateGenericMessageHandlerModules()); ;
        }

        /// <summary>
        /// Creates and returns a collection of <see cref="IMessageHandlerModule">modules</see>
        /// that will be used to create a pipeline for every incoming message.
        /// </summary>                
        /// <returns>A collection of <see cref="IMessageHandlerModule">modules</see>.</returns>              
        protected virtual IEnumerable<IMessageHandlerModule> CreateGenericMessageHandlerModules()            
        {
            return new IMessageHandlerModule[]
            {
                new MessageValidationModule(),
                new TransactionScopeModule(), 
            };
        }

        private MessageHandlerPipeline CreateSpecificMessageHandlerPipeline()
        {
            return new MessageHandlerPipeline(CreateSpecificMessageHandlerModules());
        }

        /// <summary>
        /// Creates and returns a collection of <see cref="IMessageHandlerModule">modules</see> that will be used to
        /// create a pipeline for every <see cref="IMessageHandler{TMessage}" /> handling a certain message.
        /// </summary>               
        /// <returns>A pipeline that will handle a message.</returns>              
        protected internal virtual IEnumerable<IMessageHandlerModule> CreateSpecificMessageHandlerModules()
        {
            return Enumerable.Empty<IMessageHandlerModule>();
        }

        private QueryPipeline CreateQueryPipeline()
        {
            return new QueryPipeline(CreateQueryModules());
        }

        /// <summary>
        /// Creates and returns a collection of <see cref="IQueryModule">modules</see> that will be used to
        /// create a pipeline for every query that is executed.
        /// </summary>                     
        /// <returns>A query pipeline.</returns>             
        protected virtual IEnumerable<IQueryModule> CreateQueryModules()            
        {
            return Enumerable.Empty<IQueryModule>();
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

        #region [====== TransactionQueue ======]

        /// <summary>
        /// Invokes the specified <paramref name="action"/> when the current transaction
        /// has completed succesfully, or immediately if no transaction is active.
        /// The passed in boolean indicates whether or invocation was postponed until
        /// an active transaction committed.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        public static void InvokePostCommit(Action<bool> action)
        {
            InvokePostCommit(action, Transaction.Current);
        }

        /// <summary>
        /// Invokes the specified <paramref name="action"/> when the specified <paramref name="transaction"/>
        /// has completed succesfully, or immediately if <paramref name="transaction"/> is <c>null</c>.
        /// The passed in boolean indicates whether or invocation was postponed until
        /// an active transaction committed.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="transaction">The transaction to observe.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        public static void InvokePostCommit(Action<bool> action, Transaction transaction)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }            
            if (transaction == null || HasCommitted(transaction))
            {
                action.Invoke(false);
            }
            else
            {
                transaction.TransactionCompleted += (s, e) =>
                {
                    if (HasCommitted(e.Transaction))
                    {
                        action.Invoke(true);
                    }
                };
            }
        }

        private static bool HasCommitted(Transaction transaction)
        {
            return transaction.TransactionInformation.Status == TransactionStatus.Committed;
        }

        #endregion        
    }
}
