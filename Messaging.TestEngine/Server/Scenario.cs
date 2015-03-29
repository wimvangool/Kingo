using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>       
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public abstract class Scenario : MessageSequence
    {        
        private readonly DependencyCache _cache;        
                
        internal Scenario()
        {                       
            _cache = new DependencyCache();            
        }        

        internal IDependencyCache InternalCache
        {
            get { return _cache; }
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
        /// Completes this <see cref="Scenario" /> by performing all necessary cleanup.
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
    }
}
