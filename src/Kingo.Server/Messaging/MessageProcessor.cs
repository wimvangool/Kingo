using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
        public IMessageProcessorBus EventBus
        {
            get { return _domainEventBus; }
        }                

        /// <summary>
        /// Returns the <see cref="MessageHandlerFactory" /> of this processor.
        /// </summary>
        protected internal MessageHandlerFactory MessageHandlerFactory
        {
            get
            {
                return _MessageHandlerFactories.GetOrAdd(GetType(), type =>
                {
                    var layers = CreateLayerConfiguration(type);

                    var factory = CreateMessageHandlerFactory(layers);
                    if (factory != null)
                    {
                        factory.RegisterSingleton(CreateTypeToContractMap(layers), typeof(ITypeToContractMap));
                    }
                    return factory;
                });
            }
        }

        /// <summary>
        /// Creates and returns a new <see cref="LayerConfiguration" /> that is used by this processor.
        /// </summary>
        /// <returns>A new <see cref="LayerConfiguration" />.</returns>
        public LayerConfiguration CreateLayerConfiguration()
        {
            return CreateLayerConfiguration(GetType());
        }

        private LayerConfiguration CreateLayerConfiguration(Type processorType)
        {            
            return Customize(LayerConfiguration.CreateDefaultConfiguration(processorType.Assembly));
        }

        /// <summary>
        /// When overridden, customizes the specified <paramref name="layers"/> and returns the result.
        /// </summary>
        /// <param name="layers">The configuration to customize.</param>
        /// <returns>The customized configuration.</returns>
        protected virtual LayerConfiguration Customize(LayerConfiguration layers)
        {
            return layers;
        }

        /// <summary>
        /// Creates and returns a <see cref="MessageHandlerFactory" /> for this processor. By default, this method
        /// auto-registers all message handlers and repositories found in the appropriate <paramref name="layers"/>.
        /// </summary>
        /// <param name="layers">
        /// A configuration of all logical layers of the application, which can be used to
        /// auto-register all messagehandlers, repositories and other dependencies.
        /// </param>
        /// <returns>
        /// A new <see cref="MessageHandlerFactory" /> to be used by this processor,
        /// or <c>null</c> if this processor does not use any factory.
        /// </returns>
        protected virtual MessageHandlerFactory CreateMessageHandlerFactory(LayerConfiguration layers)
        {
            var factory = CreateDefaultMessageHandlerFactory();
            factory.RegisterMessageHandlers(layers);
            factory.RegisterRepositories(layers);
            return factory;
        }

        /// <summary>
        /// Creates and return a new instance of the built-in, default message handler factory.
        /// </summary>
        /// <returns>A new instance of the built-in, default message handler factory.</returns>
        protected MessageHandlerFactory CreateDefaultMessageHandlerFactory()
        {
            return new UnityFactory();
        }

        /// <summary>
        /// Creates and returns a mapping between types and their contracts.
        /// This map will be registered with the <see cref="MessageHandlerFactory" /> of this processor.        
        /// </summary>
        /// <param name="layers">
        /// A configuration of all logical layers of the application, which can be used to obtain all types
        /// that must be mapped to a contract.
        /// </param>
        /// <returns>A mapping between types and their contracts.</returns>
        protected virtual ITypeToContractMap CreateTypeToContractMap(LayerConfiguration layers)
        {
            return TypeToContractMap.FromLayerConfiguration(layers);
        }

        /// <summary>
        /// This method is invoked just before an event is published and subsequently handled by this processor.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event that is about to be published.</typeparam>
        /// <param name="event">The event that is about to be published.</param>   
        /// <returns>A <see cref="Task" /> carrying out the operation.</returns>     
        protected internal virtual Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IMessage
        {
            return HandleAsync(@event);
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
            return MessageHandlerFactory.ToString();
        }        

        #endregion

        #region [====== Commands & Events ======]

        private static readonly ConcurrentDictionary<Type, MethodInfo> _HandleMethods = new ConcurrentDictionary<Type, MethodInfo>();

        /// <inheritdoc />
        public void Handle(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _HandleMethods.GetOrAdd(message.GetType(), GetHandleMethod).Invoke(this, new[] { message });
        }        

        private MethodInfo GetHandleMethod(Type messageType)
        {
            return GetHandleMethodDefinition().MakeGenericMethod(messageType);
        }

        private MethodInfo GetHandleMethodDefinition()
        {                        
            var methods =
                from method in GetType().GetInterfaceMap(typeof(IMessageProcessor)).TargetMethods
                where method.IsGenericMethod && method.IsGenericMethodDefinition
                where method.Name == "Handle" && method.GetParameters().Length == 1                              
                select method;

            return methods.Single();
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            Handle(message, NullHandler<TMessage>());
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, Action<TMessage> handler) where TMessage : class, IMessage
        {
            Handle(message, new MessageHandlerDelegate<TMessage>(handler));
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, Func<TMessage, Task> handler) where TMessage : class, IMessage
        {
            Handle(message, new MessageHandlerDelegate<TMessage>(handler));
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class, IMessage
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            HandleAsync(message, handler, CancellationToken.None).Await();
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            return HandleAsync(message, NullHandler<TMessage>(), CancellationToken.None);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, CancellationToken token) where TMessage : class, IMessage
        {
            return HandleAsync(message, NullHandler<TMessage>(), token);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, Action<TMessage> handler) where TMessage : class, IMessage
        {
            return HandleAsync(message, new MessageHandlerDelegate<TMessage>(handler), CancellationToken.None);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, Action<TMessage> handler, CancellationToken token) where TMessage : class, IMessage
        {
            return HandleAsync(message, new MessageHandlerDelegate<TMessage>(handler), token);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, Func<TMessage, Task> handler) where TMessage : class, IMessage
        {
            return HandleAsync(message, new MessageHandlerDelegate<TMessage>(handler), CancellationToken.None);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, Func<TMessage, Task> handler, CancellationToken token) where TMessage : class, IMessage
        {
            return HandleAsync(message, new MessageHandlerDelegate<TMessage>(handler), token);
        }

        /// <inheritdoc />
        public Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class, IMessage
        {
            return HandleAsync(message, handler, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler, CancellationToken token) where TMessage : class, IMessage
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
        public TMessageOut Execute<TMessageOut>(Func<TMessageOut> query) where TMessageOut : class, IMessage
        {
            return Execute(new QueryDelegate<TMessageOut>(query));
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageOut>(IQuery<TMessageOut> query) where TMessageOut : class, IMessage
        {
            return ExecuteAsync(query).Await();
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(Func<TMessageIn, TMessageOut> query, TMessageIn message)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage
        {
            return Execute(new QueryDelegate<TMessageIn, TMessageOut>(query), message);
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(Func<TMessageIn, Task<TMessageOut>> query, TMessageIn message)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage
        {
            return Execute(new QueryDelegate<TMessageIn, TMessageOut>(query), message);
        }

        /// <inheritdoc />
        public TMessageOut Execute<TMessageIn, TMessageOut>(IQuery<TMessageIn, TMessageOut> query, TMessageIn message)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage
        {
            return ExecuteAsync(query, message, CancellationToken.None).Await();
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageOut>(Func<Task<TMessageOut>> query) where TMessageOut : class, IMessage
        {
            return ExecuteAsync(query, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageOut>(Func<Task<TMessageOut>> query, CancellationToken token) where TMessageOut : class, IMessage
        {
            return ExecuteAsync(new QueryDelegate<TMessageOut>(query), token);
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageOut>(IQuery<TMessageOut> query) where TMessageOut : class, IMessage
        {
            return ExecuteAsync(query, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageOut>(IQuery<TMessageOut> query, CancellationToken token) where TMessageOut : class, IMessage
        {            
            return ExecuteAsync(new QueryWrapper<TMessageOut>(query, NullMessage.Instance), NullMessage.Instance, token);
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(Func<TMessageIn, TMessageOut> query, TMessageIn message)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage
        {
            return ExecuteAsync(query, message, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(Func<TMessageIn, TMessageOut> query, TMessageIn message, CancellationToken token)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage
        {
            return ExecuteAsync(new QueryDelegate<TMessageIn, TMessageOut>(query), message, token);
        }  

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(Func<TMessageIn, Task<TMessageOut>> query, TMessageIn message)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage
        {
            return ExecuteAsync(query, message, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(Func<TMessageIn, Task<TMessageOut>> query, TMessageIn message, CancellationToken token)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage
        {
            return ExecuteAsync(new QueryDelegate<TMessageIn, TMessageOut>(query), message, token);
        }        

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(IQuery<TMessageIn, TMessageOut> query, TMessageIn message)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage
        {            
            return ExecuteAsync(query, message, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(IQuery<TMessageIn, TMessageOut> query, TMessageIn message, CancellationToken token)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage
        {            
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return ExecuteAsync(new QueryWrapper<TMessageIn, TMessageOut>(query, message), message, token);
        }      
  
        private async Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(IQueryWrapper<TMessageOut> query, TMessageIn message, CancellationToken token)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage
        {
            PushMessage(ref message, token);

            try
            {                
                var handler = new QueryDispatcherModule<TMessageOut>(query, this);

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

        private static void PushMessage<TMessage>(ref TMessage message, CancellationToken token) where TMessage : class, IMessage
        {
            CurrentMessage = CurrentMessage == null ?
                new MessagePointer(message = (TMessage) message.Copy(), token) :
                CurrentMessage.CreateChildPointer(message = (TMessage) message.Copy(), token);
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
