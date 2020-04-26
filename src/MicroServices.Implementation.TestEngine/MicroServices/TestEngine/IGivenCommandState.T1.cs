using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents the state in which a command is scheduled
    /// to be executed by an <see cref="IMessageHandler{TMessage}" />.
    /// </summary>
    /// <typeparam name="TCommand">Type of the message to be handled or executed.</typeparam>
	public interface IGivenCommandState<TCommand>
    {
        #region [====== IsExecutedBy ======]

        /// <summary>
        /// Prepares the command to be executed by the specified <typeparamref name="TMessageHandler" />.
        /// </summary>
        /// <typeparam name="TMessageHandler">The message handler that will execute the command.</typeparam>
        /// <param name="message">Message that will be processed by the <typeparamref name="TMessageHandler"/>.</param>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        void IsExecutedBy<TMessageHandler>(TCommand message) where TMessageHandler : class, IMessageHandler<TCommand>;

        /// <summary>
        /// Prepares the command to be executed by the specified <typeparamref name="TMessageHandler" />.
        /// </summary>
        /// <typeparam name="TMessageHandler">The message handler that will execute the command.</typeparam>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        void IsExecutedBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TCommand>, MicroProcessorTestContext> configurator)
            where TMessageHandler : class, IMessageHandler<TCommand>;

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="messageHandler">The message handler that will execute the command.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        void IsExecutedBy(IMessageHandler<TCommand> messageHandler, Action<MessageHandlerTestOperationInfo<TCommand>, MicroProcessorTestContext> configurator);

        #endregion
    }
}
