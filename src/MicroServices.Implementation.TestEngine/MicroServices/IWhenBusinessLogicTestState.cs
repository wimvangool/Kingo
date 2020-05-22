using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a state in which the test-engine prepares
    /// to execute a command or handle an event.
    /// </summary>
    public interface IWhenBusinessLogicTestState
    {
        /// <summary>
        /// Schedules a specific command to be executed by a <see cref="IMessageHandler{TMessage}"/>,
        /// of which the behavior will be recorded and verified.
        /// </summary>
        /// <typeparam name="TCommand">Type of the message to handle.</typeparam>
        /// <returns>
        /// The state that can be used to specify the <see cref="IMessageHandler{TMessage}"/> that will be used
        /// to execute or handle the message.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IWhenCommandState<TCommand> Command<TCommand>();

        /// <summary>
        /// Schedules a specific event to be handled by a <see cref="IMessageHandler{TMessage}"/>,
        /// of which the behavior will be recorded and verified.
        /// </summary>
        /// <typeparam name="TEvent">Type of the message to handle.</typeparam>
        /// <returns>
        /// The state that can be used to specify the <see cref="IMessageHandler{TMessage}"/> that will be used
        /// to execute or handle the message.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IWhenEventState<TEvent> Event<TEvent>();
    }
}
