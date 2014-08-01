using System.Collections.Concurrent;
using System.ComponentModel.Messaging.Resources;
using System.Threading;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a scope that manages a <see cref="SynchronizationContext" />.
    /// </summary>
    public sealed class SynchronizationContextScope : IDisposable
    {
        private readonly SynchronizationContextScope _previousScope;
        private readonly SynchronizationContext _context;
        private bool _isDisposed;      
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationContextScope" /> class.
        /// </summary>
        /// <param name="context">The context that is managed by this scope.</param>
        public SynchronizationContextScope(SynchronizationContext context)
        {
            _previousScope = BeginScope(this, _context = context);           
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            EndScope(this);

            _isDisposed = true;
        }
        
        /// <summary>
        /// The context that is managed by this scope.
        /// </summary>
        public SynchronizationContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Sends a (synchronous) message to the appropriate thread.
        /// </summary>
        /// <param name="action">The message to send.</param>
        /// <exception cref="ObjectDisposedException">
        /// The scope has already been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="SynchronizationContext" /> that is managed by this scope is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        public void Send(Action action)
        {
            if (_isDisposed)
            {
                throw NewScopeDisposedException();
            }
            if (_context == null)
            {
                throw NewContextNotSetException();
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _context.Send(state => action.Invoke(), null);
        }

        /// <summary>
        /// If associated to a <see cref="SynchronizationContext" />, posts an (asynchronous) message to the appropriate thread.
        /// </summary>
        /// <param name="action">The message to post.</param>
        /// <exception cref="ObjectDisposedException">
        /// The scope has already been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        public void Post(Action action)
        {
            if (_isDisposed)
            {
                throw NewScopeDisposedException();
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            if (_context != null)
            {
                _context.Post(state => action.Invoke(), null);
            }           
        }

        #region [====== Current Scope Management ======]

        private static readonly ThreadLocal<SynchronizationContextScope> _Current;

        static SynchronizationContextScope()
        {
            _Current = new ThreadLocal<SynchronizationContextScope>();
        }

        /// <summary>
        /// Returns the scope that is currently active, or <c>null</c> if no scope was set as the current scope.
        /// </summary>
        public static SynchronizationContextScope Current
        {
            get { return _Current.Value; }
            private set { _Current.Value = value; }
        }

        private static SynchronizationContextScope BeginScope(SynchronizationContextScope scope, SynchronizationContext context)
        {
            var previousScope = Current;

            Current = scope;
            SynchronizationContext.SetSynchronizationContext(context);

            return previousScope;
        }

        private static void EndScope(SynchronizationContextScope scope)
        {
            var currentScope = Current;
            if (currentScope != scope)
            {
                throw NewIncorrectNestingOfScopesException();
            }            
            Current = scope._previousScope;
            SynchronizationContext.SetSynchronizationContext(Current == null ? null : Current._context);
        }

        #endregion

        #region [====== Exception Factory Methods ======]

        private static Exception NewScopeDisposedException()
        {
            return new ObjectDisposedException(typeof(SynchronizationContextScope).Name);
        }

        private static Exception NewContextNotSetException()
        {
            return new InvalidOperationException(ExceptionMessages.SynchronizationContextScope_ContextNotSet);
        }

        private static Exception NewIncorrectNestingOfScopesException()
        {
            return new InvalidOperationException(ExceptionMessages.Scope_IncorrectNesting);
        }

        #endregion
    }
}
