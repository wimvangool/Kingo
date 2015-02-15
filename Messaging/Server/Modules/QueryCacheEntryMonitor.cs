using System.Runtime.Caching;

namespace System.ComponentModel.Server.Modules
{
    internal sealed class QueryCacheEntryMonitor : ChangeMonitor
    {
        private readonly string _uniqueId;

        internal QueryCacheEntryMonitor()
        {
            _uniqueId = Guid.NewGuid().ToString("N");

            InitializationComplete();
        }

        private QueryCacheEntryMonitor(string uniqueId)
        {
            _uniqueId = uniqueId;

            InitializationComplete();
        }

        protected override void Dispose(bool disposing) { }

        public override string UniqueId
        {
            get { return _uniqueId; }
        }

        internal void RemoveCacheEntry()
        {
            OnChanged(null);
        }

        internal QueryCacheEntryMonitor Copy()
        {
            return new QueryCacheEntryMonitor(_uniqueId);
        }
    }
}
