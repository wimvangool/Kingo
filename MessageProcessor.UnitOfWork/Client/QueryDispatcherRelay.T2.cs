using System.Threading;

namespace YellowFlare.MessageProcessing.Client
{
    internal sealed class QueryDispatcherRelay<TQuery, TResult> : QueryDispatcher<TQuery, TResult>        
        where TQuery : Query<TQuery, TResult>
    {
        private readonly TQuery _query;

        public QueryDispatcherRelay(TQuery query) : base(query)
        {
            _query = query;
        }

        protected override TResult Execute(TQuery message, CancellationToken? token)
        {
            return _query.Execute(message, token);
        }

        protected override QueryCacheValue CreateCacheValue(object key, TResult result)
        {
            return _query.CreateCacheValue(key, result);
        }        
    }
}
