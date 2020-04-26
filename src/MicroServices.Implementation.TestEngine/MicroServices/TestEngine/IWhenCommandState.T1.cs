using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents the state in which a command is scheduled
    /// to be executed by a <see cref="IMessageHandler{TMessage}" />.
    /// </summary>
    /// <typeparam name="TCommand">Type of the message to be executed.</typeparam>
	public interface IWhenCommandState<TCommand>
    {
        #region [====== IsExecutedBy ======]

        /// <summary>
        /// Schedules the command to be executed by the handler or type <typeparamref name="TMessageHandler" />.
        /// </summary>
        /// <typeparam name="TMessageHandler">The message handler that will execute the command.</typeparam>
        /// <param name="message">Message that will be processed by the <typeparamref name="TMessageHandler"/>.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IReadyToRunMessageHandlerTestState<TCommand> IsExecutedBy<TMessageHandler>(TCommand message) where TMessageHandler : class, IMessageHandler<TCommand>;

        /// <summary>
        /// Schedules the command to be executed by the handler or type <typeparamref name="TMessageHandler" />.
        /// </summary>
        /// <typeparam name="TMessageHandler">The message handler that will execute the command.</typeparam>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IReadyToRunMessageHandlerTestState<TCommand> IsExecutedBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TCommand>, MicroProcessorTestContext> configurator)
            where TMessageHandler : class, IMessageHandler<TCommand>;

        /// <summary>
        /// Schedules the command to be executed by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="messageHandler">The message handler that will execute the command.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler" />, <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IReadyToRunMessageHandlerTestState<TCommand> IsExecutedBy(IMessageHandler<TCommand> messageHandler, Action<MessageHandlerTestOperationInfo<TCommand>, MicroProcessorTestContext> configurator);

        #endregion
    }
}
