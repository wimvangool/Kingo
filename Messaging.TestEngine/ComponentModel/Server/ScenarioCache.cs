namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a cache that can be used to store dependencies that have a lifetime that is bound to a
    /// specific <see cref="Scenario" />.
    /// </summary>
    public sealed class ScenarioCache : DependencyCacheRelay
    {
        /// <inheritdoc />
        protected override bool TryGetCache(out IDependencyCache cache)
        {
            return (cache = Scenario.Cache) != null;
        }
    }
}
