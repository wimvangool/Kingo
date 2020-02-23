using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents a state in which a setup-operation or -state can be configured.
    /// </summary>
    public interface IGivenState
    {
        /// <summary>
        /// Sets the current date and time to a specific value before or between operations.
        /// </summary>
        /// <param name="value">
        /// The date and time that will represent the current date and time at the start of
        /// the next operation(s).
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is a time in the past relative to the current timeline.
        /// </exception>
        void TimeIs(DateTimeOffset value);

        /// <summary>
        /// Prepares the test-engine to move the clock forward a specific period of time.
        /// </summary>
        /// <param name="value">
        /// A discrete value of time-units, of which the meaning is to be determined by the returned state.
        /// </param>
        /// <returns>
        /// A <see cref="IGivenTimeHasPassedForState" /> that can be used to define exactly what
        /// period should be inserted before or between the operations.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is a negative.
        /// </exception>
        IGivenTimeHasPassedForState TimeHasPassedFor(int value);

        /// <summary>
        /// Moves the clock forward a specified period of time at the start of
        /// the next operation(s).
        /// </summary>
        /// <param name="value">
        /// The period that the clock will be moved forward at the start of the next operation(s).
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is a negative <see cref="TimeSpan" />.
        /// </exception>
        void TimeHasPassed(TimeSpan value);

        /// <summary>
        /// Prepares the specified <typeparamref name="TMessage"/> to be handled or executed by a
        /// <see cref="IMessageHandler{TMessage}" /> during the setup-phase of a test.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <returns>
        /// A <see cref="IGivenCommandOrEventState{TMessage}"/> that can be used to configure which
        /// <see cref="IMessageHandler{TMessage}"/> to use when handling or executing the specified message.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IGivenCommandOrEventState<TMessage> Message<TMessage>();
    }
}
