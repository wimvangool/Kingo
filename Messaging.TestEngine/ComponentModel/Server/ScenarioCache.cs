namespace System.ComponentModel.Server
{
    internal sealed class ScenarioCache : DependencyCacheRelay
    {
        protected override bool TryGetCache(out IDependencyCache cache)
        {
            var scenario = Scenario.Current;
            if (scenario == null)
            {
                cache = null;
                return false;
            }
            cache = scenario.InternalCache;
            return true;
        }
    }
}
