using System;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents an alterate flow, in which a certain type of <see cref="FunctionalException" /> is to be thrown.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is processed on the When-phase.</typeparam>
    public sealed class AlternateFlow<TMessage> : ExecutionFlow<TMessage> where TMessage : class, IMessage
    {        
        private readonly Scenario<TMessage> _scenario;        
        private readonly bool _rethrowException;
        private bool _hasExpectationBeenSet;

        internal AlternateFlow(Scenario<TMessage> scenario, bool rethrowException)
        {                        
            _scenario = scenario;
            _rethrowException = rethrowException;
        }
        
        internal override Scenario<TMessage> Scenario
        {
            get { return _scenario; }
        }

        /// <summary>
        /// Specifies that a certain type of exception is expected to be thrown.
        /// </summary>
        /// <typeparam name="TException">Type of the expected exception.</typeparam>        
        /// <param name="validateMethod">
        /// Optional delegate that is used to add certain constraints to the expected exception.
        /// </param>
        /// <returns>This flow.</returns>
        public AlternateFlow<TMessage> Expect<TException>(Action<IMemberConstraintSet<TException>> validateMethod = null) where TException : FunctionalException
        {
            if (_hasExpectationBeenSet)
            {
                throw NewExpectationAlreadySetException(typeof(TException));
            }
            var constraint = Validator.VerifyThat(scenario => scenario.ThrownException).IsInstanceOf<TException>();

            if (validateMethod != null)
            {
                constraint.And(validateMethod);
            }
            _hasExpectationBeenSet = true;
            return this;
        }        

        /// <inheritdoc />
        public override async Task ExecuteAsync()
        {
            await _scenario.ExecuteCoreAsync();

            if (_scenario.ThrownException == null)
            {
                OnExpectedExceptionNotThrown();
            }
            else
            {
                ValidateExpectations();
            }
        }

        private void OnExpectedExceptionNotThrown()
        {
            if (_scenario.PublishedEvents.Count == 0)
            {
                Scenario.OnVerificationFailed(ExceptionMessages.Scenario_ExpectedExceptionNotThrown);
            }
            else
            {
                var events = string.Join(", ", _scenario.PublishedEvents.Select(@event => @event.GetType().Name));
                var eventPostFixFormat = ExceptionMessages.Scenario_UnexpectedEventsPublished;
                var eventsPostFix = string.Format(eventPostFixFormat, events);

                Scenario.OnVerificationFailed(ExceptionMessages.Scenario_ExpectedExceptionNotThrown + " " + eventsPostFix);
            }
        }

        private static Exception NewExpectationAlreadySetException(Type exceptionType)
        {
            var messageFormat = ExceptionMessages.Scenario_ExceptionExpectationAlreadySet;
            var message = string.Format(messageFormat, exceptionType.Name);
            return new InvalidOperationException(message);
        }
    }
}
