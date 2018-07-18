using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class QueryConnector<TMessageOut> : Query<TMessageOut>
    {
        private readonly Query<TMessageOut> _nextQuery;
        private readonly IMicroProcessorFilter _filter;

        public QueryConnector(Query<TMessageOut> nextQuery, IMicroProcessorFilter filter)            
        {
            _nextQuery = nextQuery;
            _filter = filter;
        }

        protected override ITypeAttributeProvider TypeAttributeProvider =>
            _nextQuery;

        protected override IMethodAttributeProvider MethodAttributeProvider =>
            _nextQuery;

        public override Task<InvokeAsyncResult<TMessageOut>> InvokeAsync(MicroProcessorContext context)
        {
            if (_filter.IsEnabled(context))
            {
                return _filter.InvokeQueryAsync(_nextQuery, context);
            }
            return _nextQuery.InvokeAsync(context);
        }

        public override string ToString() =>
            MicroProcessorPipeline.ToString(_filter, _nextQuery);
    }
}
