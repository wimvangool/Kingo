namespace System.ComponentModel.Server.Caching
{
    internal sealed class ObjectCacheEntry : CacheEntry
    {
        private readonly ObjectCacheEntryMonitor _monitor;

        internal ObjectCacheEntry(object messageIn, ObjectCacheEntryMonitor monitor) : base(messageIn)
        {
            _monitor = monitor;
        }

        protected override void Invalidate(bool isPostCommit)
        {
            _monitor.RemoveCacheEntry();
        }
    }
}
