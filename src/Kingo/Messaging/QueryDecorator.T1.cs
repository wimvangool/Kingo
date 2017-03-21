using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    internal sealed class QueryDecorator<TMessageOut> : Query<TMessageOut>
    {
        private readonly QueryContext _controller;
        private readonly IQuery<TMessageOut> _query;        
        
        public QueryDecorator(QueryContext controller, IQuery<TMessageOut> query)
        {
            _controller = controller;
            _query = query;

            TypeAttributeProvider = new TypeAttributeProvider(query.GetType());
            MethodAttributeProvider = Messaging.MethodAttributeProvider.FromQuery(query);
        }
        
        protected override ITypeAttributeProvider TypeAttributeProvider
        {
            get;
        }
        
        protected override IMethodAttributeProvider MethodAttributeProvider
        {
            get;
        }
        
        public override async Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync(IMicroProcessorContext context) =>
            _controller.CreateExecuteAsyncResult(await _query.ExecuteAsync(context));

        public override void Accept(IMicroProcessorPipelineVisitor visitor) =>
            visitor?.Visit(_query);
    }
}
