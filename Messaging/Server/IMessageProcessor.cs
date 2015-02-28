using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// When implemented by a class, represents a handler of any message.
    /// </summary>
    public interface IMessageProcessor : IDisposable
    {
        /// <summary>
        /// Returns the <see cref="IMessageProcessorBus" /> of this processor.
        /// </summary>
        IMessageProcessorBus DomainEventBus
        {
            get;
        }

        /// <summary>
        /// Returns a pointer to the message that is currently being handled by the processor.
        /// </summary>
        MessagePointer MessagePointer
        {
            get;
        }

        #region [====== Commands & Events ======]

        /// <summary>
        /// Processes the specified message by invoking all registered message handlers asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="validator">
        /// Optional custom validator of the message.
        /// If <c>null</c>, the default validator provided by the <paramref name="message" /> is used.
        /// </param>        
        /// <param name="token">Optional token that can be used to cancel the operation.</param>  
        /// <returns>The <see cref="Task" /> that is handling the <paramref name="message"/>.</returns>               
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task HandleAsync<TMessage>(TMessage message, IMessageValidator<TMessage> validator = null, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>;

        /// <summary>
        /// Processes the specified message by invoking the specified handler asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>
        /// <param name="validator">
        /// Optional custom validator of the message.
        /// If <c>null</c>, the default validator provided by the <paramref name="message" /> is used.
        /// </param>        
        /// <param name="token">Optional token that can be used to cancel the operation.</param>  
        /// <returns>The <see cref="Task" /> that is handling the <paramref name="message"/>.</returns>               
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task HandleAsync<TMessage>(TMessage message, Action<TMessage> handler, IMessageValidator<TMessage> validator = null, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>;

        /// <summary>
        /// Processes the specified message by invoking the specified handler asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>
        /// <param name="validator">
        /// Optional custom validator of the message.
        /// If <c>null</c>, the default validator provided by the <paramref name="message" /> is used.
        /// </param>        
        /// <param name="token">Optional token that can be used to cancel the operation.</param>  
        /// <returns>The <see cref="Task" /> that is handling the <paramref name="message"/>.</returns>               
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>  
        Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler, IMessageValidator<TMessage> validator = null, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>;

        /// <summary>
        /// Processes the specified message by invoking all registered message handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="validator">
        /// Optional custom validator of the message.
        /// If <c>null</c>, the default validator provided by the <paramref name="message" /> is used.
        /// </param>        
        /// <param name="token">Optional token that can be used to cancel the operation.</param>                
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception>
        void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator = null, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>;

        /// <summary>
        /// Processes the specified message by invoking the specified handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>
        /// <param name="validator">
        /// Optional custom validator of the message.
        /// If <c>null</c>, the default validator provided by the <paramref name="message" /> is used.
        /// </param>        
        /// <param name="token">Optional token that can be used to cancel the operation.</param>                
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception>
        void Handle<TMessage>(TMessage message, Action<TMessage> handler, IMessageValidator<TMessage> validator = null, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>;

        /// <summary>
        /// Processes the specified message by invoking the specified handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>
        /// <param name="validator">
        /// Optional custom validator of the message.
        /// If <c>null</c>, the default validator provided by the <paramref name="message" /> is used.
        /// </param>        
        /// <param name="token">Optional token that can be used to cancel the operation.</param>                
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception>
        void Handle<TMessage>(TMessage message, IMessageHandler<TMessage> handler, IMessageValidator<TMessage> validator = null, CancellationToken? token = null) where TMessage : class, IMessage<TMessage>;

        #endregion

        #region [====== Queries ======]

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="validator">
        /// Optional custom validator of the message.
        /// If <c>null</c>, the default validator provided by the <paramref name="message" /> is used.
        /// </param>
        /// <param name="options">Specifies options for how the processor should execute the specified <paramref name="query"/>.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param> 
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception> 
        Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query, IMessageValidator<TMessageIn> validator = null, QueryExecutionOptions options = QueryExecutionOptions.Default, CancellationToken? token = null)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>;

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="validator">
        /// Optional custom validator of the message.
        /// If <c>null</c>, the default validator provided by the <paramref name="message" /> is used.
        /// </param>
        /// <param name="options">Specifies options for how the processor should execute the specified <paramref name="query"/>.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param> 
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception> 
        Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, IMessageValidator<TMessageIn> validator = null, QueryExecutionOptions options = QueryExecutionOptions.Default, CancellationToken? token = null)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>;

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="validator">
        /// Optional custom validator of the message.
        /// If <c>null</c>, the default validator provided by the <paramref name="message" /> is used.
        /// </param> 
        /// <param name="options">Specifies options for how the processor should execute the specified <paramref name="query"/>.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param>  
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception>  
        TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, Func<TMessageIn, TMessageOut> query, IMessageValidator<TMessageIn> validator = null, QueryExecutionOptions options = QueryExecutionOptions.Default, CancellationToken? token = null)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>;

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="validator">
        /// Optional custom validator of the message.
        /// If <c>null</c>, the default validator provided by the <paramref name="message" /> is used.
        /// </param> 
        /// <param name="options">Specifies options for how the processor should execute the specified <paramref name="query"/>.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param>  
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception>  
        TMessageOut Execute<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, IMessageValidator<TMessageIn> validator = null, QueryExecutionOptions options = QueryExecutionOptions.Default, CancellationToken? token = null)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>;
       
        #endregion        
    }
}
