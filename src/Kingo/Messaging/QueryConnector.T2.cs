using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class QueryConnector<TMessageIn, TMessageOut> : Query<TMessageIn, TMessageOut>
    {
        private readonly Query<TMessageIn, TMessageOut> _nextQuery;
        private readonly IMicroProcessorPipeline _pipeline;

        public QueryConnector(Query<TMessageIn, TMessageOut> nextQuery, IMicroProcessorPipeline pipeline)
        {
            _nextQuery = nextQuery;
            _pipeline = pipeline;
        }

        protected override ITypeAttributeProvider TypeAttributeProvider =>
            _nextQuery;

        protected override IMethodAttributeProvider MethodAttributeProvider =>
            _nextQuery;

        public override Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync(TMessageIn message, IMicroProcessorContext context) =>
            _pipeline.ExecuteAsync(_nextQuery, message, context);

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
