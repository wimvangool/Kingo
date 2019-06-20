using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a processor that can process commands, events and queries.
    /// </summary>
    public interface IMicroProcessor
    {
        /// <summary>
        /// Returns the service provider the processor uses to resolve its dependencies.
        /// </summary>
        IMicroProcessorServiceProvider ServiceProvider
        {
            get;
        }

        /// <summary>
        /// Creates and returns all endpoints that reference the methods of all registered
        /// message handlers that are marked by the <see cref="EndpointAttribute" />.
        /// </summary>
        /// <returns>A collection of endpoints.</returns>
        IEnumerable<HandleAsyncMethodEndpoint> CreateMethodEndpoints();

        /// <summary>
        /// Executes a command with a specified <paramref name="messageHandler"/>.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command.</typeparam>
        /// <param name="messageHandler">The message handler that will handle the command.</param>
        /// <param name="message">The command to handle.</param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>
        /// The result of the operation, which includes all published events and the number of message handlers that were invoked.
        /// </returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>       
        Task<MessageHandlerOperationResult> ExecuteCommandAsync<TCommand>(IMessageHandler<TCommand> messageHandler, TCommand message, CancellationToken? token = null);

        /// <summary>
        /// Handles an event with a specified <paramref name="messageHandler"/>.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <param name="message">The event to handle.</param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>
        /// The result of the operation, which includes all published events and the number of message handlers that were invoked.
        /// </returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>       
        Task<MessageHandlerOperationResult> HandleEventAsync<TEvent>(IMessageHandler<TEvent> messageHandler, TEvent message, CancellationToken? token = null);

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TResponse">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param> 
        /// <param name="token">Optional token that can be used to cancel the operation.</param>                        
        /// <returns>The result that carries the response returned by the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>         
        Task<QueryOperationResult<TResponse>> ExecuteQueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken? token = null);

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">Type of the message going into the query.</typeparam>
        /// <typeparam name="TResponse">Type of the message returned by the query.</typeparam>
        /// <param name="query">The query to execute.</param>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>The result that carries the response returned by the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>         
        Task<QueryOperationResult<TResponse>> ExecuteQueryAsync<TRequest, TResponse>(IQuery<TRequest, TResponse> query, TRequest message, CancellationToken? token = null);        
    }
}
