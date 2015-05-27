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
    public class MessageProcessor : IMessageProcessor
    {        
        private readonly IMessageProcessorBus _domainEventBus;        

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>        
        public MessageProcessor()
        {                       
            _domainEventBus = new MessageProcessorBus(this);               
        }                

        /// <inheritdoc />
        public virtual IMessageProcessorBus EventBus
        {
            get { return _domainEventBus; }
        }        

        internal UnitOfWorkScope CreateUnitOfWorkScope()
        {
            return new UnitOfWorkScope(this);
        }

        /// <summary>
        /// Returns the <see cref="MessageHandlerFactory" /> of this processor.
        /// </summary>
        protected internal virtual MessageHandlerFactory MessageHandlerFactory
        {
            get { return null; }
        }

        #region [====== ToString ======]

        /// <inheritdoc />
        public override string ToString()
        {
            var processorString = new StringBuilder(GetType().Name);

            processorString.AppendFormat(" ({0}, {1})",
                ToString(MessageHandlerFactory),
                ToString(CurrentMessage));
            
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
        public void Handle<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, NullHandler<TMessage>());
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, Action<TMessage> handler) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, (ActionDecorator<TMessage>) handler);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class, IMessage<TMessage>
        {
            HandleAsync(message, handler, CancellationToken.None).Wait();
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, NullHandler<TMessage>(), CancellationToken.None);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, CancellationToken token) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, NullHandler<TMessage>(), token);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, Action<TMessage> handler) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, (ActionDecorator<TMessage>) handler, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, Action<TMessage> handler, CancellationToken token) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, (ActionDecorator<TMessage>) handler, token);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, handler, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler, CancellationToken token) where TMessage : class, IMessage<TMessage>
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return HandleAsyncCore(message, handler, token);
        }

        private async Task HandleAsyncCore<TMessage>(TMessage message, IMessageHandler<TMessage> handler, CancellationToken token) where TMessage : class, IMessage<TMessage>
        {
            PushMessage(ref message, token);

            try
            {
                await BuildMessageEntryPipeline()
                    .ConnectTo(new MessageHandlerDispatcher<TMessage>(message, handler, this))
                    .InvokeAsync();
            }
            finally
            {
                PopMessage();
            } 
        }

        #endregion      
  
        #region [====== Queries ======]

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return Execute(message, (FuncDecorator<TMessageIn, TMessageOut>) query);
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return ExecuteAsync(message, query, CancellationToken.None).Result;            
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return ExecuteAsync(message, query, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query, CancellationToken token)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return ExecuteAsync(message, (FuncDecorator<TMessageIn, TMessageOut>) query, token);
        }        

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {            
            return ExecuteAsync(message, query, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, CancellationToken token)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {                        
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return ExecuteAsyncCore(message, query, token);
        }

        private async Task<TMessageOut> ExecuteAsyncCore<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, CancellationToken token)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {            
            PushMessage(ref message, token);

            try
            {
                var handler = new QueryDispatcherModule<TMessageIn, TMessageOut>(message, query, this);

                await BuildMessageEntryPipeline().ConnectTo(handler).InvokeAsync();

                return handler.MessageOut;
            }
            finally
            {
                PopMessage();
            }
        }  

        #endregion                      

        #region [====== Pipeline Factories ======]

        private MessageHandlerPipeline BuildMessageEntryPipeline()
        {
            return new MessageHandlerPipeline(CreateMessageEntryPipeline());           
        }

        /// <summary>
        /// Creates and returns a collection of <see cref="MessageHandlerModule">modules</see>
        /// that will be used to create a pipeline for every incoming message.
        /// </summary>                
        /// <returns>A collection of <see cref="MessageHandlerModule">modules</see>.</returns>              
        protected virtual IEnumerable<MessageHandlerModule> CreateMessageEntryPipeline()
        {
            return Enumerable.Empty<MessageHandlerModule>();
        }

        internal MessageHandlerPipeline BuildCommandOrEventHandlerPipeline()
        {
            return new MessageHandlerPipeline(CreateCommandOrEventHandlerPipeline());            
        }

        /// <summary>
        /// Creates and returns a collection of <see cref="MessageHandlerModule">modules</see> that will be used to
        /// create a pipeline for every <see cref="IMessageHandler{TMessage}" /> handling a certain message.
        /// </summary>               
        /// <returns>A pipeline that will handle a message.</returns>              
        protected virtual IEnumerable<MessageHandlerModule> CreateCommandOrEventHandlerPipeline()
        {
            return Enumerable.Empty<MessageHandlerModule>();
        }

        internal QueryPipeline BuildQueryExecutionPipeline()
        {
            return new QueryPipeline(CreateQueryExecutionPipeline());            
        }

        /// <summary>
        /// Creates and returns a collection of <see cref="QueryModule">modules</see> that will be used to
        /// create a pipeline for every query that is executed.
        /// </summary>                     
        /// <returns>A query pipeline.</returns>             
        protected virtual IEnumerable<QueryModule> CreateQueryExecutionPipeline()            
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly AsyncLocal<MessagePointer> _CurrentMessage = new AsyncLocal<MessagePointer>();

        /// <summary>
        /// Returns a <see cref="MessagePointer">pointer</see> to the message that is currently being handled.
        /// </summary>
        public static MessagePointer CurrentMessage
        {
            get { return _CurrentMessage.Value; }
            private set { _CurrentMessage.Value = value; }
        }

        private static void PushMessage<TMessage>(ref TMessage message, CancellationToken token) where TMessage : class, IMessage<TMessage>
        {
            CurrentMessage = CurrentMessage == null ?
                new MessagePointer(message = message.Copy(), token) :
                CurrentMessage.CreateChildPointer(message = message.Copy(), token);
        }

        private static void PopMessage()
        {
            CurrentMessage = CurrentMessage.ParentPointer;
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
        /// <exception cref="InvalidOperationException">
        /// Enlistment failed because no message is currently being handled.
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
                return;
            }
            throw NewEnlistFailedException(unitOfWork);
        }

        private static Exception NewEnlistFailedException(IUnitOfWork unitOfWork)
        {
            var messageFormat = ExceptionMessages.MessageProcessor_EnlistFailed;
            var message = string.Format(messageFormat, unitOfWork.GetType().Name);
            return new InvalidOperationException(message);
        }        

        #endregion

        #region [====== InvokePostCommit ======]

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
