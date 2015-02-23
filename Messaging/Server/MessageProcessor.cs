using System.Diagnostics;
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
        protected internal abstract MessageHandlerFactory MessageHandlerFactory
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
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return Start(() => Handle(message, handler, validator, token), message.GetType(), token);
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
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            PushMessage(ref message, token);

            try
            {
                handler = new MessageDispatcherModule<TMessage>(handler, this);
                handler = CreatePerMessagePipeline(validator).CreateMessageHandlerPipeline(handler);
                handler.Handle(message);
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
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return Start(() => Execute(message, query, validator, options, token), message.GetType(), token);
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
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            PushMessage(ref message, token);

            try
            {
                var handler = new QueryDispatcherModule<TMessageIn, TMessageOut>(query, options, this);

                CreatePerMessagePipeline(validator).CreateMessageHandlerPipeline(handler).Handle(message);

                return handler.Result;
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
        /// Creates and returns a <see cref="IMessageHandlerPipelineFactory{TMessage}" /> that will be used to
        /// create a pipeline for every incoming message.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>        
        /// <param name="validator">Optional validator of the message.</param>
        /// <returns>A new <see cref="IMessageHandlerPipelineFactory{TMessage}" />.</returns>        
        protected virtual IMessageHandlerPipelineFactory<TMessage> CreatePerMessagePipeline<TMessage>(IMessageValidator<TMessage> validator) where TMessage : class, IMessage
        {
            return new MessageHandlerPipelineFactory<TMessage>()
            {                
                handler => new MessageValidationModule<TMessage>(handler, validator),
                handler => new TransactionScopeModule<TMessage>(handler)
            };
        }

        /// <summary>
        /// Creates and returns a <see cref="IMessageHandlerPipelineFactory{TMessage}" /> that will be used to
        /// create a pipeline for every <see cref="IMessageHandler{TMessage}" /> handling a certain message.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>        
        /// <returns>A pipeline that will handle a message.</returns>        
        protected internal virtual IMessageHandlerPipelineFactory<TMessage> CreatePerMessageHandlerPipeline<TMessage>() where TMessage : class
        {
            return new MessageHandlerPipelineFactory<TMessage>();
        }

        /// <summary>
        /// Creates and returns a <see cref="IQueryPipelineFactory{TMessageIn, TMessageOut}"/> that will be used to
        /// create a pipeline for every query that is executed.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>               
        /// <returns>A query pipeline.</returns>        
        protected internal virtual IQueryPipelineFactory<TMessageIn, TMessageOut> CreateQueryPipeline<TMessageIn, TMessageOut>(QueryExecutionOptions options)
            where TMessageIn : class, IMessage<TMessageIn>           
        {            
            return new QueryPipelineFactory<TMessageIn, TMessageOut>();
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
