namespace YellowFlare.MessageProcessing.Messages
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
