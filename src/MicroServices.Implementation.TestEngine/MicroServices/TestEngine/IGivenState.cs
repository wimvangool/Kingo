using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents a state in which a setup-operation or -state can be configured.
    /// </summary>
    public interface IGivenState
    {
        #region [====== Time ======]

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
        /// <param name="offset">
        /// The period that the clock will be moved forward at the start of the next operation(s).
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> is a negative <see cref="TimeSpan" />.
        /// </exception>
        void TimeHasPassed(TimeSpan offset);

        #endregion

        #region [====== Commands & Events ======]

        /// <summary>
        /// Prepares the specified <typeparamref name="TCommand"/> to be executed by a
        /// <see cref="IMessageHandler{TMessage}" /> during the setup-phase of a test.
        /// </summary>
        /// <typeparam name="TCommand">Type of the message to handle.</typeparam>
        /// <returns>
        /// A <see cref="IGivenCommandState{TMessage}"/> that can be used to configure which
        /// <see cref="IMessageHandler{TMessage}"/> to use when executing the specified command.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IGivenCommandState<TCommand> Command<TCommand>();

        /// <summary>
        /// Prepares the specified <typeparamref name="TEvent"/> to be handled by a
        /// <see cref="IMessageHandler{TMessage}" /> during the setup-phase of a test.
        /// </summary>
        /// <typeparam name="TEvent">Type of the message to handle.</typeparam>
        /// <returns>
        /// A <see cref="IGivenEventState{TMessage}"/> that can be used to configure which
        /// <see cref="IMessageHandler{TMessage}"/> to use when handling the specified event.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IGivenEventState<TEvent> Event<TEvent>();

        #endregion

        #region [====== Requests ======]

        /// <summary>
        /// Prepares a (void) request to be executed by a
        /// <see cref="IQuery{TResponse}" /> during the setup-phase of a test.
        /// </summary>
        /// <returns>
        /// A <see cref="IGivenRequestState"/> that can be used to configure which
        /// <see cref="IQuery{TResponse}"/> to use when executing the specified request.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IGivenRequestState Request();

        /// <summary>
        /// Prepares the specified <typeparamref name="TRequest"/> to be executed by a
        /// <see cref="IQuery{TRequest, TResponse}" /> during the setup-phase of a test.
        /// </summary>
        /// <typeparam name="TRequest">Type of the message to handle.</typeparam>
        /// <returns>
        /// A <see cref="IGivenRequestState{TRequest}"/> that can be used to configure which
        /// <see cref="IQuery{TRequest, TResponse}"/> to use when executing the specified request.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IGivenRequestState<TRequest> Request<TRequest>();

        #endregion
    }
}
