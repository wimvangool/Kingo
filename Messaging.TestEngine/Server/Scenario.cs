using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>       
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public abstract class Scenario : MessageSequence, IScenario
    {        
        private readonly VerificationStatement _statement;
        private readonly List<object> _domainEvents;
        private readonly ScopeSpecificCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scenario" /> class.
        /// </summary>        
        internal Scenario()
        {            
            _statement = new VerificationStatement(this);
            _domainEvents = new List<object>();
            _cache = new ScopeSpecificCache();
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

        internal IDependencyCache InternalCache
        {
            get { return _cache; }
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
        protected internal Exception Exception
        {
            get;
            internal set;
        }        

        /// <summary>
        /// Returns the processor that is used to execute this <see cref="Scenario" />.
        /// </summary>
        protected abstract IMessageProcessor MessageProcessor
        {
            get;
        }

        /// <summary>
        /// Executes this <see cref="Scenario" />.
        /// </summary>
        public virtual void Execute()
        {
            Current = this;

            try
            {
                ProcessWith(MessageProcessor);
            }
            finally
            {
                Current = null;
            }            
        }

        /// <summary>
        /// Completes this execution by performing all necessary cleanup.
        /// </summary>
        public virtual void Complete()
        {
            _cache.Dispose();                      
        }

        void IScenario.Fail()
        {
            Fail();
        }

        /// <summary>
        /// Marks this scenario as failed.
        /// </summary>
        protected abstract void Fail();

        void IScenario.Fail(string message)
        {
            Fail(message);
        }

        /// <summary>
        /// Marks this scenario as failed using the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The reason why the scenario failed.</param>
        protected abstract void Fail(string message);

        void IScenario.Fail(string message, params object[] parameters)
        {
            Fail(message, parameters);
        }

        /// <summary>
        /// Marks this scenario as failed using the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The reason why the scenario failed.</param>
        /// <param name="parameters">An optional array of parameters to include in the message.</param>
        protected abstract void Fail(string message, params object[] parameters);

        internal void SaveDomainEvent(object domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

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
        /// Returns the cache that is associated to the <see cref="Scenario" /> that is currently being executed.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly IDependencyCache Cache = new ScenarioCache();

        private static readonly ThreadLocal<Scenario> _Current = new ThreadLocal<Scenario>();

        /// <summary>
        /// Returns the <see cref="Scenario" /> that is currently being executed on this thread.
        /// </summary>
        internal static Scenario Current
        {
            get {  return _Current.Value; }
            private set { _Current.Value = value; }
        }

        private static Exception NewNoDomainEventFoundAtIndexException(int index)
        {
            var messageFormat = ExceptionMessages.Scenario_DomainEventNotFound;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }
    }
}
