using System.Runtime.Caching;

namespace System.ComponentModel.Server.Caching
{
    internal sealed class ObjectCacheEntryMonitor : ChangeMonitor
    {
        private readonly string _uniqueId;

        internal ObjectCacheEntryMonitor()
        {
            _uniqueId = Guid.NewGuid().ToString("N");

            InitializationComplete();
        }

        private ObjectCacheEntryMonitor(string uniqueId)
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

        internal ObjectCacheEntryMonitor Copy()
        {
            return new ObjectCacheEntryMonitor(_uniqueId);
        }
    }
}
