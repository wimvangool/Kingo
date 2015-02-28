using System.Timers;
using System.Web;
using System.Web.SessionState;

namespace System.ComponentModel.Server.Caching
{
    internal sealed class AspNetSessionCacheManager : AspNetCacheManager
    {        
        private readonly HttpSessionState _cache;
        private readonly Timer _expirationTimer;

        private AspNetSessionCacheManager(AspNetCacheModule cacheModule, HttpSessionState cache)
            : base(cacheModule)
        {
            _cache = cache;
            _expirationTimer = new Timer(TimerIntervalFromMinutes(cache.Timeout));
            _expirationTimer.Elapsed += HandleSessionExpired;
            _expirationTimer.AutoReset = false;
            _expirationTimer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                _expirationTimer.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override bool TryGetCacheEntry(string key, out object value)
        {
            ExtendLifetime();

            return (value = _cache[key]) != null;
        }

        protected override bool ContainsCacheEntry(string key)
        {
            ExtendLifetime();

            return _cache[key] != null;
        }

        protected override void InsertCacheEntry(string key, object value)
        {
            ExtendLifetime();

            _cache.Add(key, value);
        }

        protected override void UpdateCacheEntry(string key, object value)
        {
            ExtendLifetime();

            _cache[key] = value;
        }

        protected override void DeleteCacheEntry(string key)
        {
            ExtendLifetime();

            _cache.Remove(key);
        }

        #region [====== Expiration ======]

        private void ExtendLifetime()
        {
            _expirationTimer.Stop();
            _expirationTimer.Start();
        }

        private void HandleSessionExpired(object sender, EventArgs e)
        {
            CacheProvider.OnCacheExpired(new CacheExpiredEventArgs(_cache));
        }

        private static double TimerIntervalFromMinutes(int timeoutInMinutes)
        {            
            // Two minutes are added as slack to ensure the HttpSessionState-instance has actually expired.
            return TimeSpan.FromMinutes(timeoutInMinutes + 2).TotalMilliseconds;
        }

        #endregion

        #region [====== Factory ======]

        private sealed class SessionCacheManagerFactory : QueryCacheManagerFactory<HttpSessionState>
        {
            private readonly AspNetCacheModule _cacheModule;

            internal SessionCacheManagerFactory(HttpSessionState cache, AspNetCacheModule cacheModule)
                : base(cache)
            {
                _cacheModule = cacheModule;
            }

            public override QueryCacheManager CreateCacheManager()
            {
                return new AspNetSessionCacheManager(_cacheModule, Cache);
            }
        }

        internal static bool TryCreateFactory(AspNetCacheModule cacheModule, out IQueryCacheManagerFactory cacheManagerFactory)
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
            {
                cacheManagerFactory = null;
                return false;
            }
            cacheManagerFactory = new SessionCacheManagerFactory(httpContext.Session, cacheModule);
            return true;
        }

        #endregion
    }
}
