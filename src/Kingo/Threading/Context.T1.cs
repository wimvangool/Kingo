using System;
using System.Threading;

namespace Kingo.Threading
{
    /// <summary>
    /// Represents a contextual container for a specific value. All instance methods on this class are thread-safe.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to store.</typeparam>
    public sealed class Context<TValue> : IDisposable
    {
        #region [====== Scopes ======]       

        private sealed class ThreadLocalScope<T> : ContextScope<T>
        {
            private readonly Context<T> _context;
            private readonly Tuple<T> _oldValue;
            private readonly Tuple<T> _newValue;

            internal ThreadLocalScope(Context<T> context, Tuple<T> oldValue, Tuple<T> newValue)
            {
                _context = context;
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override T Value =>
             _newValue.Item1;

            internal override bool RestoreOldValue()
            {
                if (_context._currentThreadLocal.Value == _newValue)
                {
                    _context._currentThreadLocal.Value = _oldValue;
                    return true;
                }
                return false;
            }
        }

        private sealed class AsyncLocalScope<T> : ContextScope<T>
        {
            private readonly Context<T> _context;
            private readonly Tuple<T> _oldValue;
            private readonly Tuple<T> _newValue;

            internal AsyncLocalScope(Context<T> context, Tuple<T> oldValue, Tuple<T> newValue)
            {
                _context = context;
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override T Value =>
             _newValue.Item1;

            internal override bool RestoreOldValue()
            {
                _context._readerWriterLock.EnterWriteLock();

                try
                {
                    return RestoreOldValueCore();
                }
                finally
                {
                    _context._readerWriterLock.ExitWriteLock();
                }
            }

            private bool RestoreOldValueCore()
            {
                if (_context._currentThreadLocal.Value == null)
                if (_context._currentAsyncLocal.Value == _newValue)
                {
                    _context._currentAsyncLocal.Value = _oldValue;
                    return true;
                }
                return false;
            }
        }

        private sealed class DefaultScope<T> : ContextScope<T>
        {
            private readonly Context<T> _context;
            private readonly Tuple<T> _oldValue;
            private readonly Tuple<T> _newValue;

            internal DefaultScope(Context<T> context, Tuple<T> oldValue, Tuple<T> newValue)
            {
                _context = context;
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override T Value =>
             _newValue.Item1;

            internal override bool RestoreOldValue()
            {
                _context._readerWriterLock.EnterWriteLock();

                try
                {
                    return RestoreOldValueCore();
                }
                finally
                {
                    _context._readerWriterLock.ExitWriteLock();
                }
            }

            private bool RestoreOldValueCore()
            {
                if (_context._currentThreadLocal.Value == null)
                if (_context._currentAsyncLocal.Value == null)
                if (_context._currentDefault == _newValue)
                {
                    _context._currentDefault = _oldValue;
                    return true;
                }
                return false;
            }
        }

        #endregion

        private readonly ReaderWriterLockSlim _readerWriterLock;
        private readonly ThreadLocal<Tuple<TValue>> _currentThreadLocal;
        private readonly AsyncLocal<Tuple<TValue>> _currentAsyncLocal;
        private Tuple<TValue> _currentDefault;
        private bool _isDisposed;
 
        /// <summary>
        /// Initializes a new instance of the <see cref="Context{T}" /> class.
        /// </summary>
        public Context()
            : this(default(TValue)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Context{T}" /> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        public Context(TValue defaultValue)
        {
            _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _currentThreadLocal = new ThreadLocal<Tuple<TValue>>();
            _currentAsyncLocal = new AsyncLocal<Tuple<TValue>>();
            _currentDefault = new Tuple<TValue>(defaultValue);
        }

        /// <summary>
        /// Returns the value of the item that is current with respect to the current thread.
        /// </summary>
        public TValue Current =>
            ReadCurrent().Item1;

        private bool IsInsideThreadLocalScope =>
            _currentThreadLocal.Value != null;

        private bool IsInsideAsyncLocalScope =>
            _currentAsyncLocal.Value != null;

        private Tuple<TValue> ReadCurrent()
        {
            // The ThreadLocal value can be checked and returned without using the lock
            // since it's thread-safe by definition.
            var threadLocalValue = _currentThreadLocal.Value;
            if (threadLocalValue != null)
            {
                return threadLocalValue;
            }
            _readerWriterLock.EnterReadLock();

            try
            {
                var asyncLocalValue = _currentAsyncLocal.Value;
                if (asyncLocalValue != null)
                {
                    return asyncLocalValue;
                }
                return _currentDefault;
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }       

        /// <summary>
        /// Sets the current value that is accessible by the current thread through <see cref="Current" />
        /// only as long as the scope is active.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>The scope that is to be disposed when ended.</returns>
        public ContextScope<TValue> OverrideThreadLocal(TValue value)
        {
            if (_isDisposed)
            {
                throw NewContextDisposedException();
            }
            var oldValue = _currentThreadLocal.Value;
            var newValue = _currentThreadLocal.Value = new Tuple<TValue>(value);

            return new ThreadLocalScope<TValue>(this, oldValue, newValue);
        }        

        /// <summary>
        /// Sets the current value that is accessible by all threads that share the same <see cref="LogicalCallContext" />
        /// through <see cref="Current" /> as long as the scope is active.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>The scope that is to be disposed when ended.</returns>
        /// <exception cref="InvalidOperationException">
        /// The call is made inside a thread local scope.
        /// </exception>
        public ContextScope<TValue> OverrideAsyncLocal(TValue value)
        {
            if (_isDisposed)
            {
                throw NewContextDisposedException();
            }
            if (IsInsideThreadLocalScope)
            {
                throw NewIllegalScopeStartedException();
            }
            _readerWriterLock.EnterWriteLock();

            try
            {
                return OverrideAsyncLocalCore(value);
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        private ContextScope<TValue> OverrideAsyncLocalCore(TValue value)
        {
            var oldValue = _currentAsyncLocal.Value;
            var newValue = _currentAsyncLocal.Value = new Tuple<TValue>(value);

            return new AsyncLocalScope<TValue>(this, oldValue, newValue);
        }

        /// <summary>
        /// Sets the current value that is accessible by all threads
        /// through <see cref="Current" /> as long as the scope is active.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>The scope that is to be disposed when ended.</returns>
        /// <exception cref="InvalidOperationException">
        /// The call is made inside an async local or thread local scope.
        /// </exception>
        public ContextScope<TValue> Override(TValue value)
        {
            if (_isDisposed)
            {
                throw NewContextDisposedException();
            }
            if (IsInsideThreadLocalScope)
            {
                throw NewIllegalScopeStartedException();
            }
            _readerWriterLock.EnterWriteLock();

            try
            {
                return OverrideCore(value);
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        private ContextScope<TValue> OverrideCore(TValue value)
        {
            if (IsInsideAsyncLocalScope)
            {
                throw NewIllegalScopeStartedException();
            }
            var oldValue = _currentDefault;
            var newValue = _currentDefault = new Tuple<TValue>(value);

            return new DefaultScope<TValue>(this, oldValue, newValue);
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
            _readerWriterLock.Dispose();
            _isDisposed = true;
        }

        private static Exception NewContextDisposedException() =>
             new ObjectDisposedException(typeof(Context<TValue>).Name);

        private static Exception NewIllegalScopeStartedException() =>
             new InvalidOperationException(ExceptionMessages.Context_IllegalScopeStarted);
    }
}
