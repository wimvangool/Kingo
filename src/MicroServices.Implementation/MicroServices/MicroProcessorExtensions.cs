using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Contains extension methods for the <see cref="IMicroProcessor" /> interface.
    /// </summary>
    public static class MicroProcessorExtensions
    {
        #region [====== ExecuteCommandAsync ======]                  

        /// <summary>
        /// Executes a command with a specified <paramref name="messageHandler"/>.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command.</typeparam>
        /// <param name="processor">The processor used to execute the command.</param>
        /// <param name="messageHandler">The message handler that will handle the command.</param>
        /// <param name="message">The command to handle.</param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>
        /// The result of the operation, which includes all published events and the number of message handlers that were invoked.
        /// </returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/>, <paramref name="messageHandler"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>        
        public static Task<IMessageHandlerOperationResult> ExecuteCommandAsync<TCommand>(this IMicroProcessor processor, Action<TCommand, IMessageHandlerOperationContext> messageHandler, TCommand message, CancellationToken? token = null) =>
            NotNull(processor).ExecuteCommandAsync(MessageHandlerDecorator<TCommand>.Decorate(messageHandler), message, token);

        /// <summary>
        /// Executes a command with a specified <paramref name="messageHandler"/>.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command.</typeparam>
        /// <param name="processor">The processor used to execute the command.</param>
        /// <param name="messageHandler">The message handler that will handle the command.</param>
        /// <param name="message">The command to handle.</param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>
        /// The result of the operation, which includes all published events and the number of message handlers that were invoked.
        /// </returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/>, <paramref name="messageHandler"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>         
        public static Task<IMessageHandlerOperationResult> ExecuteCommandAsync<TCommand>(this IMicroProcessor processor, Func<TCommand, IMessageHandlerOperationContext, Task> messageHandler, TCommand message, CancellationToken? token = null) =>
            NotNull(processor).ExecuteCommandAsync(MessageHandlerDecorator<TCommand>.Decorate(messageHandler), message, token);

        #endregion

        #region [====== HandleEventAsync ======]                  

        /// <summary>
        /// Handles an event with a specified <paramref name="messageHandler"/>.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="processor">The processor used to handle the event.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <param name="message">The event to handle.</param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>
        /// The result of the operation, which includes all published events and the number of message handlers that were invoked.
        /// </returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/>, <paramref name="messageHandler"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>        
        public static Task<IMessageHandlerOperationResult> HandleEventAsync<TEvent>(this IMicroProcessor processor, Action<TEvent, IMessageHandlerOperationContext> messageHandler, TEvent message, CancellationToken? token = null) =>
            NotNull(processor).HandleEventAsync(MessageHandlerDecorator<TEvent>.Decorate(messageHandler), message, token);

        /// <summary>
        /// Handles an event with a specified <paramref name="messageHandler"/>.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="processor">The processor used to handle the event.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <param name="message">The event to handle.</param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>
        /// The result of the operation, which includes all published events and the number of message handlers that were invoked.
        /// </returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/>, <paramref name="messageHandler"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>         
        public static Task<IMessageHandlerOperationResult> HandleEventAsync<TEvent>(this IMicroProcessor processor, Func<TEvent, IMessageHandlerOperationContext, Task> messageHandler, TEvent message, CancellationToken? token = null) =>
            NotNull(processor).HandleEventAsync(MessageHandlerDecorator<TEvent>.Decorate(messageHandler), message, token);

        #endregion

        #region [====== ExecuteQueryAsync ======]

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TResponse">Type of the message returned by the query.</typeparam>   
        /// <param name="processor">A processor.</param>     
        /// <param name="query">The query to execute.</param>               
        /// <param name="token">Optional token that can be used to cancel the operation.</param>          
        /// <returns>The result that carries the response returned by the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/>, <paramref name="query"/> is <c>null</c>.
        /// </exception>        
        public static Task<IQueryOperationResult<TResponse>> ExecuteQueryAsync<TResponse>(this IMicroProcessor processor, Func<IQueryOperationContext, TResponse> query, CancellationToken? token = null) =>
            NotNull(processor).ExecuteQueryAsync(QueryDecorator<TResponse>.Decorate(query), token);

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TResponse">Type of the message returned by the query.</typeparam> 
        /// <param name="processor">A processor.</param>       
        /// <param name="query">The query to execute.</param>               
        /// <param name="token">Optional token that can be used to cancel the operation.</param>          
        /// <returns>The result that carries the response returned by the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/>, <paramref name="query"/> is <c>null</c>.
        /// </exception>        
        public static Task<IQueryOperationResult<TResponse>> ExecuteQueryAsync<TResponse>(this IMicroProcessor processor, Func<IQueryOperationContext, Task<TResponse>> query, CancellationToken? token = null) =>
            NotNull(processor).ExecuteQueryAsync(QueryDecorator<TResponse>.Decorate(query), token);

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">Type of the message going into the query.</typeparam>
        /// <typeparam name="TResponse">Type of the message returned by the query.</typeparam>
        /// <param name="processor">A processor.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>The result that carries the response returned by the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/>, <paramref name="query"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>          
        public static Task<IQueryOperationResult<TResponse>> ExecuteQueryAsync<TRequest, TResponse>(this IMicroProcessor processor, Func<TRequest, IQueryOperationContext, TResponse> query, TRequest message, CancellationToken? token = null) =>
            NotNull(processor).ExecuteQueryAsync(QueryDecorator<TRequest, TResponse>.Decorate(query), message, token);

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">Type of the message going into the query.</typeparam>
        /// <typeparam name="TResponse">Type of the message returned by the query.</typeparam>
        /// <param name="processor">A processor.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>The result that carries the response returned by the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/>, <paramref name="query"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>        
        public static Task<IQueryOperationResult<TResponse>> ExecuteQueryAsync<TRequest, TResponse>(this IMicroProcessor processor, Func<TRequest, IQueryOperationContext, Task<TResponse>> query, TRequest message, CancellationToken? token = null) =>
            NotNull(processor).ExecuteQueryAsync(QueryDecorator<TRequest, TResponse>.Decorate(query), message, token);

        #endregion

        private static IMicroProcessor NotNull(IMicroProcessor processor) =>
            processor ?? throw new ArgumentNullException(nameof(processor));
    }
}
