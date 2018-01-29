using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    internal sealed class QueryDecorator<TMessageOut> : Query<TMessageOut>
    {
        private readonly QueryContext _controller;
        private readonly IQuery<TMessageOut> _query;        
        
        public QueryDecorator(QueryContext controller, IQuery<TMessageOut> query) :
            base(new TypeAttributeProvider(query.GetType()), Messaging.MethodAttributeProvider.FromQuery(query))
        {
            _controller = controller;
            _query = query;
        }               
        
        public override async Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync(IMicroProcessorContext context) =>
            _controller.CreateExecuteAsyncResult(await _query.ExecuteAsync(context));

        public override string ToString() =>
            MicroProcessorPipeline.ToString(_query);
    }
}
