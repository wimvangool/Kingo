using System.Threading;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Serves as a base-class for objects that are bound to specific <see cref="SynchronizationContext" /> when
    /// they are created.
    /// </summary>
    public abstract class AsyncObject
    {
        private readonly SynchronizationContext _synchronizationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncObject" /> class.
        /// </summary>        
        protected AsyncObject()
        {
            _synchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncObject" /> class.
        /// </summary>
        /// <param name="synchronizationContext">The context to use to send messages to the appropriate thread.</param>
        protected AsyncObject(SynchronizationContext synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
        }

        /// <summary>
        /// Returns the <see cref="SynchronizationContext" /> that is used by this instance to
        /// send messages to the appropriate thread.
        /// </summary>
        protected SynchronizationContext SynchronizationContext
        {
            get { return _synchronizationContext; }
        }

        /// <summary>
        /// Creates and returns a new <see cref="SynchronizationContextScope" /> based on the associated <see cref="SynchronizationContext" />.
        /// </summary>
        /// <returns>A new <see cref="SynchronizationContextScope" />.</returns>
        protected SynchronizationContextScope CreateSynchronizationContextScope()
        {
            return new SynchronizationContextScope(SynchronizationContext);
        }
    }
}
