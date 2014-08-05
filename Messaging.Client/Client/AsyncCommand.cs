using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="IRequestDispatcherCommand" /> interface.
    /// </summary>  
    public class AsyncCommand : AsyncCommand<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{T}" /> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>             
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public AsyncCommand(IRequestDispatcher dispatcher)
            : base(dispatcher) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{T}" /> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>
        /// <param name="isValidIndicator">
        /// The indicator that is used to determine whether the command can be executed.
        /// </param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public AsyncCommand(IRequestDispatcher dispatcher, IIsValidIndicator isValidIndicator)
            : base(dispatcher, isValidIndicator) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{T}" /> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>
        /// <param name="isValidIndicator">
        /// The indicator that is used to determine whether the command can be executed.
        /// </param>
        /// <param name="options">
        /// The opions that determine the exact behavior of this command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public AsyncCommand(IRequestDispatcher dispatcher, IIsValidIndicator isValidIndicator, AsyncCommandOptions options)
            : base(dispatcher, isValidIndicator, options) { }

        protected override bool TryConvertParameter(object parameterIn, out object parameterOut)
        {
            parameterOut = parameterIn;
            return true;
        }

        #region [====== Command Factory Methods - CommandDispatcher ======]

        /// <summary>
        /// Creates and returns a new <see cref="AsyncCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is sent by the dispatcher.</typeparam>
        /// <typeparam name="TParameter">Type of the parameter of the command.</typeparam>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>        
        /// <returns>A new <see cref="AsyncCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static AsyncCommand<TParameter> CreateCommand<TMessage, TParameter>(CommandDispatcher<TMessage> dispatcher) where TMessage : class, IMessage<TMessage>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new AsyncCommand<TParameter>(dispatcher, dispatcher.Message);
        }

        /// <summary>
        /// Creates and returns a new <see cref="AsyncCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is sent by the dispatcher.</typeparam>
        /// <typeparam name="TParameter">Type of the parameter of the command.</typeparam>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>
        /// <param name="options">The opions that determine the exact behavior of this command.</param>
        /// <returns>A new <see cref="AsyncCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static AsyncCommand<TParameter> CreateCommand<TMessage, TParameter>(CommandDispatcher<TMessage> dispatcher, AsyncCommandOptions options) where TMessage : class, IMessage<TMessage>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new AsyncCommand<TParameter>(dispatcher, dispatcher.Message, options);
        }

        /// <summary>
        /// Creates and returns a new <see cref="AsyncCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is sent by the dispatcher.</typeparam>        
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>
        /// <param name="options">The opions that determine the exact behavior of this command.</param>
        /// <returns>A new <see cref="AsyncCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static AsyncCommand CreateCommand<TMessage>(CommandDispatcher<TMessage> dispatcher, AsyncCommandOptions options) where TMessage : class, IMessage<TMessage>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new AsyncCommand(dispatcher, dispatcher.Message, options);
        }

        /// <summary>
        /// Creates and returns a new <see cref="AsyncCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is sent by the dispatcher.</typeparam>        
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>        
        /// <returns>A new <see cref="AsyncCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static AsyncCommand CreateCommand<TMessage>(CommandDispatcher<TMessage> dispatcher) where TMessage : class, IMessage<TMessage>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new AsyncCommand(dispatcher, dispatcher.Message);
        }

        #endregion

        #region [====== Command Factory Methods - QueryDispatcher ======]

        /// <summary>
        /// Creates and returns a new <see cref="AsyncCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is sent by the dispatcher.</typeparam>
        /// <typeparam name="TResult">Type of the result of the query.</typeparam>
        /// <typeparam name="TParameter">Type of the parameter of the command.</typeparam>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>        
        /// <returns>A new <see cref="AsyncCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static AsyncCommand<TParameter> CreateCommand<TMessage, TResult, TParameter>(QueryDispatcher<TMessage, TResult> dispatcher) where TMessage : class, IMessage<TMessage>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new AsyncCommand<TParameter>(dispatcher, dispatcher.Message);
        }

        /// <summary>
        /// Creates and returns a new <see cref="AsyncCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is sent by the dispatcher.</typeparam>
        /// <typeparam name="TResult">Type of the result of the query.</typeparam>
        /// <typeparam name="TParameter">Type of the parameter of the command.</typeparam>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>
        /// <param name="options">The opions that determine the exact behavior of this command.</param>
        /// <returns>A new <see cref="AsyncCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static AsyncCommand<TParameter> CreateCommand<TMessage, TResult, TParameter>(QueryDispatcher<TMessage, TResult> dispatcher, AsyncCommandOptions options) where TMessage : class, IMessage<TMessage>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new AsyncCommand<TParameter>(dispatcher, dispatcher.Message, options);
        }

        /// <summary>
        /// Creates and returns a new <see cref="AsyncCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is sent by the dispatcher.</typeparam>      
        /// <typeparam name="TResult">Type of the result of the query.</typeparam>  
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>
        /// <param name="options">The opions that determine the exact behavior of this command.</param>
        /// <returns>A new <see cref="AsyncCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static AsyncCommand CreateCommand<TMessage, TResult>(QueryDispatcher<TMessage, TResult> dispatcher, AsyncCommandOptions options) where TMessage : class, IMessage<TMessage>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new AsyncCommand(dispatcher, dispatcher.Message, options);
        }

        /// <summary>
        /// Creates and returns a new <see cref="AsyncCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is sent by the dispatcher.</typeparam>    
        /// <typeparam name="TResult">Type of the result of the query.</typeparam>    
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>        
        /// <returns>A new <see cref="AsyncCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static AsyncCommand CreateCommand<TMessage, TResult>(QueryDispatcher<TMessage, TResult> dispatcher) where TMessage : class, IMessage<TMessage>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new AsyncCommand(dispatcher, dispatcher.Message);
        }

        #endregion
    }
}
