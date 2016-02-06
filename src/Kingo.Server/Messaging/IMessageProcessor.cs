﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a handler of any message.
    /// </summary>
    public interface IMessageProcessor
    {
        /// <summary>
        /// Returns the <see cref="IMessageProcessorBus" /> of this processor.
        /// </summary>
        IMessageProcessorBus EventBus
        {
            get;
        }

        /// <summary>
        /// Creates and returns a new <see cref="UnitOfWorkScope" /> that is associated to this processor.
        /// </summary>
        /// <returns>A new <see cref="UnitOfWorkScope" />.</returns>
        UnitOfWorkScope CreateUnitOfWorkScope();

        #region [====== Commands & Events ======]

        /// <summary>
        /// Processes the specified message by invoking all registered message handlers.
        /// </summary>        
        /// <param name="message">Message to handle.</param>                        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        void Handle(object message);

        /// <summary>
        /// Processes the specified message by invoking all registered message handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>                        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>              
        void Handle<TMessage>(TMessage message) where TMessage : class, IMessage;

        /// <summary>
        /// Processes the specified message by invoking the specified handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>                        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>                
        void Handle<TMessage>(TMessage message, Action<TMessage> handler) where TMessage : class, IMessage;

        /// <summary>
        /// Processes the specified message by invoking the specified handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>                        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>               
        void Handle<TMessage>(TMessage message, Func<TMessage, Task> handler) where TMessage : class, IMessage;

        /// <summary>
        /// Processes the specified message by invoking the specified handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>                       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>               
        void Handle<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class, IMessage;

        /// <summary>
        /// Processes the specified message by invoking all registered message handlers asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>                        
        /// <returns>The <see cref="Task" /> that is handling the <paramref name="message"/>.</returns>               
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task HandleAsync<TMessage>(TMessage message) where TMessage : class, IMessage;

        /// <summary>
        /// Processes the specified message by invoking all registered message handlers asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>                
        /// <param name="token">Optional token that can be used to cancel the operation.</param>  
        /// <returns>The <see cref="Task" /> that is handling the <paramref name="message"/>.</returns>               
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task HandleAsync<TMessage>(TMessage message, CancellationToken token) where TMessage : class, IMessage;

        /// <summary>
        /// Processes the specified message by invoking the specified handler asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>                      
        /// <returns>The <see cref="Task" /> that is handling the <paramref name="message"/>.</returns>               
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task HandleAsync<TMessage>(TMessage message, Action<TMessage> handler) where TMessage : class, IMessage;

        /// <summary>
        /// Processes the specified message by invoking the specified handler asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>              
        /// <param name="token">Optional token that can be used to cancel the operation.</param>  
        /// <returns>The <see cref="Task" /> that is handling the <paramref name="message"/>.</returns>               
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task HandleAsync<TMessage>(TMessage message, Action<TMessage> handler, CancellationToken token) where TMessage : class, IMessage;

        /// <summary>
        /// Processes the specified message by invoking the specified handler asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>                      
        /// <returns>The <see cref="Task" /> that is handling the <paramref name="message"/>.</returns>               
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task HandleAsync<TMessage>(TMessage message, Func<TMessage, Task> handler) where TMessage : class, IMessage;

        /// <summary>
        /// Processes the specified message by invoking the specified handler asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>              
        /// <param name="token">Optional token that can be used to cancel the operation.</param>  
        /// <returns>The <see cref="Task" /> that is handling the <paramref name="message"/>.</returns>               
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task HandleAsync<TMessage>(TMessage message, Func<TMessage, Task> handler, CancellationToken token) where TMessage : class, IMessage;

        /// <summary>
        /// Processes the specified message by invoking the specified handler asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>                      
        /// <returns>The <see cref="Task" /> that is handling the <paramref name="message"/>.</returns>               
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>  
        Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class, IMessage;

        /// <summary>
        /// Processes the specified message by invoking the specified handler asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">
        /// Optional handler that will be used to handle the message.
        /// If <c>null</c>, the processor will attempt to resolve any registered handlers for the specified <paramref name="message"/>.
        /// </param>              
        /// <param name="token">Optional token that can be used to cancel the operation.</param>  
        /// <returns>The <see cref="Task" /> that is handling the <paramref name="message"/>.</returns>               
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>  
        Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler, CancellationToken token) where TMessage : class, IMessage;

        #endregion

        #region [====== Queries ======]

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param>                        
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>  
        TMessageOut Execute<TMessageOut>(Func<TMessageOut> query) where TMessageOut : class, IMessage;

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param>                        
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>  
        TMessageOut Execute<TMessageOut>(IQuery<TMessageOut> query) where TMessageOut : class, IMessage;

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param>                        
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>                
        TMessageOut Execute<TMessageIn, TMessageOut>(Func<TMessageIn, TMessageOut> query, TMessageIn message)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage;

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param>                        
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>               
        TMessageOut Execute<TMessageIn, TMessageOut>(Func<TMessageIn, Task<TMessageOut>> query, TMessageIn message)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage;

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param>                          
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>               
        TMessageOut Execute<TMessageIn, TMessageOut>(IQuery<TMessageIn, TMessageOut> query, TMessageIn message)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage;

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param>                        
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>  
        Task<TMessageOut> ExecuteAsync<TMessageOut>(Func<Task<TMessageOut>> query) where TMessageOut : class, IMessage;

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param>               
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param>          
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>  
        Task<TMessageOut> ExecuteAsync<TMessageOut>(Func<Task<TMessageOut>> query, CancellationToken token) where TMessageOut : class, IMessage;

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param>                        
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>  
        Task<TMessageOut> ExecuteAsync<TMessageOut>(IQuery<TMessageOut> query) where TMessageOut : class, IMessage;

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param> 
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param>                        
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>  
        Task<TMessageOut> ExecuteAsync<TMessageOut>(IQuery<TMessageOut> query, CancellationToken token) where TMessageOut : class, IMessage;

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param>                         
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(Func<TMessageIn, TMessageOut> query, TMessageIn message)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage;

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param>                
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param> 
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(Func<TMessageIn, TMessageOut> query, TMessageIn message, CancellationToken token)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage;

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param>                         
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(Func<TMessageIn, Task<TMessageOut>> query, TMessageIn message)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage;

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param>                
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param> 
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(Func<TMessageIn, Task<TMessageOut>> query, TMessageIn message, CancellationToken token)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage;

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="query">The query to execute.</param>                        
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(IQuery<TMessageIn, TMessageOut> query, TMessageIn message)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage;

        /// <summary>
        /// Executes the specified <paramref name="query"/> using the specified <paramref name="message"/> and returns its result asynchronously.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message going into the query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message returned by the query.</typeparam>        
        /// <param name="query">The query to execute.</param>                
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param> 
        /// <returns>The <see cref="Task{TMessageOut}" /> that is executing the <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(IQuery<TMessageIn, TMessageOut> query, TMessageIn message, CancellationToken token)
            where TMessageIn : class, IMessage
            where TMessageOut : class, IMessage;
       
        #endregion        
    }
}
