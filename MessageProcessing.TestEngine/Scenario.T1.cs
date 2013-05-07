﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is executed on the When-phase.</typeparam>    
    public abstract class Scenario<TMessage> : MessageSequence, IScenario
        where TMessage : class
    {
        private readonly ScenarioClock _clock;
        private readonly VerificationStatement _statement;
        private readonly List<object> _domainEvents;                
      
        /// <summary>
        /// Initializes a new instance of the <see cref="Scenario{T}" /> class.
        /// </summary>
        protected Scenario()
        {
            _clock = new ScenarioClock(DateTime.MinValue);
            _statement = new VerificationStatement(this);
            _domainEvents = new List<object>();            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scenario{T}" /> class.
        /// </summary>
        /// <param name="clockOffset">The date and time to which the clock is initialized.</param>
        protected Scenario(DateTime clockOffset)
        {
            _clock = new ScenarioClock(clockOffset);
            _statement = new VerificationStatement(this);
            _domainEvents = new List<object>(); 
        }

        /// <summary>
        /// Returns the clock that is used to control the timeline in which the scenario is executed.
        /// </summary>
        public ScenarioClock Clock
        {
            get { return _clock; }
        }

        /// <summary>
        /// Returns the last message that was handled in the When-phase.
        /// </summary>
        protected TMessage Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a statement that can be used to verify a certain value in the Then-phase.
        /// </summary>
        protected IVerificationStatement Verify
        {
            get { return _statement; }
        }

        /// <summary>
        /// Returns a collection of domain-events that were raised in the When-phase.
        /// </summary>
        protected IList<object> DomainEvents
        {
            get { return _domainEvents; }
        }

        /// <summary>
        /// Indicates whether or not an exception is expected to be thrown during the When-phase.
        /// </summary>
        protected bool ExceptionExpected
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the exception that was thrown during the When-phase, or <c>null</c> if no exception was thrown.
        /// </summary>
        protected Exception Exception
        {
            get;
            private set;
        }        

        /// <summary>
        /// Handles the scenario in two phases: first the <i>Given</i>-phase, followed by the <i>When</i>-phase.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Given-phase is represented by the set of messages that are returned by the <see cref="Given" />-method.
        /// All returned messages are assumed to succeed. Any exceptions thrown during the Given-phase will not be
        /// caught and always result in a failed scenario test.
        /// </para>
        /// <para>
        /// The When-phase is represented by a single message, of which all effects are subject to test and verification.
        /// If an exception is expected to be thrown, then the <see cerf="ExceptionException" /> must be set to
        /// <c>true</c> before this method is called. If so, any exception will be caught and the <see cref="Exception" />
        /// property will be set accordingly. On the other hand, if <see cerf="ExceptionException" /> is set to <c>true</c>
        /// and no exception is thrown as a result of the message to test, then the scenario's <see cref="Fail(string)" />
        /// method will be called with a message indicating the expected exception was not thrown. If no exception is
        /// expected, any exception is simply rethrown, assuming the scenario will fail automatically as a result.
        /// </para>
        /// <para>
        /// During the When-phase, all events that are published on the <see cref="DomainEventBus" /> are stored in a
        /// collection, ready to be verified using the <see cref="DomainEvents" /> property. This collection will be
        /// empty if no events were published, or when an exception was raised during the When-phase.
        /// </para>
        /// <para>
        /// </para>
        /// </remarks>
        public sealed override void HandleWith(IMessageProcessor handler)
        {
            Reset();
            Given().HandleWith(handler);
            Message = When();
            
            try
            {
                HandleMessage(handler, Message);
            }
            catch (Exception exception)
            {
                Exception = exception;

                if (!ExceptionExpected)
                {
                    throw;
                }
            }
        }

        private void HandleMessage(IMessageProcessor messageHandler, TMessage message)
        {
            var messageNode = new MessageSequenceNode<TMessage>(message);

            using (DomainEventBus.Subscribe<object>(domainEvent => _domainEvents.Add(domainEvent)))
            {
                messageNode.HandleWith(messageHandler);                
            }
            if (ExceptionExpected)
            {
                Fail(FailureMessages.ExceptionExpected);
            }
        }
        
        void IScenario.Fail()
        {
            Fail(null);
        }
        
        void IScenario.Fail(string message, params object[] parameters)
        {
            Fail(string.Format(CultureInfo.CurrentCulture, message, parameters));
        }

        /// <summary>
        /// Fails the scenario with the specified message.
        /// </summary>
        /// <param name="message">A message indicating the cause of failure.</param>
        protected abstract void Fail(string message);

        /// <summary>
        /// Resets the scenario's by setting the <see cref="Exception" /> to <c>null</c> and clearing the <see cref="DomainEvents"/>.
        /// </summary>
        protected void Reset()
        {
            Message = null;
            Exception = null;

            _domainEvents.Clear();                        
        }

        /// <summary>
        /// Returns the domain-event at the specified index as the requested type.
        /// </summary>
        /// <typeparam name="TEvent">Expected type of the domain-event.</typeparam>
        /// <param name="index">Index of the domain-event.</param>
        /// <returns>
        /// The domain-event at the specified <paramref name="index"/>, cast to the specified <paramtyperef name="TEvent"/>, or
        /// <c>null</c> if no domain-event of the specified index of the specified type exists.
        /// </returns>
        protected TEvent DomainEventAt<TEvent>(int index) where TEvent : class
        {
            if (index < 0)
            {
                throw NewIndexOutOfRangeException(index);
            }
            if (index < _domainEvents.Count)
            {
                return _domainEvents[index] as TEvent;
            }
            return null;
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

        private static Exception NewIndexOutOfRangeException(int index)
        {
            var messageFormat = ExceptionMessages.Scenario_IndexNegative;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }        
    }
}
