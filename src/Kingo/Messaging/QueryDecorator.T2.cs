using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    internal sealed class QueryDecorator<TMessageIn, TMessageOut> : Query<TMessageIn, TMessageOut>
    {
        private readonly QueryContext _controller;
        private readonly IQuery<TMessageIn, TMessageOut> _query;        
        
        public QueryDecorator(QueryContext controller, IQuery<TMessageIn, TMessageOut> query)
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

        public override async Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync(TMessageIn message, IMicroProcessorContext context) =>
            _controller.CreateExecuteAsyncResult(await _query.ExecuteAsync(message, context));            

        public override void Accept(IMicroProcessorFilterVisitor visitor) =>
            visitor?.Visit(_query);
    }
}
