using System;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing.Messages
{
    /// <summary>
    /// Provides a base-implementation of the <see cref="QueryCacheValueLifetime" /> interface.
    /// </summary>
    public abstract class QueryCacheValueLifetime : IDisposable
    {
        private readonly object _lock;        

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCacheValueLifetime" /> class.
        /// </summary>
        protected QueryCacheValueLifetime()
        {
            _lock = new object();
        }

        #region [====== Disposing ======]

        /// <summary>
        /// Indicates whether or not this instance has been disposed.
        /// </summary>
        protected bool IsDisposed
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
        /// Creates and returns an exception that can be thrown when an instance method is called while
        /// this instance has already been disposed of.
        /// </summary>
        /// <returns>The exception to throw.</returns>
        protected ObjectDisposedException NewLifetimeIsDisposedException()
        {
            return new ObjectDisposedException(GetType().Name);
        }

        #endregion

        /// <summary>
        /// Occurs when this lifetime has just expired. This event is guarantueed only te be raised at most once.
        /// </summary>
        /// <remarks>
        /// Note that this event may be raised on a different thread than the one this lifetime was created on. Most lifetimes
        /// will expire when a timer elapsed or some other external signal comes in on a different thread. 
        /// </remarks>
        public event EventHandler Expired;

        /// <summary>
        /// Raises the <see cref="Expired" /> event if it wasn't already raised before.
        /// </summary>
        protected virtual void OnExpired()
        {
            if (IsExpired)
            {
                return;
            }
            lock (_lock)
            {
                if (IsExpired)
                {
                    return;
                }
                IsExpired = true;

                Expired.Raise(this);
            }
        }

        /// <summary>
        /// Indicates whether or not this lifetime has started.
        /// </summary>
        public bool HasStarted
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates whether or not this lifetime is (still) alive.
        /// </summary>        
        public bool IsAlive
        {
            get { return HasStarted && !IsExpired; }
        }

        /// <summary>
        /// Indicates whether or not this lifetime has expired.
        /// </summary>
        public bool IsExpired
        {
            get;
            private set;
        }

        /// <summary>
        /// Starts this lifetime.
        /// </summary>
        internal void Start()
        {
            if (IsDisposed)
            {
                throw NewLifetimeIsDisposedException();
            }
            if (HasStarted)
            {
                throw NewCannotStartException();
            }            
            Run();
            HasStarted = true;
        }

        /// <summary>
        /// Marks the beginning of this lifetime.
        /// </summary>
        protected abstract void Run();

        /// <summary>
        /// Notifies this lifetime that the associated value has been accessed.
        /// </summary>
        public virtual void NotifyValueAccessed() { }

        /// <summary>
        /// Returns a lifetime that expires when either this or the specified lifetime expires.
        /// </summary>
        /// <param name="lifetime">Another lifetime.</param>
        /// <returns>The combined lifetime.</returns>
        public QueryCacheValueLifetime Or(QueryCacheValueLifetime lifetime)
        {    
            if (IsDisposed)
            {
                throw NewLifetimeIsDisposedException();
            }
            if (HasStarted)
            {
                throw NewCannotCombineException();
            }            
            if (lifetime == null)
            {
                throw new ArgumentNullException("lifetime");
            }
            if (lifetime.HasStarted)
            {
                throw NewCannotCombineException("lifetime");
            }
            if (lifetime == this)
            {
                return this;
            }
            return lifetime.CombineWith(this, false);
        }

        internal virtual QueryCacheValueLifetime CombineWith(InfiniteLifetime lifetime)
        {
            return this;
        }        

        internal virtual QueryCacheValueLifetime CombineWith(QueryCacheValueLifetime lifetime, bool isSecondAttempt)
        {
            if (isSecondAttempt)
            {
                return new OrLifetime(this, lifetime);
            }
            return lifetime.CombineWith(this, true);
        }

        private static Exception NewCannotStartException()
        {
            return new InvalidOperationException(ExceptionMessages.QueryCacheValueLifetime_AlreadyStarted);
        }

        private static Exception NewCannotCombineException()
        {
            return new InvalidOperationException(ExceptionMessages.QueryCacheValueLifetime_ThisAlreadyStarted);
        }

        private static Exception NewCannotCombineException(string paramName)
        {
            return new ArgumentException(ExceptionMessages.QueryCacheValueLifetime_OtherAlreadyStarted, paramName);
        }
    }
}
