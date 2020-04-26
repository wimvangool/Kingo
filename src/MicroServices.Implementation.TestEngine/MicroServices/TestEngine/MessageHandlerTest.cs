using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Serves as a base-class for all tests that verify the behavior of a message handler.
    /// </summary>
    public abstract class MessageHandlerTest : MicroProcessorTest
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
        protected IWhenCommandState<TCommand> WhenCommand<TCommand>() =>
            State.WhenCommand<TCommand>();

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
        protected IWhenEventState<TEvent> WhenEvent<TEvent>() =>
            State.WhenEvent<TEvent>();
    }
}
