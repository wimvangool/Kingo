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
        #region [====== HandleAsync ======]                  

        /// <summary>
        /// Processes the specified message asynchronously by invoking all handlers that are registered for the specified <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="processor">A processor.</param>
        /// <param name="message">Message to handle.</param>        
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>
        /// The total number of message handler invocations that took place to handle the specified <paramref name="message"/>
        /// </returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="MicroProcessorException">
        /// Something went wrong while handling the message.
        /// </exception>  
        public static Task<int> HandleAsync<TMessage>(this IMicroProcessor processor, TMessage message, CancellationToken? token = null) =>
            processor.HandleAsync(message, null, token);

        /// <summary>
        /// Processes the specified message asynchronously. If <paramref name="handler"/> is not <c>null</c>, this handler will be invoked
        /// instead of any registered handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="processor">A processor.</param>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>
        /// The total number of message handler invocations that took place to handle the specified <paramref name="message"/>
        /// </returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="MicroProcessorException">
        /// Something went wrong while handling the message.
        /// </exception>  
        public static Task<int> HandleAsync<TMessage>(this IMicroProcessor processor, TMessage message, Action<TMessage, MessageHandlerContext> handler, CancellationToken? token = null) =>
            processor.HandleAsync(message, MessageHandlerDecorator<TMessage>.Decorate(handler), token);

        /// <summary>
        /// Processes the specified message asynchronously. If <paramref name="handler"/> is not <c>null</c>, this handler will be invoked
        /// instead of any registered handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="processor">A processor.</param>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param> 
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>
        /// The total number of message handler invocations that took place to handle the specified <paramref name="message"/>
        /// </returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="MicroProcessorException">
        /// Something went wrong while handling the message.
        /// </exception>  
        public static Task<int> HandleAsync<TMessage>(this IMicroProcessor processor, TMessage message, Func<TMessage, MessageHandlerContext, Task> handler, CancellationToken? token = null) =>
            processor.HandleAsync(message, MessageHandlerDecorator<TMessage>.Decorate(handler), token);

        #endregion

        #region [====== ExecuteAsync (1) ======]

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TResponse">Type of the message returned by the query.</typeparam>   
        /// <param name="processor">A processor.</param>     
        /// <param name="query">The query to execute.</param>               
        /// <param name="token">Optional token that can be used to cancel the operation.</param>          
        /// <returns>The <see cref="Task{TResponse}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="MicroProcessorException">
        /// Something went wrong while executing the query.
        /// </exception>  
        public static Task<TResponse> ExecuteAsync<TResponse>(this IMicroProcessor processor, Func<QueryContext, TResponse> query, CancellationToken? token = null) =>
            processor.ExecuteAsync(QueryDecorator<TResponse>.Decorate(query), token);

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TResponse">Type of the message returned by the query.</typeparam> 
        /// <param name="processor">A processor.</param>       
        /// <param name="query">The query to execute.</param>               
        /// <param name="token">Optional token that can be used to cancel the operation.</param>          
        /// <returns>The <see cref="Task{TResponse}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="MicroProcessorException">
        /// Something went wrong while executing the query.
        /// </exception>  
        public static Task<TResponse> ExecuteAsync<TResponse>(this IMicroProcessor processor, Func<QueryContext, Task<TResponse>> query, CancellationToken? token = null) =>
            processor.ExecuteAsync(QueryDecorator<TResponse>.Decorate(query), token);

        #endregion        

        #region [====== ExecuteAsync (2) ======]

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">Type of the message going into the query.</typeparam>
        /// <typeparam name="TResponse">Type of the message returned by the query.</typeparam>  
        /// <param name="processor">A processor.</param>      
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="query">The query to execute.</param>                                
        /// <param name="token">Optional token that can be used to cancel the operation.</param>                        
        /// <returns>The <see cref="Task{TResponse}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="MicroProcessorException">
        /// Something went wrong while executing the query.
        /// </exception>  
        public static Task<TResponse> ExecuteAsync<TRequest, TResponse>(this IMicroProcessor processor, TRequest message, Func<TRequest, QueryContext, TResponse> query, CancellationToken? token = null) =>
            processor.ExecuteAsync(message, QueryDecorator<TRequest, TResponse>.Decorate(query), token);

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">Type of the message going into the query.</typeparam>
        /// <typeparam name="TResponse">Type of the message returned by the query.</typeparam>   
        /// <param name="processor">A processor.</param>     
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="query">The query to execute.</param>                                 
        /// <param name="token">Optional token that can be used to cancel the operation.</param>                        
        /// <returns>The <see cref="Task{TResponse}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="MicroProcessorException">
        /// Something went wrong while executing the query.
        /// </exception>  
        public static Task<TResponse> ExecuteAsync<TRequest, TResponse>(this IMicroProcessor processor, TRequest message, Func<TRequest, QueryContext, Task<TResponse>> query, CancellationToken? token = null) =>
            processor.ExecuteAsync(message, QueryDecorator<TRequest, TResponse>.Decorate(query), token);

        #endregion
    }
}
