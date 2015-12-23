using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Kingo.Threading;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a handler of arbitrary messages.
    /// </summary>    
    public class MessageProcessor : IMessageProcessor
    {        
        private static readonly ConcurrentDictionary<Type, MessageHandlerFactory> _MessageHandlerFactories = new ConcurrentDictionary<Type, MessageHandlerFactory>();
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

        /// <summary>
        /// Returns the <see cref="MessageHandlerFactory" /> of this processor.
        /// </summary>
        protected internal MessageHandlerFactory MessageHandlerFactory
        {
            get { return _MessageHandlerFactories.GetOrAdd(GetType(), type => CreateMessageHandlerFactory()); }
        }

        /// <summary>
        /// Creates and returns a <see cref="MessageHandlerFactory" /> for this processor.
        /// </summary>
        /// <returns>
        /// A new <see cref="MessageHandlerFactory" /> to be used by this processor,
        /// or <c>null</c> if this processor does not use any factory.</returns>
        protected virtual MessageHandlerFactory CreateMessageHandlerFactory()
        {
            return null;
        }

        /// <inheritdoc />
        public UnitOfWorkScope CreateUnitOfWorkScope()
        {
            return UnitOfWorkContext.StartUnitOfWorkScope(this);
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
            Handle(message, new MessageHandlerDelegate<TMessage>(handler));
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, Func<TMessage, Task> handler) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, new MessageHandlerDelegate<TMessage>(handler));
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class, IMessage<TMessage>
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            HandleAsync(message, handler, CancellationToken.None).Await();
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
            return HandleAsync(message, new MessageHandlerDelegate<TMessage>(handler), CancellationToken.None);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, Action<TMessage> handler, CancellationToken token) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, new MessageHandlerDelegate<TMessage>(handler), token);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, Func<TMessage, Task> handler) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, new MessageHandlerDelegate<TMessage>(handler), CancellationToken.None);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, Func<TMessage, Task> handler, CancellationToken token) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, new MessageHandlerDelegate<TMessage>(handler), token);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class, IMessage<TMessage>
        {
            return HandleAsync(message, handler, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler, CancellationToken token) where TMessage : class, IMessage<TMessage>
        {            
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
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

        /// <summary>
        /// Determines whether or not the specified message is a Command. By default,
        /// this method returns <c>true</c> when the type-name ends with 'Command'.
        /// </summary>
        /// <param name="message">The message to analyze.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="message"/> is a command; otherwise <c>false</c>.
        /// </returns>
        protected internal virtual bool IsCommand(object message)
        {
            return message.GetType().Name.EndsWith("Command");
        }

        #endregion      
  
        #region [====== Queries ======]

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return Execute(message, new MessageHandlerDelegate<TMessageIn, TMessageOut>(query));
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, Task<TMessageOut>> query)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return Execute(message, new MessageHandlerDelegate<TMessageIn, TMessageOut>(query));
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return ExecuteAsync(message, query, CancellationToken.None).Await();
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
            return ExecuteAsync(message, new MessageHandlerDelegate<TMessageIn, TMessageOut>(query), token);
        }  

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, Task<TMessageOut>> query)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return ExecuteAsync(message, query, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, Task<TMessageOut>> query, CancellationToken token)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            return ExecuteAsync(message, new MessageHandlerDelegate<TMessageIn, TMessageOut>(query), token);
        }        

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {            
            return ExecuteAsync(message, query, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, CancellationToken token)
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
