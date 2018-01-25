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

        public override Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync(IMicroProcessorContext context) =>
            _filter.ExecuteAsync(_nextQuery, context);

        public override void Accept(IMicroProcessorFilterVisitor visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(_filter);

                _nextQuery.Accept(visitor);
            }
        }
    }
}
