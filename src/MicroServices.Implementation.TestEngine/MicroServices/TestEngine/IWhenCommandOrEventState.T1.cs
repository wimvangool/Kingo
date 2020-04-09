using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents the state in which a command or event is scheduled
    /// to be handled or executed by a <see cref="IMessageHandler{TMessage}" />.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to be handled or executed.</typeparam>
	public interface IWhenCommandOrEventState<TMessage>
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
        IReadyToRunMessageHandlerTestState<TMessage> IsExecutedBy<TMessageHandler>(TMessage message) where TMessageHandler : class, IMessageHandler<TMessage>;

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
        IReadyToRunMessageHandlerTestState<TMessage> IsExecutedBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator)
            where TMessageHandler : class, IMessageHandler<TMessage>;

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
        IReadyToRunMessageHandlerTestState<TMessage> IsExecutedBy(IMessageHandler<TMessage> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator);

        #endregion

        #region [====== IsHandledBy ======]

        /// <summary>
        /// Schedules the event to be handled by the handler or type <typeparamref name="TMessageHandler" />.
        /// </summary>
        /// <typeparam name="TMessageHandler">The message handler that will handle the event.</typeparam>
        /// <param name="message">Message that will be processed by the <typeparamref name="TMessageHandler"/>.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IReadyToRunMessageHandlerTestState<TMessage> IsHandledBy<TMessageHandler>(TMessage message) where TMessageHandler : class, IMessageHandler<TMessage>;

            /// <summary>
        /// Schedules the event to be handled by the handler or type <typeparamref name="TMessageHandler" />.
        /// </summary>
        /// <typeparam name="TMessageHandler">The message handler that will handle the event.</typeparam>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IReadyToRunMessageHandlerTestState<TMessage> IsHandledBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator)
            where TMessageHandler : class, IMessageHandler<TMessage>;

        /// <summary>
        /// Schedules the event to be handled by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler" /> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IReadyToRunMessageHandlerTestState<TMessage> IsHandledBy(IMessageHandler<TMessage> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator);

        #endregion
    }
}
