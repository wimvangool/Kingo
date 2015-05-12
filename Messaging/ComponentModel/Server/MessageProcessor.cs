using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a handler of arbitrary messages.
    /// </summary>    
    public abstract class MessageProcessor : IMessageProcessor
    {
        private readonly InstanceLock _instanceLock;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ThreadLocal<MessagePointer> _currentMessagePointer;                 
        private readonly IMessageProcessorBus _domainEventBus;

        private readonly Lazy<MessageHandlerPipeline> _messageEntryPipeline;
        private readonly Lazy<MessageHandlerPipeline> _businessLogicPipeline;
        private readonly Lazy<QueryPipeline> _dataAccessPipeline;               

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>        
        protected MessageProcessor()
        {           
            _instanceLock = new InstanceLock(this, true);
            _currentMessagePointer = new ThreadLocal<MessagePointer>();
            _domainEventBus = new MessageProcessorBus(this);
            
            _messageEntryPipeline = new Lazy<MessageHandlerPipeline>(CreateMessageEntryPipeline, true);
            _businessLogicPipeline = new Lazy<MessageHandlerPipeline>(CreateBusinessLogicPipeline, true);
            _dataAccessPipeline = new Lazy<QueryPipeline>(CreateDataAccessPipeline, true);
        }

        internal MessageHandlerPipeline MessageEntryPipeline
        {
            get { return _messageEntryPipeline.Value; }
        }

        internal MessageHandlerPipeline BusinessLogicPipeline
        {
            get { return _businessLogicPipeline.Value; }
        }

        internal QueryPipeline DataAccessPipeline
        {
            get { return _dataAccessPipeline.Value; }
        }

        #region [====== Dispose ======]

        /// <summary>
        /// Returns the lock that is used to manage safe disposal of this instance.
        /// </summary>
        protected IInstanceLock InstanceLock
        {
            get { return _instanceLock; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _instanceLock.EnterDispose();

            try
            {
                Dispose(true);
            }
            finally
            {
                _instanceLock.ExitDispose();
            }            
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
            if (InstanceLock.IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                if (_messageEntryPipeline.IsValueCreated)
                {
                    _messageEntryPipeline.Value.Dispose();
                }
                if (_businessLogicPipeline.IsValueCreated)
                {
                    _businessLogicPipeline.Value.Dispose();
                }
                if (_dataAccessPipeline.IsValueCreated)
                {
                    _dataAccessPipeline.Value.Dispose();
                }
            }            
        }        

        #endregion

        /// <inheritdoc />
        public virtual IMessageProcessorBus EventBus
        {
            get { return _domainEventBus; }
        }

        /// <inheritdoc />
        public MessagePointer Message
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
        public UnitOfWorkScope CreateUnitOfWorkScope()
        {
            return new UnitOfWorkScope(this);
        }

        #region [====== ToString ======]

        /// <inheritdoc />
        public override string ToString()
        {
            var processorString = new StringBuilder(GetType().Name);

            processorString.AppendFormat(" ({0}, {1})",
                ToString(MessageHandlerFactory),
                ToString(Message));
            
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

        #endregion

        #region [====== Commands & Events ======]

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, NullHandler<TMessage>(), token);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, Action<TMessage> handler, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, (ActionDecorator<TMessage>) handler, token);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            InstanceLock.EnterMethod();

            try
            {                
                var callerContext = SynchronizationContext.Current;

                return Start(() =>
                {
                    var previousContext = SynchronizationContext.Current;

                    SynchronizationContext.SetSynchronizationContext(callerContext);

                    try
                    {
                        Handle(message, handler, token);
                    }
                    finally
                    {
                        SynchronizationContext.SetSynchronizationContext(previousContext);
                    }
                }, message.GetType(), token);
            }
            finally
            {
                InstanceLock.ExitMethod();
            }            
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, NullHandler<TMessage>(), token);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, Action<TMessage> handler, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, (ActionDecorator<TMessage>) handler, token);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageHandler<TMessage> handler, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>
        {            
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }            
            PushMessage(ref message, token);

            try
            {
                var dispatcher = new MessageHandlerDispatcher<TMessage>(message, handler, this);
                var pipeline = MessageEntryPipeline.ConnectTo(dispatcher);

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
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query, QueryExecutionOptions options = QueryExecutionOptions.Default, CancellationToken? token = null)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return ExecuteAsync(message, (FuncDecorator<TMessageIn, TMessageOut>) query, options, token);
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, QueryExecutionOptions options = QueryExecutionOptions.Default, CancellationToken? token = null)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {            
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            InstanceLock.EnterMethod();

            try
            {
                var callerContext = SynchronizationContext.Current;

                return Start(() =>
                {
                    var previousContext = SynchronizationContext.Current;

                    SynchronizationContext.SetSynchronizationContext(callerContext);

                    try
                    {
                        return Execute(message, query, options, token);
                    }
                    finally
                    {
                        SynchronizationContext.SetSynchronizationContext(previousContext);
                    }
                }, message.GetType(), token);
            }
            finally
            {
                InstanceLock.ExitMethod();
            }            
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query, QueryExecutionOptions options = QueryExecutionOptions.Default, CancellationToken? token = null)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return Execute(message, (FuncDecorator<TMessageIn, TMessageOut>) query, options, token);
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, QueryExecutionOptions options = QueryExecutionOptions.Default, CancellationToken? token = null)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {            
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            PushMessage(ref message, token);

            try
            {
                var handler = new QueryDispatcherModule<TMessageIn, TMessageOut>(message, query, options, this);
                
                MessageEntryPipeline.ConnectTo(handler).Invoke();

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
            InstanceLock.EnterMethod();
     
            Message = Message == null ?
                new MessagePointer(message = message.Copy(), token) :
                Message.CreateChildPointer(message = message.Copy(), token);                        
        }

        private void PopMessage()
        {
            Message = Message.ParentPointer;

            InstanceLock.ExitMethod();
        }               

        private MessageHandlerPipeline CreateMessageEntryPipeline()
        {
            var pipeline = new MessageHandlerPipeline(CreateMessageEntryPipelineModules());
            pipeline.Start();
            return pipeline;
        }

        /// <summary>
        /// Creates and returns a collection of <see cref="MessageHandlerModule">modules</see>
        /// that will be used to create a pipeline for every incoming message.
        /// </summary>                
        /// <returns>A collection of <see cref="MessageHandlerModule">modules</see>.</returns>              
        protected abstract IEnumerable<MessageHandlerModule> CreateMessageEntryPipelineModules();

        private MessageHandlerPipeline CreateBusinessLogicPipeline()
        {
            var pipeline = new MessageHandlerPipeline(CreateBusinessLogicPipelineModules());
            pipeline.Start();
            return pipeline;
        }

        /// <summary>
        /// Creates and returns a collection of <see cref="MessageHandlerModule">modules</see> that will be used to
        /// create a pipeline for every <see cref="IMessageHandler{TMessage}" /> handling a certain message.
        /// </summary>               
        /// <returns>A pipeline that will handle a message.</returns>              
        protected internal virtual IEnumerable<MessageHandlerModule> CreateBusinessLogicPipelineModules()
        {
            return Enumerable.Empty<MessageHandlerModule>();
        }

        private QueryPipeline CreateDataAccessPipeline()
        {
            var pipeline = new QueryPipeline(CreateDataAccessPipelineModules());
            pipeline.Start();
            return pipeline;
        }

        /// <summary>
        /// Creates and returns a collection of <see cref="QueryModule">modules</see> that will be used to
        /// create a pipeline for every query that is executed.
        /// </summary>                     
        /// <returns>A query pipeline.</returns>             
        protected virtual IEnumerable<QueryModule> CreateDataAccessPipelineModules()            
        {
            return Enumerable.Empty<QueryModule>();
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

        #region [====== CurrentMessage ======]

        /// <summary>
        /// Returns a <see cref="MessagePointer">pointer</see> to the message that is currently being handled.
        /// </summary>
        public static MessagePointer CurrentMessage
        {
            get
            {
                var context = UnitOfWorkContext.Current;
                if (context == null)
                {
                    return null;
                }
                return context.Processor.Message;
            }
        }

        #endregion

        #region [====== Publish ======]

        /// <summary>
        /// Publishes the specified <paramref name="message"/> as part of the current Unit of Work.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to publish.</typeparam>
        /// <param name="message">The message to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The method is not being called inside a <see cref="UnitOfWorkScope" />.
        /// </exception>
        public static void Publish<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            if (TryPublish(message))
            {
                return;
            }
            throw NoEventBusAvailableException(message);
        }        

        /// <summary>
        /// Publishes the specified <paramref name="message"/> as part of the current Unit of Work, if and only if
        /// the method is being called inside a <see cref="UnitOfWorkScope" />.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to publish.</typeparam>
        /// <param name="message">The message to publish.</param>
        /// <returns><c>true</c> if the message was published; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static bool TryPublish<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var context = UnitOfWorkContext.Current;
            if (context != null)
            {
                context.Publish(message);
                return true;
            }
            return false;
        }

        private static Exception NoEventBusAvailableException(object theMessage)
        {
            var messageFormat = ExceptionMessages.MessageProcessor_NoEventBusAvailable;
            var message = string.Format(messageFormat, theMessage);
            return new InvalidOperationException(message);
        }

        #endregion

        #region [====== Enlist ======]

        /// <summary>
        /// Enlists the specified <paramref name="unitOfWork"/> with the current unit of work, if it is present.
        /// If no unit of work is present, the specified unit is flushed immediately if required.
        /// </summary>
        /// <param name="unitOfWork">The unit of work to enlist.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="unitOfWork"/> is <c>null</c>.
        /// </exception>
        public static void Enlist(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }
            var context = UnitOfWorkContext.Current;
            if (context != null)
            {
                context.Enlist(unitOfWork);               
            }
            else if (unitOfWork.RequiresFlush())
            {
                unitOfWork.Flush();
            }            
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
