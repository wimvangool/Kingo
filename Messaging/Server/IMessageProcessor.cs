using System.Threading;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// When implemented by a class, represents a handler of any message.
    /// </summary>
    public interface IMessageProcessor
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

        #region [====== Commands ======]

        /// <summary>
        /// Executes the specified command by invoking all registered message handlers.
        /// </summary>
        /// <typeparam name="TCommand">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>        
        /// <returns>The number of handlers that handled the message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        int Execute<TCommand>(TCommand message) where TCommand : class, IRequestMessage<TCommand>;        

        /// <summary>
        /// Executes the specified command by invoking all registered message handlers.
        /// </summary>
        /// <typeparam name="TCommand">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>        
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param>  
        /// <returns>The number of handlers that handled the message.</returns>
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
        int Execute<TCommand>(TCommand message, CancellationToken? token) where TCommand : class, IRequestMessage<TCommand>;

        /// <summary>
        /// Executes the specified command by invoking the specified handler.
        /// </summary>
        /// <typeparam name="TCommand">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>        
        /// <param name="handler">Executer that will be used to handle the message.</param>
        /// <returns><c>1</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        int Execute<TCommand>(TCommand message, IMessageHandler<TCommand> handler) where TCommand : class, IRequestMessage<TCommand>;

        /// <summary>
        /// Executes the specified command by invoking the specified handler.
        /// </summary>
        /// <typeparam name="TCommand">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>        
        /// <param name="handler">Executer that will be used to handle the message.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param>        
        /// <returns><c>1</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception>         
        int Execute<TCommand>(TCommand message, IMessageHandler<TCommand> handler, CancellationToken? token) where TCommand : class, IRequestMessage<TCommand>;

        /// <summary>
        /// Executes the specified command by invoking the specified delegate.
        /// </summary>
        /// <typeparam name="TCommand">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>        
        /// <param name="handler">Delegate that will be used to handle the message.</param>
        /// <returns><c>1</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        int Execute<TCommand>(TCommand message, Action<TCommand> handler) where TCommand : class, IRequestMessage<TCommand>;

        /// <summary>
        /// Executes the specified command by invoking the specified delegate.
        /// </summary>
        /// <typeparam name="TCommand">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>        
        /// <param name="handler">Delegate that will be used to handle the message.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param>        
        /// <returns><c>1</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception>         
        int Execute<TCommand>(TCommand message, Action<TCommand> handler, CancellationToken? token) where TCommand : class, IRequestMessage<TCommand>;

        #endregion

        #region [====== Queries ======]

        #endregion

        #region [====== Events ======]

        /// <summary>
        /// Processes the specified message by invoking all registered message handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>        
        /// <returns>The number of handlers that handled the message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        int Handle<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>;

        /// <summary>
        /// Processes the specified message by invoking all registered message handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="validator">Optional validator of the message.</param>
        /// <returns>The number of handlers that handled the message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        int Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator) where TMessage : class, IMessage<TMessage>;

        /// <summary>
        /// Processes the specified message by invoking all registered message handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="validator">Optional validator of the message.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param>  
        /// <returns>The number of handlers that handled the message.</returns>
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
        int Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, CancellationToken? token) where TMessage : class, IMessage<TMessage>; 

        /// <summary>
        /// Processes the specified message by invoking the specified handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="validator">Optional validator of the message.</param>
        /// <param name="handler">Handler that will be used to handle the message.</param>
        /// <returns><c>1</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        int Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, IMessageHandler<TMessage> handler) where TMessage : class, IMessage<TMessage>;

        /// <summary>
        /// Processes the specified message by invoking the specified handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="validator">Optional validator of the message.</param>
        /// <param name="handler">Handler that will be used to handle the message.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param>        
        /// <returns><c>1</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception>         
        int Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, IMessageHandler<TMessage> handler, CancellationToken? token) where TMessage : class, IMessage<TMessage>;

        /// <summary>
        /// Processes the specified message by invoking the specified delegate.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="validator">Optional validator of the message.</param>
        /// <param name="handler">Delegate that will be used to handle the message.</param>
        /// <returns><c>1</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        int Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, Action<TMessage> handler) where TMessage : class, IMessage<TMessage>;

        /// <summary>
        /// Processes the specified message by invoking the specified delegate.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="validator">Optional validator of the message.</param>
        /// <param name="handler">Delegate that will be used to handle the message.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param>        
        /// <returns><c>1</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FunctionalException">
        /// The <paramref name="message"/> or the sender of the <paramref name="message"/> did not meet
        /// the preconditions that are in effect for this message to process.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception>         
        int Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, Action<TMessage> handler, CancellationToken? token) where TMessage : class, IMessage<TMessage>;

        #endregion
    }
}
