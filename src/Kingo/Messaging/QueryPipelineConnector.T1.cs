using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class QueryPipelineConnector<TMessageOut> : Query<TMessageOut>
    {
        private readonly Query<TMessageOut> _nextQuery;
        private readonly IMicroProcessorFilter _filter;

        public QueryPipelineConnector(Query<TMessageOut> nextQuery, IMicroProcessorFilter filter) :
            base(nextQuery, nextQuery)
        {
            _nextQuery = nextQuery;
            _filter = filter;
        }        

        public override Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync(IMicroProcessorContext context) =>
            _filter.InvokeQueryAsync(_nextQuery, context);

        public override string ToString() =>
            MicroProcessorPipeline.ToString(_filter, _nextQuery);
    }
}
