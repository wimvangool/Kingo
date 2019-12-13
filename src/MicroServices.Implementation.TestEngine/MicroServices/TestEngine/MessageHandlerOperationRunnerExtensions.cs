using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Contains extensions methods for instances that implement the <see cref="IMessageHandlerOperationRunner{TMessage}" /> interface.
    /// </summary>
    public static class MessageHandlerOperationRunnerExtensions
    {
        #region [====== ExecuteCommandAsync ======]

        /// <summary>
        /// Executes the specified <paramref name="command"/> using the specified <paramref name="messageHandler" />
        /// </summary>
        /// <typeparam name="TCommand">Type of the command to execute.</typeparam>
        /// <param name="runner">The runner that will run the operation.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="messageHandler">The message-handler that will to handle the message with inside the processor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="runner"/>, <paramref name="messageHandler"/> or <paramref name="command"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteCommandAsync<TCommand>(this IMessageHandlerOperationRunner<TCommand> runner, Action<TCommand, IMessageHandlerOperationContext> messageHandler, TCommand command) =>
            NotNull(runner).ExecuteCommandAsync(MessageHandlerDecorator<TCommand>.Decorate(messageHandler), command);

        /// <summary>
        /// Executes the specified <paramref name="command"/> using the specified <paramref name="messageHandler" />
        /// </summary>
        /// <typeparam name="TCommand">Type of the command to execute.</typeparam>
        /// <param name="runner">The runner that will run the operation.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="messageHandler">The message-handler that will to handle the message with inside the processor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="runner"/>, <paramref name="messageHandler"/> or <paramref name="command"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteCommandAsync<TCommand>(this IMessageHandlerOperationRunner<TCommand> runner, Func<TCommand, IMessageHandlerOperationContext, Task> messageHandler, TCommand command) =>
            NotNull(runner).ExecuteCommandAsync(MessageHandlerDecorator<TCommand>.Decorate(messageHandler), command);

        #endregion

        #region [====== HandleEventAsync ======]

        /// <summary>
        /// Handles the specified <paramref name="event"/> using the specified <paramref name="messageHandler" />
        /// </summary>
        /// <typeparam name="TEvent">Type of the event to handle.</typeparam>
        /// <param name="runner">The runner that will run the operation.</param>
        /// <param name="event">The event to handle.</param>
        /// <param name="messageHandler">The message-handler that will to handle the message with inside the processor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="runner"/>, <paramref name="messageHandler"/> or <paramref name="event"/> is <c>null</c>.
        /// </exception>
        public static Task HandleEventAsync<TEvent>(this IMessageHandlerOperationRunner<TEvent> runner, Action<TEvent, IMessageHandlerOperationContext> messageHandler, TEvent @event) =>
            NotNull(runner).HandleEventAsync(MessageHandlerDecorator<TEvent>.Decorate(messageHandler), @event);

        /// <summary>
        /// Handles the specified <paramref name="event"/> using the specified <paramref name="messageHandler" />
        /// </summary>
        /// <typeparam name="TEvent">Type of the event to handle.</typeparam>
        /// <param name="runner">The runner that will run the operation.</param>
        /// <param name="event">The event to handle.</param>
        /// <param name="messageHandler">The message-handler that will to handle the message with inside the processor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="runner"/>, <paramref name="messageHandler"/> or <paramref name="event"/> is <c>null</c>.
        /// </exception>
        public static Task HandleEventAsync<TEvent>(this IMessageHandlerOperationRunner<TEvent> runner, Func<TEvent, IMessageHandlerOperationContext, Task> messageHandler, TEvent @event) =>
            NotNull(runner).HandleEventAsync(MessageHandlerDecorator<TEvent>.Decorate(messageHandler), @event);

        #endregion

        private static IMessageHandlerOperationRunner<TMessage> NotNull<TMessage>(IMessageHandlerOperationRunner<TMessage> runner) =>
            runner ?? throw new ArgumentNullException(nameof(runner));
    }
}
