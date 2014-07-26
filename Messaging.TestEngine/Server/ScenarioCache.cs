namespace System.ComponentModel.Messaging.Server
{
    internal sealed class ScenarioCache : ScopeSpecificCacheRelay
    {
        protected override bool TryGetCache(out IScopeSpecificCache cache)
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
