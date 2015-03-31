namespace System.ComponentModel.Server
{
    internal sealed class UnitOfWorkCache : DependencyCacheRelay
    {
        protected override bool TryGetCache(out IDependencyCache cache)
        {
            var context = UnitOfWorkContext.Current;
            if (context == null)
            {
                cache = null;
                return false;
            }
            cache = context.InternalCache;
            return true;
        }
    }
}
