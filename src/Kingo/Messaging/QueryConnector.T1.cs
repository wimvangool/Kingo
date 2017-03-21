using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class QueryConnector<TMessageOut> : Query<TMessageOut>
    {
        private readonly Query<TMessageOut> _nextQuery;
        private readonly IMicroProcessorPipeline _pipeline;

        public QueryConnector(Query<TMessageOut> nextQuery, IMicroProcessorPipeline pipeline)
        {
            _nextQuery = nextQuery;
            _pipeline = pipeline;
        }

        protected override ITypeAttributeProvider TypeAttributeProvider =>
            _nextQuery;

        protected override IMethodAttributeProvider MethodAttributeProvider =>
            _nextQuery;

        public override Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync(IMicroProcessorContext context) =>
            _pipeline.ExecuteAsync(_nextQuery, context);

        public override void Accept(IMicroProcessorPipelineVisitor visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(_pipeline);

                _nextQuery.Accept(visitor);
            }
        }
    }
}
