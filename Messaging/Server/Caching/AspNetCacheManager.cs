using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DefaultNamespace;
using Timer = System.Timers.Timer;

namespace System.ComponentModel.Server.Caching
{
    internal abstract class AspNetCacheManager : QueryCacheManager
    {
        #region [====== AspNetCacheEntry ======]

        private sealed class AspNetCacheEntry : CacheEntry
        {
            private readonly string _key;
            private readonly AspNetCacheManager _cacheManager;
            private readonly AspNetCacheEntryLifetime? _absoluteLifetime;
            private AspNetCacheEntryLifetime? _slidingLifetime;

            internal AspNetCacheEntry(object messageIn, AspNetCacheManager cacheManager, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
                : this(messageIn, cacheManager, absoluteExpiration, slidingExpiration, Guid.NewGuid().ToString("N")) { }

            private AspNetCacheEntry(object messageIn, AspNetCacheManager cacheManager, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, string key)
                : base(messageIn)
            {
                _key = key;
                _cacheManager = cacheManager;
                _absoluteLifetime = AspNetCacheEntryLifetime.Start(absoluteExpiration);
                _slidingLifetime = AspNetCacheEntryLifetime.Start(slidingExpiration);
            }

            internal string Key
            {
                get { return _key; }
            }

            internal void ExtendLifetime()
            {
                if (_slidingLifetime.HasValue)
                {
                    _slidingLifetime = _slidingLifetime.Value.ExtendLifetime();
                }
            }

            internal void InvalidateIfRequired()
            {
                if (HasExpired())
                {
                    Invalidate(false);
                }
            }

            private bool HasExpired()
            {
                return
                    (_absoluteLifetime.HasValue && _absoluteLifetime.Value.HasExpired()) ||
                    (_slidingLifetime.HasValue && _slidingLifetime.Value.HasExpired());
            }

            protected override void Invalidate(bool isPostCommit)
            {
                _cacheManager.DeleteCacheEntryPostCommit(MessageIn, _ExpirationTimerLockWaitTimeout);
            }

            internal AspNetCacheEntry Update(TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
            {
                return new AspNetCacheEntry(MessageIn, _cacheManager, absoluteExpiration, slidingExpiration, _key);
            }
        }

        #endregion

        private static readonly TimeSpan _ExpirationTimerLockWaitTimeout = TimeSpan.FromMilliseconds(5);
        private readonly AspNetCacheProvider _cacheProvider;
        private readonly Dictionary<object, AspNetCacheEntry> _cacheEntryKeys;
        private readonly Timer _cacheEntryExpirationTimer;

        protected AspNetCacheManager(AspNetCacheProvider cacheProvider) : base(LockRecursionPolicy.NoRecursion)
        {
            _cacheProvider = cacheProvider;
            _cacheEntryKeys = new Dictionary<object, AspNetCacheEntry>();
            _cacheEntryExpirationTimer = new Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
            _cacheEntryExpirationTimer.AutoReset = true;
            _cacheEntryExpirationTimer.Elapsed += HandleItemExpirationTimerElapsed;
            _cacheEntryExpirationTimer.Start();
        }

        protected override QueryCacheProvider CacheProvider
        {
            get { return _cacheProvider; }
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                _cacheEntryExpirationTimer.Stop();
                _cacheEntryExpirationTimer.Dispose();
            }
            // We synchronize the base invocation here because the expiration timer might
            // raise its Elapsed event just when the manager is being disposed, in which case
            // we do not want to handle the Elapsed event anymore.
            lock (_cacheEntryExpirationTimer)
            {
                base.Dispose(disposing);
            }
        }

        protected override bool TryGetCacheEntry(object messageIn, out object messageOut)
        {
            AspNetCacheEntry cacheEntry;

            if (_cacheEntryKeys.TryGetValue(messageIn, out cacheEntry) && TryGetCacheEntry(cacheEntry.Key, out messageOut))
            {
                cacheEntry.ExtendLifetime();
                return true;
            }
            messageOut = null;
            return false;
        }

        protected abstract bool TryGetCacheEntry(string key, out object value);

        protected override bool ContainsCacheEntry(object messageIn)
        {
            AspNetCacheEntry cacheEntry;

            if (_cacheEntryKeys.TryGetValue(messageIn, out cacheEntry))
            {
                cacheEntry.ExtendLifetime();

                return ContainsCacheEntry(cacheEntry.Key);
            }
            return false;
        }

        protected abstract bool ContainsCacheEntry(string key);

        protected override void InsertCacheEntry(object messageIn, object messageOut, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
        {            
            var cacheEntry = new AspNetCacheEntry(messageIn, this, absoluteExpiration, slidingExpiration);

            _cacheEntryKeys.Add(messageIn, cacheEntry);

            InsertCacheEntry(cacheEntry.Key, messageOut);            
        }

        protected abstract void InsertCacheEntry(string key, object value);

        protected override bool UpdateCacheEntry(object messageIn, object messageOut, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
        {
            AspNetCacheEntry cacheEntry;

            if (_cacheEntryKeys.TryGetValue(messageIn, out cacheEntry))
            {
                _cacheEntryKeys[messageIn] = cacheEntry.Update(absoluteExpiration, slidingExpiration);

                UpdateCacheEntry(cacheEntry.Key, messageOut);
                return true;
            }
            return false;
        }

        protected abstract void UpdateCacheEntry(string key, object value);

        protected override bool DeleteCacheEntry(object messageIn)
        {
            AspNetCacheEntry cacheEntry;

            if (_cacheEntryKeys.TryGetValue(messageIn, out cacheEntry))
            {
                _cacheEntryKeys.Remove(messageIn);

                DeleteCacheEntry(cacheEntry.Key);
                return true;
            }
            return false;
        }

        protected abstract void DeleteCacheEntry(string key);

        private void HandleItemExpirationTimerElapsed(object sender, EventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }
            // We synchronize here to make sure we do not handle the event when the
            // manager has been disposed, which could cause an ObjectDisposedException
            // to be thrown and leave the worker-thread in an error state.
            lock (_cacheEntryExpirationTimer)
            {
                if (IsDisposed)
                {
                    return;
                }
                foreach (var cacheEntry in SelectCacheEntries())
                {
                    cacheEntry.InvalidateIfRequired();
                }
            }
        }

        private IEnumerable<AspNetCacheEntry> SelectCacheEntries()
        {
            // To prevent the cleanup process to interfere with running queries too much,
            // we only attempt to obtain a read lock here. If it's not obtained in a timely
            // fashion, we assume the manager is busy handling queries and we just skip
            // cleanup (by returning an empty collection).
            if (CacheLock.TryEnterReadLock(_ExpirationTimerLockWaitTimeout))
            {
                try
                {
                    return _cacheEntryKeys.Values.ToArray();
                }
                finally
                {
                    CacheLock.ExitReadLock();
                }
            }
            return Enumerable.Empty<AspNetCacheEntry>();
        }

        protected override IEnumerable<CacheEntry> SelectCacheEntriesWithKeyOfType(Type keyType)
        {
            return from cacheEntry in _cacheEntryKeys.Values
                   where keyType.IsInstanceOfType(cacheEntry.MessageIn)
                   select cacheEntry;
        }
    }
}
