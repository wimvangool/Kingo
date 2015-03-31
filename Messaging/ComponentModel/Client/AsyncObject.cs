using System.ComponentModel.Resources;
using System.Threading;

namespace System.ComponentModel.Client
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
        /// Invokes the specified <paramref name="action"/> through the associated <see cref="SynchronizationContext" />
        /// and blocks the current thread until done.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <exception cref="InvalidOperationException">
        /// <see cref="SynchronizationContext" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        protected void Send(Action action)
        {
            if (_synchronizationContext == null)
            {
                throw NewSynchronizationContextUnavailableException();
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _synchronizationContext.Send(s => action.Invoke(), null);
        }

        /// <summary>
        /// Invokes the specified <paramref name="action"/> through the associated <see cref="SynchronizationContext" />
        /// and blocks the current thread until done.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="state">The optional argument of the action to invoke.</param>
        /// <exception cref="InvalidOperationException">
        /// <see cref="SynchronizationContext" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        protected void Send(Action<object> action, object state)
        {
            if (_synchronizationContext == null)
            {
                throw NewSynchronizationContextUnavailableException();
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _synchronizationContext.Send(action.Invoke, state);
        }

        /// <summary>
        /// Invokes the specified <paramref name="action"/> through the associated <see cref="SynchronizationContext" />,
        /// if it is available.
        /// </summary>
        /// <param name="action">The action to invoke.</param>           
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        protected void Post(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            if (_synchronizationContext == null)
            {
                return;
            }
            _synchronizationContext.Post(s => action.Invoke(), null);
        }

        /// <summary>
        /// Invokes the specified <paramref name="action"/> through the associated <see cref="SynchronizationContext" />,
        /// if it is available.
        /// </summary>
        /// <param name="action">The action to invoke.</param>   
        /// <param name="state">The optional argument of the action to invoke.</param>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        protected void Post(Action<object> action, object state)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            if (_synchronizationContext == null)
            {
                return;
            }
            _synchronizationContext.Post(action.Invoke, state);
        }

        private static Exception NewSynchronizationContextUnavailableException()
        {
            return new InvalidOperationException(ExceptionMessages.AsyncObject_ContextNotSet);
        }
    }
}
