using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using YellowFlare.MessageProcessing.Clocks;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is executed on the When-phase.</typeparam>    
    public abstract class Scenario<TMessage> : MessageSequence, IScenario where TMessage : class
    {        
        private readonly StopwatchClock _stopwatch;
        private readonly VerificationStatement _statement;
        private readonly List<object> _domainEvents;                        

        /// <summary>
        /// Initializes a new instance of the <see cref="Scenario{T}" /> class.
        /// </summary>        
        protected Scenario()
        {            
            _stopwatch = new StopwatchClock(DateTime.Now);
            _statement = new VerificationStatement(this);
            _domainEvents = new List<object>();            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scenario{T}" /> class.
        /// </summary>
        /// <param name="startTime">The date and time to which the clock is initialized.</param>       
        protected Scenario(DateTime startTime)
        {            
            _stopwatch = new StopwatchClock(startTime);
            _statement = new VerificationStatement(this);
            _domainEvents = new List<object>(); 
        }

        /// <summary>
        /// Returns the clock that is used to control the timeline in which the scenario is executed.
        /// </summary>
        public virtual IClock Clock
        {
            get { return _stopwatch; }
        }

        /// <summary>
        /// Indicates whether or not this scenario has been disposed.
        /// </summary>
        protected bool IsDisposed
        {
            get;
            private set;
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
        /// Returns the number of collected domain-events.
        /// </summary>
        protected int DomainEventCount
        {
            get { return _domainEvents.Count; }
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if the method was called by the application explicitly (<c>true</c>), or by the finalizer
        /// (<c>false</c>).
        /// </param>
        /// <remarks>
        /// If <paramref name="disposing"/> is <c>true</c>, this method will dispose any managed resources immediately.
        /// Otherwise, only unmanaged resources will be released.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {            
            IsDisposed = true;
        }

        /// <summary>
        /// Executes the scenario in two phases: first the <i>Given</i>-phase, followed by the <i>When</i>-phase.
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
        /// During the When-phase, all events that are published on the <see cref="MessageProcessorBus" /> are stored in a
        /// collection, ready to be verified.
        /// </para>
        /// <para>
        /// </para>
        /// </remarks>
        public sealed override void HandleWith(IMessageProcessor processor)
        {
            if (IsDisposed)
            {
                throw NewScenarioDisposedException();
            }
            _stopwatch.Start();

            try
            {
                HandleWithProcessor(processor);
            }
            finally
            {
                _stopwatch.Stop();
            }            
        }

        private void HandleWithProcessor(IMessageProcessor processor)
        {
            using (var scope = new UnitOfWorkScope(processor.DomainEventBus))
            {
                Reset();
                Given().HandleWith(processor);
                Message = When();

                try
                {
                    HandleMessage(processor, Message, scope);
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
        }

        private void Reset()
        {
            Message = null;
            Exception = null;

            _domainEvents.Clear();
        }        

        private void HandleMessage(IMessageProcessor processor, TMessage message, UnitOfWorkScope scope)
        {
            IMessageSequence messageNode = new MessageSequenceNode<TMessage>(message);

            using (processor.DomainEventBus.Subscribe<object>(domainEvent => _domainEvents.Add(domainEvent)))
            {
                messageNode.HandleWith(processor);
                scope.Complete();
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
        /// Returns the domain-event that was published at the specified moment.
        /// </summary>
        /// <param name="index">Index that indicates the moment the domain-event should have been published.</param>
        /// <returns>The domain-event at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative, or no domain-event was found at the specified index.
        /// </exception>
        protected object DomainEventAt(int index)
        {
            try
            {
                return _domainEvents[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                throw NewNoDomainEventFoundAtIndexException(index);
            }
        }        

        /// <summary>
        /// Returns the domain-event that was published at the specified moment.
        /// </summary>
        /// <param name="index">Index that indicates the moment the domain-event should have been published.</param>
        /// <returns>The domain-event at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative, or no domain-event was found at the specified index.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The domain-event at the specified index could not be cast to the specified type.
        /// </exception>
        protected TEvent DomainEventAt<TEvent>(int index) where TEvent : class
        {
            return (TEvent) DomainEventAt(index);
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
        /// Creates and returns a new <see cref="ObjectDisposedException" /> indicating this scenario has already been disposed.
        /// </summary>
        /// <returns>
        /// A new <see cref="ObjectDisposedException" /> indicating this scenario has already been disposed.
        /// </returns>
        protected ObjectDisposedException NewScenarioDisposedException()
        {
            return new ObjectDisposedException(GetType().Name);
        }        

        private static Exception NewNoDomainEventFoundAtIndexException(int index)
        {
            var messageFormat = ExceptionMessages.Scenario_DomainEventNotFound;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }
    }
}
