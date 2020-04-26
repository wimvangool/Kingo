using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents the state in which an event is scheduled
    /// to be handled by a <see cref="IMessageHandler{TMessage}" />.
    /// </summary>
    /// <typeparam name="TEvent">Type of the message to be handled.</typeparam>
	public interface IWhenEventState<TEvent>
    {
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
        IReadyToRunMessageHandlerTestState<TEvent> IsHandledBy<TMessageHandler>(TEvent message) where TMessageHandler : class, IMessageHandler<TEvent>;

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
        IReadyToRunMessageHandlerTestState<TEvent> IsHandledBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TEvent>, MicroProcessorTestContext> configurator)
            where TMessageHandler : class, IMessageHandler<TEvent>;

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
        IReadyToRunMessageHandlerTestState<TEvent> IsHandledBy(IMessageHandler<TEvent> messageHandler, Action<MessageHandlerTestOperationInfo<TEvent>, MicroProcessorTestContext> configurator);

        #endregion
    }
}
