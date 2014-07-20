using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YellowFlare.MessageProcessing.TestEngine
{
    internal sealed class ScenarioCache : CacheRelay
    {
        protected override bool TryGetCache(out ICache cache)
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
