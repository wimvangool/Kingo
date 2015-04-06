using System.Collections.Generic;
using System.ComponentModel.FluentValidation;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
        #region [====== DomainEventListFactory ======]

        private sealed class DomainEventListFactory<T> : IDomainEventListFactory where T : class, IMessage<T>
        {
            private readonly Scenario<T> _scenario;

            internal DomainEventListFactory(Scenario<T> scenario)
            {
                _scenario = scenario;
            }

            public void And(Action<IDomainEventList> domainEventListHandler)
            {
                if (domainEventListHandler == null)
                {
                    throw new ArgumentNullException("domainEventListHandler");
                }
                domainEventListHandler.Invoke(new DomainEventList<T>(_scenario));
            }
        }

        #endregion

        #region [====== DomainEventList ======]

        private sealed class DomainEventList<T> : IDomainEventList where T : class, IMessage<T>
        {
            private readonly Scenario<T> _scenario;

            internal DomainEventList(Scenario<T> scenario)
            {
                _scenario = scenario;
            }

            public Member<object> this[int index]
            {
                get
                {
                    var domainEvent = _scenario._publishedEvents[index];
                    var name = string.Format(CultureInfo.InvariantCulture, "DomainEvent[{0}]", index);

                    return _scenario.VerifyThat(domainEvent, name);
                }                
            }
        }

        #endregion

        private readonly MemberSet _memberSet;                
        private readonly List<object> _publishedEvents;
        private Exception _exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scenario{TMessage}" /> class.
        /// </summary>
        protected Scenario()
        {
            _memberSet = new MemberSet(this);
            _publishedEvents = new List<object>();
        }

        /// <summary>
        /// Executes the scenario in two phases: first the <i>Given</i>-phase, followed by the <i>When</i>-phase.
        /// </summary>        
        public override void ProcessWith(IMessageProcessor processor)
        {            
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            Given().ProcessWith(processor);

            // This scenario must collect all events that are published during the When()-phase.
            using (processor.EventBus.ConnectThreadLocal<object>(OnEventPublished, true))
            using (var scope = processor.CreateUnitOfWorkScope())
            {
                Message = When();

                if (HandleMessage(processor, Message))
                {
                    scope.Complete();
                }
            }            
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By Design")]
        private bool HandleMessage(IMessageProcessor processor, TMessage message)
        {
            try
            {
                processor.Handle(message);
                return true;
            }
            catch (Exception exception)
            {
                _exception = exception;
                return false;                
            }                        
        }

        private void OnEventPublished(object domainEvent)
        {
            _publishedEvents.Add(domainEvent);
        }

        /// <summary>
        /// Returns the last message that was handled in the When-phase.
        /// </summary>
        public TMessage Message
        {
            get;
            private set;
        }        

        /// <summary>
        /// Returns a sequence of messages that are used to put the system into a desired state.
        /// </summary>
        /// <returns>A sequence of messages that are used to put the system into a desired state.</returns>
        /// <remarks>
        /// The default implementation returns an empty sequence. When overridden, this method should never return
        /// <c>null</c>.
        /// </remarks>
        protected virtual IMessageSequence Given()
        {
            return EmptySequence;
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

        #region [====== Verification ======]

        /// <summary>
        /// Verifies that the number of published domain events is equal to <paramref name="count" />
        /// and returns a <see cref="IDomainEventListFactory" /> that can be used to verify the contents
        /// of each event.
        /// </summary>
        /// <param name="count">The expected number of events that were published.</param>
        /// <returns>A <see cref="IDomainEventListFactory" /> that can be used to verify the contents of each event.</returns>
        protected IDomainEventListFactory TheNumberOfPublishedEventsIs(int count)
        {
            VerifyThat(_publishedEvents.Count, "NumberOfPublishedEvents")
                .IsEqualTo(count, new ErrorMessage(FailureMessages.Scenario_UnexpectedNumberOfPublishedEvents, _publishedEvents.Count, count));

            return new DomainEventListFactory<TMessage>(this);
        }

        /// <summary>
        /// Verifies that an <see cref="Exception" /> of the specified type (<typeparamref name="TException"/>) was thrown
        /// and returns an instance of this <see cref="Exception" /> in the form of a <see cref="Member{TValue}" /> such that
        /// more details about this exception can be verified.
        /// </summary>
        /// <typeparam name="TException">Type of the expected exception.</typeparam>
        /// <returns>A <see cref="Member{TValue}" /> referring to the exception that was thrown.</returns>
        protected Member<TException> TheExceptionThatWasThrownIsA<TException>()
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
            throw NewScenarioFailedException(string.Format(CultureInfo.InvariantCulture, "{0} --> {1}", memberName, errorMessage));
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
