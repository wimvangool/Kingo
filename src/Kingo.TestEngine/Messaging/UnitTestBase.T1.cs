using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a scenario that is used to test the business logic of a service or application.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is processed on the When-phase.</typeparam>    
    public abstract class UnitTestBase<TMessage> : Scenario<IUnitTestResult>
    {
        #region [====== Executable ======]

        private sealed class Executable : ScenarioResult<IMessageStream>, IUnitTestResult
        {
            private readonly UnitTestBase<TMessage> _scenario;

            public Executable(UnitTestBase<TMessage> scenario)
            {
                _scenario = scenario;
            }                        

            public async Task IsEventStreamAsync(int expectedEventCount, Action<IReadOnlyList<object>> assertCallback)
            {
                if (expectedEventCount < 0)
                {
                    throw NewInvalidEventCountException(expectedEventCount);
                }
                await _scenario.SetupAsync();

                try
                {
                    var outputStream = await ExecuteScenarioAsync();
                    if (outputStream.Count == expectedEventCount)
                    {
                        assertCallback?.Invoke(outputStream);
                        return;
                    }
                    throw NewUnexpectedEventCountException(expectedEventCount, outputStream.Count);
                }
                finally
                {
                    await _scenario.TearDownAsync();
                }                               
            }

            protected override Task SetupScenarioAsync() =>
                _scenario.SetupAsync();

            protected override Task<IMessageStream> ExecuteScenarioAsync() =>
                _scenario.Processor.HandleAsync(_scenario.WhenMessageIsHandled(), _scenario.CreateMessageHandler());

            protected override Task TearDownScenarioAsync() =>
                _scenario.TearDownAsync();            

            private static Exception NewInvalidEventCountException(int expectedEventCount)
            {
                var messageFormat = ExceptionMessages.WriteScenario_InvalidEventCount;
                var message = string.Format(messageFormat, expectedEventCount);
                return new ArgumentOutOfRangeException(nameof(expectedEventCount), message);
            }

            private Exception NewUnexpectedEventCountException(int expectedEventCount, int actualEventCount)
            {
                var messageFormat = ExceptionMessages.WriteScenario_UnexpectedEventCount;
                var message = string.Format(messageFormat, expectedEventCount, actualEventCount);
                return NewAssertFailedException(message, null);
            }

            protected override Exception NewAssertFailedException(string message, Exception exception) =>
                _scenario.NewAssertFailedException(message, exception);
        }

        #endregion

        internal override IUnitTestResult CreateResult() =>
            new Executable(this);

        internal override IMessageStream CreateStream() =>
            Given().Join().Append(WhenMessageIsHandled(), CreateMessageHandler());

        #region [====== Given / When / Then ======]          

        /// <summary>
        /// Creates and returns the message that will be handled by the processor and of which the results will be verified
        /// by the <see cref="Scenario{T}.Result"/>.
        /// </summary>
        /// <returns>The message that will be handled by the processor.</returns>
        protected abstract TMessage WhenMessageIsHandled();

        /// <summary>
        /// Creates and returns a <see cref="IMessageHandler{T}" /> that will be invoked by the processor to handle the message.
        /// By default, this method returns <c>null</c> so that the processor will attempt to resolve any registered handlers.
        /// </summary>
        /// <returns>
        /// A <see cref="IMessageHandler{T}" /> that will be invoked by the processor to handle the message; <c>null</c> by default.
        /// </returns>
        protected virtual IMessageHandler<TMessage> CreateMessageHandler() =>
            null;

        #endregion

        #region [====== Assertion ======]        

        /// <summary>
        /// Asserts that the specified <paramref name="stream"/> contains an event of the specified type <typeparamref name="TEvent"/>
        /// at the specified <paramref name="index"/>, and invokes the specified <paramref name="assertCallback"/> if it does.
        /// </summary>
        /// <typeparam name="TEvent">Type of the expected event.</typeparam>
        /// <param name="stream">An event stream.</param>
        /// <param name="index">Index at which the event is expected.</param>
        /// <param name="assertCallback">
        /// Callback that can be used to assert the properties of the event.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> is out of range of valid values for the specified <paramref name="stream"/>.
        /// </exception>
        protected void AssertEvent<TEvent>(IReadOnlyList<object> stream, int index, Action<TEvent> assertCallback)
        {            
            if (assertCallback == null)
            {
                throw new ArgumentNullException(nameof(assertCallback));
            }
            assertCallback.Invoke(GetEvent<TEvent>(stream, index));
        }

        private TEvent GetEvent<TEvent>(IReadOnlyList<object> stream, int index)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            var @event = stream[index];

            try
            {
                return (TEvent) @event;
            }
            catch (InvalidCastException)
            {
                throw NewUnexpectedEventTypeException(typeof(TEvent), @event.GetType(), index);
            }
        }

        private Exception NewUnexpectedEventTypeException(Type expectedType, Type actualType, int index)
        {
            var messageFormat = ExceptionMessages.WriteScenario_UnexpectedEventType;
            var message = string.Format(messageFormat, expectedType.FriendlyName(), index, actualType.FriendlyName());
            return NewAssertFailedException(message);
        }

        #endregion
    }
}
