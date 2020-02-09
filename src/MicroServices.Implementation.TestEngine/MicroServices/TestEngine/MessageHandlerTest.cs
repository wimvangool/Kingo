using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Serves as a base-class for all tests that verify the behavior of a message handler.
    /// </summary>
    public abstract class MessageHandlerTest : MicroProcessorTest
    {
        /// <summary>
        /// Schedules a specific type of message to be executed or handled by a <see cref="IMessageHandler{TMessage}"/>,
        /// of which the behavior will be recorded and verified.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <returns>
        /// The state that can be used to specify the <see cref="IMessageHandler{TMessage}"/> that will be used
        /// to execute or handle the message.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        protected IWhenMessageState<TMessage> When<TMessage>() =>
            State.WhenCommandOrEvent<TMessage>();
    }
}
