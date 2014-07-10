using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a lifetime that never expires.
    /// </summary>
    internal sealed class InfiniteLifetime : QueryCacheValueLifetime
    {
        protected override void Run() { }        

        internal override QueryCacheValueLifetime CombineWith(QueryCacheValueLifetime lifetime, bool isSecondAttempt)
        {
            return lifetime;
        }        
    }
}
