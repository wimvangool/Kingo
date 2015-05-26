using System.Collections.Generic;
using System.ComponentModel.FluentValidation;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Resources;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is executed on the When-phase.</typeparam>    
    public abstract class Scenario<TMessage> : Scenario, IErrorMessageConsumer where TMessage : class, IMessage<TMessage>
    {
        private readonly Lazy<TMessage> _message;
        private readonly MemberSet _memberSet;                
        private readonly List<object> _publishedEvents;
        private FunctionalException _exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scenario{TMessage}" /> class.
        /// </summary>
        protected Scenario()
        {
            _message = new Lazy<TMessage>(When);
            _memberSet = new MemberSet(this);
            _publishedEvents = new List<object>();
        }

        /// <summary>
        /// Returns the last message that was handled in the When-phase.
        /// </summary>
        public TMessage Message
        {
            get { return _message.Value; }
        }  

        /// <summary>
        /// Executes the scenario in two phases: first the <i>Given</i>-phase, followed by the <i>When</i>-phase.
        /// </summary>        
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By Design")]
        public override void ProcessWith(IMessageProcessor processor)
        {            
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            CreateSetupSequence().ProcessWith(processor);

            // This scenario must collect all events that are published during the When()-phase.
            var connection = processor.EventBus.Connect<object>(OnEventPublished, true);
            
            try
            {
                Handle(processor, Message.Copy());
            }
            catch (AggregateException exception)
            {
                // When handling an AggregateException, we expect only a single functional exception to be thrown,
                // since we can only verify a single exception by design.
                FunctionalException functionalException;

                if (TryGetFunctionalExceptionFrom(exception, out functionalException))
                {
                    _exception = functionalException;                    
                }
                else
                {
                    throw;
                }                
            }        
            catch (FunctionalException exception)
            {
                _exception = exception;
            }
            finally
            {
                connection.Dispose();
            }
        }
    
        private static bool TryGetFunctionalExceptionFrom(AggregateException aggregateException, out FunctionalException functionalException)
        {
            var functionalExceptions = new List<FunctionalException>();

            aggregateException.Flatten().Handle(innerException =>
            {
                var exception = innerException as FunctionalException;
                if (exception != null)
                {
                    functionalExceptions.Add(exception);
                    return true;
                }
                return false;
            });

            if (functionalExceptions.Count == 1)
            {
                functionalException = functionalExceptions[0];
                return true;
            }
            functionalException = null;
            return false;
        }
    
        /// <summary>
        /// Handles the specified <paramref name="message" /> by passing it to the specified <paramref name="processor"/>.
        /// </summary>
        /// <param name="processor">A message processor.</param>
        /// <param name="message">The message to handle.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected virtual void Handle(IMessageProcessor processor, TMessage message)
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            processor.Handle(message);
        }

        private void OnEventPublished(object domainEvent)
        {
            _publishedEvents.Add(domainEvent);
        } 
        
        private IMessageSequence CreateSetupSequence()
        {
            return Concatenate(Given());
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
        protected abstract TMessage When();

        /// <summary>
        /// Verifies all expected outcomes.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Then", Justification = "'Then' is part of the BDD-style naming convention.")]
        public abstract void Then();

        #region [====== Domain Events ======]

        /// <summary>
        /// Returns all published events as a collection.
        /// </summary>
        protected IEnumerable<object> PublishedEvents
        {
            get { return _publishedEvents; }
        }

        /// <summary>
        /// Returns the event at the specified <paramref name="index"/> as an instance of <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">Type of the requested event.</typeparam>
        /// <param name="index">The index of the event.</param>
        /// <returns>The event at the specified <paramref name="index"/> as an instance of <typeparamref name="TEvent"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> does not point to a valid index.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The event of the specified <paramref name="index"/> could not be cast to <typeparamref name="TEvent"/>.
        /// </exception>
        protected TEvent GetDomainEventAt<TEvent>(int index) where TEvent : class
        {
            var domainEvent = GetDomainEventAt(index);

            try
            {
                return (TEvent) domainEvent;
            }
            catch (InvalidCastException)
            {
                throw NewUnexpectedEventTypeException(typeof(TEvent), domainEvent.GetType(), index);
            }
        }               

        /// <summary>
        /// Returns the event at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the event.</param>
        /// <returns>The event at the specified <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> does not point to a valid index.
        /// </exception>
        protected object GetDomainEventAt(int index)
        {
            try
            {
                return _publishedEvents[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                throw NewEventNotFoundException(index);
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
        private static Exception NewUnexpectedEventTypeException(Type expectedType, Type actualType, int index)
        {
            var messageFormat = ExceptionMessages.Scenario_EventNotOfSpecifiedType;
            var message = string.Format(messageFormat, index, actualType, expectedType);
            return new InvalidCastException(message);
        }

        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        private static Exception NewEventNotFoundException(int index)
        {
            var messageFormat = ExceptionMessages.Scenario_EventNotFound;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }

        #endregion

        #region [====== Verification ======]

        /// <summary>
        /// Returns the number of published domain events that can be verified with a fluent syntax.
        /// </summary>
        /// <returns>A <see cref="Member{T}" /> that can be used to verify the number of published domain events.</returns>        
        protected Member<int> VerifyThatDomainEventCount()
        {
            return VerifyThat(() => _publishedEvents.Count, "DomainEventCount");
        }        

        /// <summary>
        /// Returns the published domain event at the specified index that can be verified with a fluent syntax.
        /// </summary>
        /// <param name="index">The index of the published event.</param>
        /// <returns>A <see cref="Member{T}" /> that can be used to verify the event.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> does not point to a valid index.
        /// </exception>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        protected Member<object> VerifyThatDomainEventAtIndex(int index)
        {
            return VerifyThat(GetDomainEventAt(index), string.Format("DomainEvent[{0}]", index));
        }        

        /// <summary>
        /// Verifies that an <see cref="Exception" /> of the specified type (<typeparamref name="TException"/>) was thrown
        /// and returns an instance of this <see cref="Exception" /> in the form of a <see cref="Member{TValue}" /> such that
        /// more details about this exception can be verified.
        /// </summary>
        /// <typeparam name="TException">Type of the expected exception.</typeparam>
        /// <returns>A <see cref="Member{TValue}" /> referring to the exception that was thrown.</returns>
        protected Member<TException> VerifyThatExceptionIsA<TException>() where TException : FunctionalException
        {            
            return VerifyThat(_exception, "ExpectedException")
                .IsNotNull(new ErrorMessage(FailureMessages.Scenario_NoExceptionWasThrown, typeof(TException)))              
                .IsInstanceOf<TException>(new ErrorMessage(FailureMessages.Scenario_UnexpectedExceptionWasThrown, typeof(TException), _exception.GetType()));                
        }        

        /// <summary>
        /// Creates and returns a new <see cref="Member{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="memberExpression">
        /// An expression that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>A new <see cref="Member{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberExpression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="memberExpression"/> refers to a member that was already added.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected Member<TValue> VerifyThat<TValue>(Expression<Func<TValue>> memberExpression)
        {
            return _memberSet.StartToAddConstraintsFor(memberExpression);
        }

        /// <summary>
        /// Creates and returns a new <see cref="Member{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="valueFactory">
        /// A delegate that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <param name="name">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="Member{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueFactory"/> or <paramref name="name"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> VerifyThat<TValue>(Func<TValue> valueFactory, string name)
        {
            return _memberSet.StartToAddConstraintsFor(valueFactory, name);
        }

        /// <summary>
        /// Creates and returns a new <see cref="Member{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="value">The value to add constraints for.</param>
        /// <param name="name">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="Member{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> VerifyThat<TValue>(TValue value, string name)
        {
            return _memberSet.StartToAddConstraintsFor(value, name);
        }

        void IErrorMessageConsumer.Add(string memberName, ErrorMessage errorMessage)
        {
            Fail(errorMessage);
        }

        /// <summary>
        /// Marks this scenario as failed by throwing an exception.
        /// </summary>
        /// <param name="message">Message describing the reason why the scenario failed.</param>
        /// <exception cref="Exception">
        /// Always (by definition).
        /// </exception>
        protected void Fail(ErrorMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            Fail(message.ToString());
        }

        /// <summary>
        /// Marks this scenario as failed by throwing an exception.
        /// </summary>
        /// <param name="message">Message describing the reason why the scenario failed.</param>
        /// <exception cref="Exception">
        /// Always (by definition).
        /// </exception>
        protected virtual void Fail(string message)
        {
            throw NewScenarioFailedException(message);
        }

        /// <summary>
        /// Creates and returns a new <see cref="Exception" /> that will be thrown to mark the failure of this scenario.
        /// </summary>
        /// <param name="message">The reason why the scenario failed.</param>
        /// <returns>A new <see cref="Exception" />-instance with the specfied <paramref name="message"/>.</returns>
        protected abstract Exception NewScenarioFailedException(string message);

        #endregion             
    }
}
