using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Kingo.MicroServices.TestEngine.MicroProcessorTestContext;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Contains extension methods for instances of type <see cref="IWhenCommandOrEventState{TMessage}" />.
    /// </summary>
    public static class WhenCommandOrEventStateExtensions
    {
        #region [====== IsExecutedBy ======]

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will execute the command.</param>
        /// <param name="message">Message that will be processed by the <paramref name="messageHandler"/>.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunMessageHandlerTestState<TMessage> IsExecutedBy<TMessage>(this IWhenCommandOrEventState<TMessage> state, Action<TMessage, IMessageHandlerOperationContext> messageHandler, TMessage message) =>
            NotNull(state).IsExecutedBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), ConfigureMessage(message));

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will execute the command.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="messageHandler"/> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunMessageHandlerTestState<TMessage> IsExecutedBy<TMessage>(this IWhenCommandOrEventState<TMessage> state, Action<TMessage, IMessageHandlerOperationContext> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsExecutedBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), configurator);

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will execute the command.</param>
        /// <param name="message">Message that will be processed by the <paramref name="messageHandler"/>.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunMessageHandlerTestState<TMessage> IsExecutedBy<TMessage>(this IWhenCommandOrEventState<TMessage> state, Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler, TMessage message) =>
            NotNull(state).IsExecutedBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), message);

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will execute the command.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>/// 
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="messageHandler"/> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunMessageHandlerTestState<TMessage> IsExecutedBy<TMessage>(this IWhenCommandOrEventState<TMessage> state, Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsExecutedBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), configurator);

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will execute the command.</param>
        /// <param name="message">Message that will be processed by the <paramref name="messageHandler"/>.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunMessageHandlerTestState<TMessage> IsExecutedBy<TMessage>(this IWhenCommandOrEventState<TMessage> state, IMessageHandler<TMessage> messageHandler, TMessage message) =>
            NotNull(state).IsExecutedBy(messageHandler, ConfigureMessage(message));

        #endregion

        #region [====== IsHandledBy ======]

        /// <summary>
        /// Prepares the event to be handled by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <param name="message">Message that will be processed by the <paramref name="messageHandler"/>.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunMessageHandlerTestState<TMessage> IsHandledBy<TMessage>(this IWhenCommandOrEventState<TMessage> state, Action<TMessage, IMessageHandlerOperationContext> messageHandler, TMessage message) =>
            NotNull(state).IsHandledBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), message);

        /// <summary>
        /// Prepares the event to be handled by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="messageHandler"/> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunMessageHandlerTestState<TMessage> IsHandledBy<TMessage>(this IWhenCommandOrEventState<TMessage> state, Action<TMessage, IMessageHandlerOperationContext> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsHandledBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), configurator);

        /// <summary>
        /// Prepares the event to be handled by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <param name="message">Message that will be processed by the <paramref name="messageHandler"/>.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunMessageHandlerTestState<TMessage> IsHandledBy<TMessage>(this IWhenCommandOrEventState<TMessage> state, Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler, TMessage message) =>
            NotNull(state).IsHandledBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), message);

        /// <summary>
        /// Prepares the event to be handled by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="messageHandler"/> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunMessageHandlerTestState<TMessage> IsHandledBy<TMessage>(this IWhenCommandOrEventState<TMessage> state, Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsHandledBy(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), configurator);

        /// <summary>
        /// Prepares the event to be handled by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <param name="message">Message that will be processed by the <paramref name="messageHandler"/>.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunMessageHandlerTestState<TMessage> IsHandledBy<TMessage>(this IWhenCommandOrEventState<TMessage> state, IMessageHandler<TMessage> messageHandler, TMessage message) =>
            NotNull(state).IsHandledBy(messageHandler, ConfigureMessage(message));

        #endregion

        private static IWhenCommandOrEventState<TMessage> NotNull<TMessage>(IWhenCommandOrEventState<TMessage> state) =>
            state ?? throw new ArgumentNullException(nameof(state));
    }
}
