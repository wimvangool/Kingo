using System.Diagnostics;
using System.Threading;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>           
    public abstract class Scenario : MessageSequence
    {                                    
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
        public void Execute()
        {            
            var previousCache = Cache;

            Cache = new DependencyCache();

            try
            {
                ProcessWith(MessageProcessor);                
            }
            finally
            {
                Cache.Dispose();
                Cache = previousCache;
            }            
        }        

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly ThreadLocal<DependencyCache> _Cache = new ThreadLocal<DependencyCache>();

        /// <summary>
        /// Returns the <see cref="Scenario" /> that is currently being executed on this thread.
        /// </summary>
        internal static DependencyCache Cache
        {
            get {  return _Cache.Value; }
            private set { _Cache.Value = value; }
        }        
    }
}
