using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is processed on the When-phase.</typeparam>    
    public abstract class Scenario<TMessage> : Scenario where TMessage : class, IMessage
    {
        #region [====== PublishedEventList ======]

        private sealed class PublishedEventList : IReadOnlyList<object>
        {
            private readonly List<object> _publishedEvents;
            private readonly Type _scenarioType;

            public PublishedEventList(Type scenarioType)
            {
                _publishedEvents = new List<object>();
                _scenarioType = scenarioType;
            }

            public int Count
            {
                get { return _publishedEvents.Count; }
            }

            public object this[int index]
            {
                get
                {
                    try
                    {
                        return _publishedEvents[index];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        if (index < 0)
                        {
                            throw;
                        }
                        throw NewEventNotFoundException(_scenarioType, index);
                    }
                }
            }

            public void Add(object @event)
            {
                _publishedEvents.Add(@event);
            }

            public IEnumerator<object> GetEnumerator()
            {
                return _publishedEvents.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private static Exception NewEventNotFoundException(Type scenarioType, int index)
            {
                var messageFormat = ExceptionMessages.Scenario_NoEventAtIndex;
                var message = string.Format(messageFormat, scenarioType.Name, index);
                return new ArgumentOutOfRangeException(nameof(index), message);
            }
        }

        #endregion

        private readonly Lazy<MessageToHandle<TMessage>> _message;                       
        private readonly PublishedEventList _publishedEvents;        

        /// <summary>
        /// Initializes a new instance of the <see cref="Scenario{TMessage}" /> class.
        /// </summary>
        /// <param name="mode">The mode this scenario will execute in.</param>
        protected Scenario(ScenarioMode mode = ScenarioMode.WriteOnly)
            : base(mode)
        {           
            _message = new Lazy<MessageToHandle<TMessage>>(When);            
            _publishedEvents = new PublishedEventList(GetType());
        }        

        /// <summary>
        /// Returns the last message that was handled in the When-phase.
        /// </summary>
        public TMessage Message
        {
            get { return _message.Value.Message; }
        }

        /// <summary>
        /// Gets the <see cref="FunctionalException" /> that was caught during the When-phase.
        /// </summary>
        protected internal FunctionalException ThrownException
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the collection of events that were published during the When-phase.
        /// </summary>
        protected internal IReadOnlyList<object> PublishedEvents
        {
            get { return _publishedEvents; }
        }

        /// <inheritdoc />
        public override async Task ThenAsync()
        {
            await Events().ExecuteAsync();
        }

        /// <summary>
        /// Executes this scenario while expecting exactly one event of the specified type to be published.
        /// </summary>
        /// <typeparam name="TEvent">Type of the expected event.</typeparam>
        /// <param name="validator">Optional delegate to define constraints on the expected event.</param>
        /// <returns>A <see cref="Task" /> representing the operation.</returns>
        protected async Task ExpectedEvent<TEvent>(Action<IMemberConstraintSet<TEvent>> validator = null)
        {
            await Events().Expect(validator).ExecuteAsync();
        }

        /// <summary>
        /// Creates and returns a new <see cref="HappyFlow{T}" /> which can be used to define
        /// which events are expected to be published.
        /// </summary>        
        /// <returns>A happy flow.</returns>        
        protected HappyFlow<TMessage> Events()
        {
            return new HappyFlow<TMessage>(this);
        }

        /// <summary>
        /// Executes this scenario while expecting a <see cref="CommandExecutionException" /> to be thrown.
        /// </summary>        
        /// <param name="validator">Optional delegate to define constraints on the expected exception.</param>
        /// <param name="rethrowException">
        /// Indicates whether or not the exception should be rethron after it has been caught and verified.
        /// </param>
        /// <returns>A <see cref="Task" /> representing the operation.</returns>
        protected Task ExpectedCommandExecutionException(Action<IMemberConstraintSet<CommandExecutionException>> validator = null, bool rethrowException = false)
        {
            return ExpectedException(validator, rethrowException);
        }

        /// <summary>
        /// Executes this scenario while expecting the specified type of exception to be thrown.
        /// </summary>
        /// <typeparam name="TException">The expected type of exception.</typeparam>
        /// <param name="validator">Optional delegate to define constraints on the expected exception.</param>
        /// <param name="rethrowException">
        /// Indicates whether or not the exception should be rethron after it has been caught and verified.
        /// </param>
        /// <returns>A <see cref="Task" /> representing the operation.</returns>
        protected async Task ExpectedException<TException>(Action<IMemberConstraintSet<TException>> validator = null, bool rethrowException = false)
            where TException : FunctionalException
        {
            await Exception(rethrowException).Expect(validator).ExecuteAsync();
        }

        /// <summary>
        /// Creates and returns a new <see cref="AlternateFlow{T}" /> which can be used to set some
        /// expectations on the <see cref="FunctionalException" /> that is expected to be thrown.
        /// </summary>
        /// <param name="rethrowException">
        /// Indicates whether or not the exception should be rethron after it has been caught and verified.
        /// </param>
        /// <returns>An alternate flow.</returns>
        protected AlternateFlow<TMessage> Exception(bool rethrowException = false)
        {
            return new AlternateFlow<TMessage>(this, rethrowException);
        }

        /// <summary>
        /// Executes the scenario in two phases: first the <i>Given</i>-phase, followed by the <i>When</i>-phase.
        /// </summary>                        
        public override async Task ProcessWithAsync(IMessageProcessor processor, CancellationToken token)
        {
            await ExecuteGiven(processor, token);

            // This scenario must collect all events that are published during the When()-phase.              
            var connection = processor.EventBus.Connect<object>(OnEventPublished, true);
            
            try
            {
                await _message.Value.ProcessWithAsync(processor, token);
            }                   
            catch (FunctionalException exception)
            {                
                ThrownException = exception;                
            }
            finally
            {
                connection.Dispose();
            }            
        }   
        
        private async Task ExecuteGiven(IMessageProcessor processor, CancellationToken token)
        {
            try
            {
                await CreateSetupSequence().ProcessWithAsync(processor, token);
            }
            catch (Exception exception)
            {
                var exceptionDetails = $"{exception.Message}{Environment.NewLine}{Environment.NewLine}{exception.StackTrace}";
                var messageFormat = ExceptionMessages.Scenario_GivenFailed;
                var message = string.Format(messageFormat, exception.GetType().Name, GetType().Name, exceptionDetails);

                throw NewScenarioFailedException(message, exception);
            }
        }

        private IMessageSequence CreateSetupSequence()
        {
            return Given().Concatenate();
        }

        /// <summary>
        /// Returns a sequence of messages that are used to put the system into a desired state.
        /// </summary>
        /// <returns>A sequence of messages that are used to put the system into a desired state.</returns>
        /// <remarks>
        /// The default implementation returns an empty sequence. When overridden, this method should never return
        /// <c>null</c>.
        /// </remarks>
        protected virtual IEnumerable<IMessageSequence> Given()
        {
            yield return EmptySequence;
        }

        /// <summary>
        /// Returns a message that is used to test a system's behavior.
        /// </summary>
        /// <returns>A single message of which the effects will be verified in the Then-phase.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716", MessageId = "When", Justification = "'When' is part of the BDD-style naming convention.")]
        protected abstract MessageToHandle<TMessage> When();

        /// <summary>
        /// This method is called every time an event is published during execution of When().
        /// </summary>
        /// <param name="domainEvent">The event that was published.</param>
        protected virtual void OnEventPublished(object domainEvent)
        {
            _publishedEvents.Add(domainEvent);
        }

        #region [====== Verification ======]

        /// <summary>
        /// Gets or sets the <see cref="IFormatProvider" /> that is used to format all the error messages.
        /// </summary>
        protected internal IFormatProvider FormatProvider
        {
            get;
            protected set;
        }                    

        /// <summary>
        /// Occurs when verification of a certain member during the Then-phase failed.
        /// </summary>        
        /// <param name="errorMessage">The error message.</param>
        protected internal virtual void OnVerificationFailed(string errorMessage)
        {
            throw NewScenarioFailedException(errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="ThrownException" /> that will be thrown to mark the failure of this scenario.
        /// </summary>
        /// <param name="errorMessage">The reason why the scenario failed.</param>
        /// <param name="innerException">Optional Exception that was the root-cause of the failure.</param>
        /// <returns>A new <see cref="ThrownException" />-instance with the specfied <paramref name="errorMessage"/>.</returns>
        protected abstract Exception NewScenarioFailedException(string errorMessage, Exception innerException = null);

        #endregion
    }
}
