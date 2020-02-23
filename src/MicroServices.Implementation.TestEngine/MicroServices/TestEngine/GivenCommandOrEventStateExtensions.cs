using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Contains extensions methods for instances of type <see cref="IGivenCommandOrEventState{TMessage}" />.
    /// </summary>
    public static class GivenCommandOrEventStateExtensions
    {
        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <param name="messageHandler">The message handler that will execute the command.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="configurator"/> or <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsExecutedByCommandHandler<TMessage>(this IGivenCommandOrEventState<TMessage> state, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator, Action<TMessage, IMessageHandlerOperationContext> messageHandler) =>
            NotNull(state).IsExecutedByCommandHandler(configurator, MessageHandlerDecorator<TMessage>.Decorate(messageHandler));

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <param name="messageHandler">The message handler that will execute the command.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="configurator"/> or <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsExecutedByCommandHandler<TMessage>(this IGivenCommandOrEventState<TMessage> state, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator, Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler) =>
            NotNull(state).IsExecutedByCommandHandler(configurator, MessageHandlerDecorator<TMessage>.Decorate(messageHandler));

        /// <summary>
        /// Prepares the event to be handled by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="configurator"/> or <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsHandledByEventHandler<TMessage>(this IGivenCommandOrEventState<TMessage> state, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator, Action<TMessage, IMessageHandlerOperationContext> messageHandler) =>
            NotNull(state).IsHandledByEventHandler(configurator, MessageHandlerDecorator<TMessage>.Decorate(messageHandler));

        /// <summary>
        /// Prepares the event to be handled by the specified <paramref name="messageHandler" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <param name="messageHandler">The message handler that will handle the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="configurator"/> or <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsHandledByEventHandler<TMessage>(this IGivenCommandOrEventState<TMessage> state, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator, Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler) =>
            NotNull(state).IsHandledByEventHandler(configurator, MessageHandlerDecorator<TMessage>.Decorate(messageHandler));

        private static IGivenCommandOrEventState<TMessage> NotNull<TMessage>(IGivenCommandOrEventState<TMessage> state) =>
            state ?? throw new ArgumentNullException(nameof(state));
    }
}
