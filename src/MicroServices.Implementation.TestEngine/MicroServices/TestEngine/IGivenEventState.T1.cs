using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents the state in which a event is scheduled
    /// to be handled by an <see cref="IMessageHandler{TMessage}" />.
    /// </summary>
    /// <typeparam name="TEvent">Type of the message to be handled or executed.</typeparam>
	public interface IGivenEventState<TEvent>
    {
        #region [====== IsHandledBy ======]

        /// <summary>
        /// Prepares the event to be handled by the specified <typeparamref name="TMessageHandler" />.
        /// </summary>
        /// <typeparam name="TMessageHandler">The message handler that will handle the event.</typeparam>
        /// <param name="message">Message that will be processed by the <typeparamref name="TMessageHandler"/>.</param>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        void IsHandledBy<TMessageHandler>(TEvent message) where TMessageHandler : class, IMessageHandler<TEvent>;

        /// <summary>
        /// Prepares the event to be handled by the specified <typeparamref name="TMessageHandler" />.
        /// </summary>
        /// <typeparam name="TMessageHandler">The message handler that will handle the event.</typeparam>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        void IsHandledBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TEvent>, MicroProcessorTestContext> configurator)
            where TMessageHandler : class, IMessageHandler<TEvent>;

        /// <summary>
        /// Prepares the event to be handled by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        void IsHandledBy(IMessageHandler<TEvent> messageHandler, Action<MessageHandlerTestOperationInfo<TEvent>, MicroProcessorTestContext> configurator);

        #endregion
    }
}
