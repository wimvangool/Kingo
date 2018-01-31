using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class QueryPipelineConnector<TMessageIn, TMessageOut> : Query<TMessageIn, TMessageOut>
    {
        private readonly Query<TMessageIn, TMessageOut> _nextQuery;
        private readonly IMicroProcessorFilter _filter;

        public QueryPipelineConnector(Query<TMessageIn, TMessageOut> nextQuery, IMicroProcessorFilter filter) :
            base(nextQuery, nextQuery)
        {
            _nextQuery = nextQuery;
            _filter = filter;
        }        

        public override Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync(TMessageIn message, IMicroProcessorContext context) =>
            _filter.InvokeQueryAsync(_nextQuery, message, context);

        public override string ToString() =>
            MicroProcessorPipeline.ToString(_filter, _nextQuery);
    }
}
