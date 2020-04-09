using System;
using System.Threading.Tasks;
using static Kingo.MicroServices.TestEngine.MicroProcessorTestContext;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Contains extensions methods for instances of type <see cref="IGivenCommandOrEventState{TMessage}" />.
    /// </summary>
    public static class GivenCommandOrEventStateExtensions
    {
        #region [====== IsExecutedBy ======]

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will execute the command.</param>
        /// <param name="message">Message that will be processed by the <paramref name="messageHandler"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="messageHandler"/>is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsExecutedBy<TMessage>(this IGivenCommandOrEventState<TMessage> state, Action<TMessage, IMessageHandlerOperationContext> messageHandler, TMessage message) =>
            NotNull(state).IsExecutedBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), message);

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will execute the command.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="messageHandler"/> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsExecutedBy<TMessage>(this IGivenCommandOrEventState<TMessage> state, Action<TMessage, IMessageHandlerOperationContext> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsExecutedBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), configurator);

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will execute the command.</param>
        /// <param name="message">Message that will be processed by the <paramref name="messageHandler"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsExecutedBy<TMessage>(this IGivenCommandOrEventState<TMessage> state, Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler, TMessage message) =>
            NotNull(state).IsExecutedBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), message);

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will execute the command.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="messageHandler"/> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsExecutedBy<TMessage>(this IGivenCommandOrEventState<TMessage> state, Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsExecutedBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), configurator);

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will execute the command.</param>
        /// <param name="message">Message that will be processed by the <paramref name="messageHandler"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsExecutedBy<TMessage>(this IGivenCommandOrEventState<TMessage> state, IMessageHandler<TMessage> messageHandler, TMessage message) =>
            NotNull(state).IsExecutedBy(messageHandler, ToConfigurator(message));

        #endregion

        #region [====== IsHandledBy ======]

        /// <summary>
        /// Prepares the event to be handled by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <param name="message">Message that will be processed by the <paramref name="messageHandler"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="messageHandler"/>is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsHandledBy<TMessage>(this IGivenCommandOrEventState<TMessage> state, Action<TMessage, IMessageHandlerOperationContext> messageHandler, TMessage message) =>
            NotNull(state).IsHandledBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), message);

        /// <summary>
        /// Prepares the event to be handled by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="messageHandler"/> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsHandledBy<TMessage>(this IGivenCommandOrEventState<TMessage> state, Action<TMessage, IMessageHandlerOperationContext> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsHandledBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), configurator);

        /// <summary>
        /// Prepares the event to be handled by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <param name="message">Message that will be processed by the <paramref name="messageHandler"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsHandledBy<TMessage>(this IGivenCommandOrEventState<TMessage> state, Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler, TMessage message) =>
            NotNull(state).IsHandledBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), message);

        /// <summary>
        /// Prepares the event to be handled by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="messageHandler"/> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsHandledBy<TMessage>(this IGivenCommandOrEventState<TMessage> state, Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsHandledBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), configurator);

        /// <summary>
        /// Prepares the event to be handled by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <param name="message">Message that will be processed by the <paramref name="messageHandler"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsHandledBy<TMessage>(this IGivenCommandOrEventState<TMessage> state, IMessageHandler<TMessage> messageHandler, TMessage message) =>
            NotNull(state).IsHandledBy(messageHandler, ToConfigurator(message));

        #endregion

        private static IGivenCommandOrEventState<TMessage> NotNull<TMessage>(IGivenCommandOrEventState<TMessage> state) =>
            state ?? throw new ArgumentNullException(nameof(state));
    }
}
