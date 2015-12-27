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
        private int _expectedEventCount;

        internal HappyFlow(Scenario<TMessage> scenario)
        {                       
            _scenario = scenario;                        
        }
        
        internal override Scenario<TMessage> Scenario
        {
            get { return _scenario; }
        }

        /// <summary>
        /// Specifies that a certain type of event is expected to be published at a certain time.
        /// </summary>
        /// <typeparam name="TEvent">Type of the expected event.</typeparam>        
        /// <param name="validateMethod">
        /// Optional delegate that is used to add certain constraints to the expected event.
        /// </param>
        /// <returns>This flow.</returns>                
        public HappyFlow<TMessage> Expect<TEvent>(Action<IMemberConstraintSet<TEvent>> validateMethod = null)
        {
            var index = _expectedEventCount;
            var constraint = Validator.VerifyThat(scenario => scenario.PublishedEvents[index]).IsInstanceOf<TEvent>();

            if (validateMethod != null)
            {
                constraint.And(validateMethod);
            }
            _expectedEventCount++;
            return this;
        }

        /// <inheritdoc />
        public override async Task ExecuteAsync()
        {
 	        await _scenario.ExecuteCoreAsync();

            if (_scenario.ThrownException == null)
            {
                var actualEventCount = Scenario.PublishedEvents.Count;
                if (actualEventCount == _expectedEventCount)
                {
                    ValidateExpectations();
                }
                else
                {
                    OnUnexpectedNumberOfEventsPublished(_expectedEventCount, actualEventCount);
                }             
            }
            else
            {
                _scenario.ThrownException.Rethrow();
            }
        }              

        private void OnUnexpectedNumberOfEventsPublished(int expectedEventCount, int actualEventCount)
        {
            var messageFormat = ExceptionMessages.Scenario_UnexpectedEventCount;
            var message = string.Format(messageFormat, expectedEventCount, actualEventCount);

            _scenario.OnVerificationFailed(message);
        }        
    }
}
