namespace YellowFlare.MessageProcessing.Requests
{
    internal sealed class OrLifetime : QueryCacheValueLifetime
    {
        private readonly QueryCacheValueLifetime _left;
        private readonly QueryCacheValueLifetime _right;

        internal OrLifetime(QueryCacheValueLifetime left, QueryCacheValueLifetime right)
        {
            _left = left;
            _left.Expired += (s, e) => OnExpired();
            _right = right;
            _right.Expired += (s, e) => OnExpired();
        }        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _left.Dispose();
                _right.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void Run()
        {
            _left.Start();
            _right.Start();
        }                      
    }
}
