using System.Web;

namespace System.ComponentModel.Server.Caching
{
    internal sealed class AspNetApplicationCacheManager : AspNetCacheManager
    {
        #region [====== Factory ======]

        private sealed class ApplicationCacheManagerFactory : QueryCacheManagerFactory<HttpApplicationState>
        {
            private readonly AspNetCacheProvider _cacheProvider;

            internal ApplicationCacheManagerFactory(HttpApplicationState cache, AspNetCacheProvider cacheProvider)
                : base(cache)
            {
                _cacheProvider = cacheProvider;
            }

            public override QueryCacheManager CreateCacheManager()
            {
                return new AspNetApplicationCacheManager(_cacheProvider, Cache);
            }
        }

        #endregion

        private readonly HttpApplicationState _cache;

        private AspNetApplicationCacheManager(AspNetCacheProvider cacheProvider, HttpApplicationState cache)
            : base(cacheProvider)
        {
            _cache = cache;
        }        

        protected override bool TryGetCacheEntry(string key, out object value)
        {
            return (value = _cache[key]) != null;
        }

        protected override bool ContainsCacheEntry(string key)
        {
            return _cache[key] != null;
        }

        protected override void InsertCacheEntry(string key, object value)
        {
            _cache.Add(key, value);
        }

        protected override void UpdateCacheEntry(string key, object value)
        {
            _cache[key] = value;
        }

        protected override void DeleteCacheEntry(string key)
        {
            _cache.Remove(key);
        }        

        internal static bool TryCreateFactory(AspNetCacheProvider cacheProvider, out IQueryCacheManagerFactory cacheManagerFactory)
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
            {
                cacheManagerFactory = null;
                return false;
            }            
            cacheManagerFactory = new ApplicationCacheManagerFactory(httpContext.Application, cacheProvider);
            return true;
        }
    }
}
