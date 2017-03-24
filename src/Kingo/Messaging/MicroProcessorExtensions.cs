using System;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{
    /// <summary>
    /// Contains extension methods for the <see cref="IMicroProcessor" /> interface.
    /// </summary>
    public static class MicroProcessorExtensions
    {
        #region [====== Commands & Events ======]

        /// <summary>
        /// Processes the specified message. If <paramref name="handler"/> is not <c>null</c>, this handler will be invoked
        /// instead of any registered handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="processor">A processor.</param>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>
        /// <returns>A stream of events that represents all changes made by this processor.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>               
        /// <exception cref="ExternalProcessorException">
        /// The specified <paramref name="message"/> could not be handled because it was a bad request or because an
        /// internal server error occurred.
        /// </exception>                                     
        public static IMessageStream Handle<TMessage>(this IMicroProcessor processor, TMessage message, Action<TMessage, IMicroProcessorContext> handler) =>
            processor.HandleStream(new MessageStream<TMessage>(message, handler));

        /// <summary>
        /// Processes the specified message. If <paramref name="handler"/> is not <c>null</c>, this handler will be invoked
        /// instead of any registered handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="processor">A processor.</param>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>  
        /// <returns>A stream of events that represents all changes made by this processor.</returns>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ExternalProcessorException">
        /// The specified <paramref name="message"/> could not be handled because it was a bad request or because an
        /// internal server error occurred.
        /// </exception>                                    
        public static IMessageStream Handle<TMessage>(this IMicroProcessor processor, TMessage message, Func<TMessage, IMicroProcessorContext, Task<IMessageStream>> handler) =>
            processor.HandleStream(new MessageStream<TMessage>(message, handler));

        /// <summary>
        /// Processes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="processor">A processor.</param>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>
        /// <returns>A stream of events that represents all changes made by this processor.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>                        
        /// <exception cref="ExternalProcessorException">
        /// The specified <paramref name="message"/> could not be handled because it was a bad request or because an
        /// internal server error occurred.
        /// </exception>                       
        public static IMessageStream Handle<TMessage>(this IMicroProcessor processor, TMessage message, IMessageHandler<TMessage> handler = null) =>
            processor.HandleAsync(message, handler).Await();

        /// <summary>
        /// Processes the specified stream by invoking all appropriate message handlers for each message.
        /// </summary>        
        /// <param name="processor">A processor.</param>
        /// <param name="inputStream">Stream of message to handles.</param>
        /// <returns>A stream of events that represents all changes made by this processor.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="inputStream"/> is <c>null</c>.
        /// </exception>     
        /// <exception cref="ExternalProcessorException">
        /// One of the messages inside the <paramref name="inputStream"/> could not be handled because it was a bad request or because an
        /// internal server error occurred.
        /// </exception>   
        public static IMessageStream HandleStream(this IMicroProcessor processor, IMessageStream inputStream) =>
            processor.HandleStreamAsync(inputStream).Await();

        #endregion        

        #region [====== Commands & Events (Async) ======]  

        /// <summary>
        /// Processes the specified message asynchronously by invoking all handlers that are registered for the specified <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="processor">A processor.</param>
        /// <param name="message">Message to handle.</param>        
        /// <param name="token">Optional token that can be used to cancel the operation.</param> 
        /// <returns>A stream of events that represents all changes made by this processor.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>                                               
        public static Task<IMessageStream> HandleAsync<TMessage>(this IMicroProcessor processor, TMessage message, CancellationToken token) =>
            processor.HandleStreamAsync(new MessageStream<TMessage>(message), token);

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
        /// <returns>A stream of events that represents all changes made by this processor.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>                                               
        public static Task<IMessageStream> HandleAsync<TMessage>(this IMicroProcessor processor, TMessage message, Action<TMessage, IMicroProcessorContext> handler, CancellationToken? token = null) =>
            processor.HandleStreamAsync(new MessageStream<TMessage>(message, handler), token);

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
        /// <returns>A stream of events that represents all changes made by this processor.</returns>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>                                            
        public static Task<IMessageStream> HandleAsync<TMessage>(this IMicroProcessor processor, TMessage message, Func<TMessage, IMicroProcessorContext, Task> handler, CancellationToken? token = null) =>
            processor.HandleStreamAsync(new MessageStream<TMessage>(message, handler), token);

        /// <summary>
        /// Processes the specified message asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="processor">A processor.</param>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param> 
        /// <returns>A stream of events that represents all changes made by this processor.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>                                               
        public static Task<IMessageStream> HandleAsync<TMessage>(this IMicroProcessor processor, TMessage message, IMessageHandler<TMessage> handler = null, CancellationToken? token = null) =>
            processor.HandleStreamAsync(new MessageStream<TMessage>(message, handler), token);

        #endregion

        #region [====== Queries (1) ======]

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>  
        /// <param name="processor">A processor.</param>      
        /// <param name="query">The query to execute.</param>                        
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>  
        /// <exception cref="InternalServerErrorException">
        /// The query could not be executed because an internal server error occurred.
        /// </exception>  
        public static TMessageOut Execute<TMessageOut>(this IMicroProcessor processor, Func<IMicroProcessorContext, TMessageOut> query) =>
            processor.Execute(Query<TMessageOut>.FromDelegate(query));

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>  
        /// <param name="processor">A processor.</param>      
        /// <param name="query">The query to execute.</param>                        
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>  
        /// <exception cref="InternalServerErrorException">
        /// The query could not be executed because an internal server error occurred.
        /// </exception> 
        public static TMessageOut Execute<TMessageOut>(this IMicroProcessor processor, Func<IMicroProcessorContext, Task<TMessageOut>> query) =>
            processor.Execute(Query<TMessageOut>.FromDelegate(query));

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>   
        /// <param name="processor">A processor.</param>     
        /// <param name="query">The query to execute.</param>                        
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>  
        /// <exception cref="InternalServerErrorException">
        /// The query could not be executed because an internal server error occurred.
        /// </exception> 
        public static TMessageOut Execute<TMessageOut>(this IMicroProcessor processor, IQuery<TMessageOut> query) =>
            processor.ExecuteAsync(query).Await();

        #endregion

        #region [====== Queries (1, Async) ======]

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>   
        /// <param name="processor">A processor.</param>     
        /// <param name="query">The query to execute.</param>               
        /// <param name="token">Optional token that can be used to cancel the operation.</param>          
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>  
        public static Task<TMessageOut> ExecuteAsync<TMessageOut>(this IMicroProcessor processor, Func<IMicroProcessorContext, TMessageOut> query, CancellationToken? token = null) =>
            processor.ExecuteAsync(Query<TMessageOut>.FromDelegate(query), token);

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam> 
        /// <param name="processor">A processor.</param>       
        /// <param name="query">The query to execute.</param>               
        /// <param name="token">Optional token that can be used to cancel the operation.</param>          
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>  
        public static Task<TMessageOut> ExecuteAsync<TMessageOut>(this IMicroProcessor processor, Func<IMicroProcessorContext, Task<TMessageOut>> query, CancellationToken? token = null) =>
            processor.ExecuteAsync(Query<TMessageOut>.FromDelegate(query), token);

        #endregion

        #region [====== Queries (2) ======]

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>  
        /// <param name="processor">A processor.</param>      
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="query">The query to execute.</param>                                
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception> 
        /// <exception cref="ExternalProcessorException">
        /// The specified <paramref name="query"/> could not be executed because the specified <paramref name="message"/> represents a bad request
        /// or because because an internal server error occurred.
        /// </exception>                
        public static TMessageOut Execute<TMessageIn, TMessageOut>(this IMicroProcessor processor, TMessageIn message, Func<TMessageIn, IMicroProcessorContext, TMessageOut> query) =>
            processor.Execute(message, Query<TMessageIn, TMessageOut>.FromDelegate(query));

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>  
        /// <param name="processor">A processor.</param>      
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="query">The query to execute.</param>                                
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ExternalProcessorException">
        /// The specified <paramref name="query"/> could not be executed because the specified <paramref name="message"/> represents a bad request
        /// or because because an internal server error occurred.
        /// </exception>            
        public static TMessageOut Execute<TMessageIn, TMessageOut>(this IMicroProcessor processor, TMessageIn message, Func<TMessageIn, IMicroProcessorContext, Task<TMessageOut>> query) =>
            processor.Execute(message, Query<TMessageIn, TMessageOut>.FromDelegate(query));

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>          
        /// <param name="processor">A processor.</param>      
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="query">The query to execute.</param>                                 
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>     
        /// <exception cref="ExternalProcessorException">
        /// The specified <paramref name="query"/> could not be executed because the specified <paramref name="message"/> represents a bad request
        /// or because because an internal server error occurred.
        /// </exception>           
        public static TMessageOut Execute<TMessageIn, TMessageOut>(this IMicroProcessor processor, TMessageIn message, IQuery<TMessageIn, TMessageOut> query) =>
            processor.ExecuteAsync(message, query).Await();

        #endregion

        #region [====== Queries (2, Async) ======]

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>  
        /// <param name="processor">A processor.</param>      
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="query">The query to execute.</param>                                
        /// <param name="token">Optional token that can be used to cancel the operation.</param>                        
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception> 
        public static Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(this IMicroProcessor processor, TMessageIn message, Func<TMessageIn, IMicroProcessorContext, TMessageOut> query, CancellationToken? token = null) =>
            processor.ExecuteAsync(message, Query<TMessageIn, TMessageOut>.FromDelegate(query), token);

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>   
        /// <param name="processor">A processor.</param>     
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="query">The query to execute.</param>                                 
        /// <param name="token">Optional token that can be used to cancel the operation.</param>                        
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception> 
        public static Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(this IMicroProcessor processor, TMessageIn message, Func<TMessageIn, IMicroProcessorContext, Task<TMessageOut>> query, CancellationToken? token = null) =>
            processor.ExecuteAsync(message, Query<TMessageIn, TMessageOut>.FromDelegate(query), token);

        #endregion
    }
}
