using System.Runtime.Caching;

namespace System.ComponentModel.Server.Modules
{
    internal sealed class QueryCacheEntryMonitor : ChangeMonitor
    {
        private readonly string _uniqueId;

        internal QueryCacheEntryMonitor()
        {
            _uniqueId = Guid.NewGuid().ToString();

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
    }
}
