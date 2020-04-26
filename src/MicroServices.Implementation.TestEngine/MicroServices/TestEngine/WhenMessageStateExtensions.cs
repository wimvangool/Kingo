using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Kingo.MicroServices.TestEngine.MicroProcessorTestContext;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Contains extension methods for instances of type <see cref="IWhenCommandState{TMessage}" /> and <see cref="IWhenEventState{TEvent}" />.
    /// </summary>
    public static class WhenMessageStateExtensions
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
        public static IReadyToRunMessageHandlerTestState<TCommand> IsExecutedBy<TCommand>(this IWhenCommandState<TCommand> state, Action<TCommand, IMessageHandlerOperationContext> messageHandler, TCommand message) =>
            NotNull(state).IsExecutedBy(MessageHandlerDecorator<TCommand>.Decorate(messageHandler), ConfigureMessage(message));

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
        public static IReadyToRunMessageHandlerTestState<TCommand> IsExecutedBy<TCommand>(this IWhenCommandState<TCommand> state, Action<TCommand, IMessageHandlerOperationContext> messageHandler, Action<MessageHandlerTestOperationInfo<TCommand>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsExecutedBy(MessageHandlerDecorator<TCommand>.Decorate(messageHandler), configurator);

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
        public static IReadyToRunMessageHandlerTestState<TCommand> IsExecutedBy<TCommand>(this IWhenCommandState<TCommand> state, Func<TCommand, IMessageHandlerOperationContext, Task> messageHandler, TCommand message) =>
            NotNull(state).IsExecutedBy(MessageHandlerDecorator<TCommand>.Decorate(messageHandler), message);

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
        public static IReadyToRunMessageHandlerTestState<TCommand> IsExecutedBy<TCommand>(this IWhenCommandState<TCommand> state, Func<TCommand, IMessageHandlerOperationContext, Task> messageHandler, Action<MessageHandlerTestOperationInfo<TCommand>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsExecutedBy(MessageHandlerDecorator<TCommand>.Decorate(messageHandler), configurator);

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
        public static IReadyToRunMessageHandlerTestState<TCommand> IsExecutedBy<TCommand>(this IWhenCommandState<TCommand> state, IMessageHandler<TCommand> messageHandler, TCommand message) =>
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
        public static IReadyToRunMessageHandlerTestState<TEvent> IsHandledBy<TEvent>(this IWhenEventState<TEvent> state, Action<TEvent, IMessageHandlerOperationContext> messageHandler, TEvent message) =>
            NotNull(state).IsHandledBy(MessageHandlerDecorator<TEvent>.Decorate(messageHandler), message);

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
        public static IReadyToRunMessageHandlerTestState<TEvent> IsHandledBy<TEvent>(this IWhenEventState<TEvent> state, Action<TEvent, IMessageHandlerOperationContext> messageHandler, Action<MessageHandlerTestOperationInfo<TEvent>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsHandledBy(MessageHandlerDecorator<TEvent>.Decorate(messageHandler), configurator);

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
        public static IReadyToRunMessageHandlerTestState<TEvent> IsHandledBy<TEvent>(this IWhenEventState<TEvent> state, Func<TEvent, IMessageHandlerOperationContext, Task> messageHandler, TEvent message) =>
            NotNull(state).IsHandledBy(MessageHandlerDecorator<TEvent>.Decorate(messageHandler), message);

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
        public static IReadyToRunMessageHandlerTestState<TEvent> IsHandledBy<TEvent>(this IWhenEventState<TEvent> state, Func<TEvent, IMessageHandlerOperationContext, Task> messageHandler, Action<MessageHandlerTestOperationInfo<TEvent>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsHandledBy(MessageHandlerDecorator<TEvent>.Decorate(messageHandler), configurator);

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
        public static IReadyToRunMessageHandlerTestState<TEvent> IsHandledBy<TEvent>(this IWhenEventState<TEvent> state, IMessageHandler<TEvent> messageHandler, TEvent message) =>
            NotNull(state).IsHandledBy(messageHandler, ConfigureMessage(message));

        #endregion

        private static TState NotNull<TState>(TState state) where TState : class =>
            state ?? throw new ArgumentNullException(nameof(state));
    }
}
