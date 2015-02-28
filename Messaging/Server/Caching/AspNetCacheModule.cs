using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// Represents a <see cref="ObjectCacheModule" /> implementation that is based on ASP.NET's
    /// <see cref="HttpApplicationState" /> and <see cref="HttpSessionState" /> instances.
    /// </summary>
    public class AspNetCacheModule : QueryCacheModule
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetCacheModule" /> class.
        /// </summary>
        /// <param name="recursionPolicy">
        /// The lock recursion policy that is used for the <see cref="QueryCacheModule.CacheManagerLock" />.
        /// </param>
        public AspNetCacheModule(LockRecursionPolicy recursionPolicy = LockRecursionPolicy.NoRecursion)
            : base(recursionPolicy) { }

        /// <inheritdoc />
        protected override bool TryGetApplicationCacheFactory(out IQueryCacheManagerFactory applicationCacheManagerFactory)
        {
            return AspNetApplicationCacheManager.TryCreateFactory(this, out applicationCacheManagerFactory);
        }

        /// <inheritdoc />
        protected override bool TryGetSessionCacheFactory(out IQueryCacheManagerFactory sessionCacheManagerFactory)
        {
            return AspNetSessionCacheManager.TryCreateFactory(this, out sessionCacheManagerFactory);
        }                      
    }
}
