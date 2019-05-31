using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a processor that can process commands, events and queries.
    /// </summary>
    public interface IMicroProcessor : IMessageProcessor, IServiceScopeFactory
    {
        /// <summary>
        /// Returns the service provider the processor uses in the current scope to resolve its dependencies.
        /// </summary>
        IServiceProvider ServiceProvider
        {
            get;
        }

        /// <summary>
        /// Processes the specified message asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>        
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>
        /// The result of the operation, which includes all published events and the number of message handlers that were invoked.
        /// </returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="MicroProcessorException">
        /// Something went wrong while handling the message.
        /// </exception> 
        Task<HandleAsyncResult> HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler = null, CancellationToken? token = null);

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
        /// <exception cref="MicroProcessorException">
        /// Something went wrong while executing the query.
        /// </exception>  
        Task<ExecuteAsyncResult<TResponse>> ExecuteAsync<TResponse>(IQuery<TResponse> query, CancellationToken? token = null);

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">Type of the message going into the query.</typeparam>
        /// <typeparam name="TResponse">Type of the message returned by the query.</typeparam>        
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="query">The query to execute.</param>                        
        /// <param name="token">Optional token that can be used to cancel the operation.</param> 
        /// <returns>The result that carries the response returned by the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="MicroProcessorException">
        /// Something went wrong while executing the query.
        /// </exception>  
        Task<ExecuteAsyncResult<TResponse>> ExecuteAsync<TRequest, TResponse>(TRequest message, IQuery<TRequest, TResponse> query, CancellationToken? token = null);        
    }
}
