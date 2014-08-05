using System.Threading;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// When implemented by a class, represents a handler of any message.
    /// </summary>
    public interface IMessageProcessor
    {
        /// <summary>
        /// Returns the domain-event bus of this processor.
        /// </summary>
        IMessageProcessorBus DomainEventBus
        {
            get;
        }

        /// <summary>
        /// Returns the message that is currently being handled by the processor.
        /// </summary>
        UseCase CurrentUseCase
        {
            get;
        }

        /// <summary>
        /// Processes the specified message by invoking all registered message handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="RequestExecutionException">
        /// The specified <paramref name="message"/> represents a command and failed for (somewhat) predictable reasons,
        /// like insufficient rights, invalid parameters or because the system's state/business rules wouldn't allow this
        /// command to be executed.
        /// </exception>
        void Process<TMessage>(TMessage message) where TMessage : class;

        /// <summary>
        /// Processes the specified message by invoking all registered external message handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param>
        /// <param name="reporter">
        /// Reporter that can be used to report the progress.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception> 
        /// <exception cref="RequestExecutionException">
        /// The specified <paramref name="message"/> represents a command and failed for (somewhat) predictable reasons,
        /// like insufficient rights, invalid parameters or because the system's state/business rules wouldn't allow this
        /// command to be executed.
        /// </exception>
        void Process<TMessage>(TMessage message, CancellationToken? token, IProgressReporter reporter) where TMessage : class;

        /// <summary>
        /// Processes the specified message by invoking the specified handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">Handler that will be used to handle the message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="RequestExecutionException">
        /// The specified <paramref name="message"/> represents a command and failed for (somewhat) predictable reasons,
        /// like insufficient rights, invalid parameters or because the system's state/business rules wouldn't allow this
        /// command to be executed.
        /// </exception>
        void Process<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class;

        /// <summary>
        /// Processes the specified message by invoking the specified handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">Handler that will be used to handle the message.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param>
        /// <param name="reporter">
        /// Reporter that can be used to report the progress.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception> 
        /// <exception cref="RequestExecutionException">
        /// The specified <paramref name="message"/> represents a command and failed for (somewhat) predictable reasons,
        /// like insufficient rights, invalid parameters or because the system's state/business rules wouldn't allow this
        /// command to be executed.
        /// </exception>
        void Process<TMessage>(TMessage message, IMessageHandler<TMessage> handler, CancellationToken? token, IProgressReporter reporter) where TMessage : class;

        /// <summary>
        /// Processes the specified message by invoking the specified delegate.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="action">Delegate that will be used to handle the message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="action"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="RequestExecutionException">
        /// The specified <paramref name="message"/> represents a command and failed for (somewhat) predictable reasons,
        /// like insufficient rights, invalid parameters or because the system's state/business rules wouldn't allow this
        /// command to be executed.
        /// </exception>
        void Process<TMessage>(TMessage message, Action<TMessage> action) where TMessage : class;

        /// <summary>
        /// Processes the specified message by invoking the specified delegate.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="action">Delegate that will be used to handle the message.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the operation.
        /// </param>
        /// <param name="reporter">
        /// Reporter that can be used to report the progress.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="action"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception> 
        /// <exception cref="RequestExecutionException">
        /// The specified <paramref name="message"/> represents a command and failed for (somewhat) predictable reasons,
        /// like insufficient rights, invalid parameters or because the system's state/business rules wouldn't allow this
        /// command to be executed.
        /// </exception>
        void Process<TMessage>(TMessage message, Action<TMessage> action, CancellationToken? token, IProgressReporter reporter) where TMessage : class;
    }
}
