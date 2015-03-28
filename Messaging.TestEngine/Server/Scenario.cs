using System.Collections.Generic;
using System.ComponentModel.FluentValidation;
using System.ComponentModel.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>       
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public abstract class Scenario : MessageSequence, IErrorMessageConsumer
    {
        private readonly MemberSet _memberSet;
        private readonly List<object> _domainEvents;
        private readonly DependencyCache _cache;        
                
        internal Scenario()
        {            
            _memberSet = new MemberSet(this);
            _domainEvents = new List<object>();
            _cache = new DependencyCache();            
        }

        #region [====== Verification ======]

        /// <summary>
        /// Returns a <see cref="Member{Object}" /> instance that represents the event at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Index of the requested event.</param>
        /// <returns>A <see cref="Member{Object}" /> instance representing the requested event.</returns>        
        protected Member<object> VerifyThatEventAtIndex(int index)
        {
            return VerifyThat(() => DomainEvents).HasElementAt(index);            
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
        public Member<TValue> Put<TValue>(Func<TValue> valueFactory, string name)
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
        public Member<TValue> Put<TValue>(TValue value, string name)
        {
            return _memberSet.StartToAddConstraintsFor(value, name);
        }       

        void IErrorMessageConsumer.Add(string memberName, ErrorMessage errorMessage)
        {
            Fail(string.Format(CultureInfo.InvariantCulture, "{0} --> {1}", memberName, errorMessage));
        }

        /// <summary>
        /// Marks this scenario as failed using the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The reason why the scenario failed.</param>
        protected abstract void Fail(string message);

        #endregion

        #region [====== Domain Events ======]

        /// <summary>
        /// Returns the collection of domain events that were published.
        /// </summary>
        public IEnumerable<object> DomainEvents
        {
            get { return _domainEvents; }
        }

        /// <summary>
        /// Returns the number of domain events that were published.
        /// </summary>
        public int DomainEventCount
        {
            get { return _domainEvents.Count; }
        }

        /// <summary>
        /// Returns the expected number of collected domain-events.
        /// </summary>
        protected abstract int ExpectedDomainEventCount
        {
            get;
        }

        internal void SaveDomainEvent(object domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }                

        #endregion

        internal IDependencyCache InternalCache
        {
            get { return _cache; }
        }

        /// <summary>
        /// Indicates whether or not an exception is expected to be thrown during the When-phase.
        /// </summary>
        protected abstract bool ExceptionExpected
        {
            get;
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

                VerifyThat(() => DomainEventCount)
                    .IsEqualTo(ExceptionExpected ? 0 : ExpectedDomainEventCount, FailureMessages.Scenario_UnexpectedDomainEventCount, DomainEventCount, ExpectedDomainEventCount);
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
