using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging.Constraints;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a happy flow, in which a certain number of events is expected to be published.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is processed on the When-phase.</typeparam>
    public sealed class HappyFlow<TMessage> : ExecutionFlow<TMessage> where TMessage : class, IMessage
    {
        #region [====== ExpectedEvents ======]

        private abstract class ExpectedEvent
        {
            internal abstract void Apply();
        }

        private sealed class ExpectedEvent<TEvent> : ExpectedEvent
        {
            private readonly HappyFlow<TMessage> _happyFlow;
            private readonly Action<IMemberConstraintSet<TEvent>> _validateMethod;
            private bool _hasBeenApplied;

            internal ExpectedEvent(HappyFlow<TMessage> happyFlow, Action<IMemberConstraintSet<TEvent>> validateMethod)
            {
                _happyFlow = happyFlow;
                _validateMethod = validateMethod;
            }

            internal override void Apply()
            {
                if (_hasBeenApplied)
                {
                    return;
                }
                var index = _happyFlow._expectedEvents.IndexOf(this);
                var constraint = _happyFlow.Validator.VerifyThat(scenario => scenario.PublishedEvents[index]).IsInstanceOf<TEvent>();

                if (_validateMethod != null)
                {
                    constraint.And(_validateMethod);
                }
                _hasBeenApplied = true;
            }
        }

        #endregion

        private readonly Scenario<TMessage> _scenario;
        private readonly List<ExpectedEvent> _expectedEvents;

        internal HappyFlow(Scenario<TMessage> scenario)
        {                       
            _scenario = scenario;   
            _expectedEvents = new List<ExpectedEvent>();         
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
            _expectedEvents.Add(new ExpectedEvent<TEvent>(this, validateMethod));            
            return this;
        }

        /// <inheritdoc />
        public override async Task ExecuteAsync()
        {
 	        await _scenario.ExecuteCoreAsync();
            
            if (_scenario.ThrownException == null)
            {
                var actualEventCount = Scenario.PublishedEvents.Count;
                if (actualEventCount == _expectedEvents.Count)
                {
                    foreach (var expectedEvent in _expectedEvents)
                    {
                        expectedEvent.Apply();
                    }
                    ValidateExpectations();
                }
                else
                {
                    OnUnexpectedNumberOfEventsPublished(_expectedEvents.Count, actualEventCount);
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
