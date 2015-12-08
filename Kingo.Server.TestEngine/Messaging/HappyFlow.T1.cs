using System;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a happy flow, in which a certain number of events is expected to be published.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is processed on the When-phase.</typeparam>
    public sealed class HappyFlow<TMessage> : ExecutionFlow<TMessage> where TMessage : class, IMessage<TMessage>
    {        
        private readonly Scenario<TMessage> _scenario;        
        private readonly bool[] _expectations;

        internal HappyFlow(Scenario<TMessage> scenario, int expectedEventCount)
        {
            if (expectedEventCount < 0)
            {
                throw NewInvalidEventCountException(expectedEventCount);
            }            
            _scenario = scenario;            
            _expectations = new bool[expectedEventCount];
        }

        /// <inheritdoc />
        protected override Scenario<TMessage> Scenario
        {
            get { return _scenario; }
        }

        /// <summary>
        /// Specifies that a certain type of event is expected to be published at a certain time (<paramref name="index"/>).
        /// </summary>
        /// <typeparam name="TEvent">Type of the expected event.</typeparam>
        /// <param name="index">
        /// The index of the event, which basically corresponds to the position of the event in the sequence of published events.
        /// </param>
        /// <param name="validateMethod">
        /// Optional delegate that is used to add certain constraints to the expected event.
        /// </param>
        /// <returns>This flow.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An expectation for the specified <paramref name="index"/> has already been set.
        /// </exception>
        public HappyFlow<TMessage> Expect<TEvent>(int index, Action<IMemberConstraintSet<TEvent>> validateMethod = null)
        {
            if (index < 0)
            {
                throw NewNegativeIndexException(index);
            }
            if (_expectations[index])
            {
                throw NewExpectationAlreadySetException(index);
            }            
            var constraint = Validator.VerifyThat(scenario => scenario.PublishedEvents[index]).IsInstanceOf<TEvent>();

            if (validateMethod != null)
            {
                constraint.And(validateMethod);
            }
            _expectations[index] = true;
            return this;
        }

        /// <inheritdoc />
        public override async Task ExecuteAsync()
        {
 	        await _scenario.ExecuteCoreAsync();

            if (_scenario.Exception == null)
            {
                var actualEventCount = Scenario.PublishedEvents.Count;
                if (actualEventCount == _expectations.Length)
                {
                    ValidateExpectations();
                }
                else
                {
                    OnUnexpectedNumberOfEventsPublished(_expectations.Length, actualEventCount);
                }             
            }
            else
            {
                Rethrow(_scenario.Exception); 
            }
        }              

        private void OnUnexpectedNumberOfEventsPublished(int expectedEventCount, int actualEventCount)
        {
            var messageFormat = ExceptionMessages.Scenario_UnexpectedEventCount;
            var message = string.Format(messageFormat, expectedEventCount, actualEventCount);

            _scenario.OnVerificationFailed(message);
        }

        private static Exception NewInvalidEventCountException(int expectedEventCount)
        {
            var messageFormat = ExceptionMessages.Scenario_InvalidEventCount;
            var message = string.Format(messageFormat, expectedEventCount);
            return new ArgumentOutOfRangeException(message);
        }

        private static Exception NewNegativeIndexException(int index)
        {
            var messageFormat = ExceptionMessages.Scenario_NegativeIndex;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }

        private static Exception NewExpectationAlreadySetException(int index)
        {
            var messageFormat = ExceptionMessages.Scenario_EventExpectationAlreadySet;
            var message = string.Format(messageFormat, index);
            return new ArgumentException(message, "index");
        }
    }
}
