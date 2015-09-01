namespace Kingo.BuildingBlocks.ComponentModel.Server
{
    /// <summary>
    /// Represents a cache that can be used to store dependencies that have a lifetime that is bound to a
    /// specific <see cref="UnitOfWorkContext" />.
    /// </summary>
    public sealed class UnitOfWorkCache : DependencyCacheRelay
    {
        /// <inheritdoc />
        protected override bool TryGetCache(out IDependencyCache cache)
        {
            var context = UnitOfWorkContext.Current;
            if (context == null)
            {
                cache = null;
                return false;
            }
            cache = context.Cache;
            return true;
        }
    }
}
