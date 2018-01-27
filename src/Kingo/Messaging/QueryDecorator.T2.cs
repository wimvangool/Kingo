using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    internal sealed class QueryDecorator<TMessageIn, TMessageOut> : Query<TMessageIn, TMessageOut>
    {
        private readonly QueryContext _controller;
        private readonly IQuery<TMessageIn, TMessageOut> _query;        
        
        public QueryDecorator(QueryContext controller, IQuery<TMessageIn, TMessageOut> query) :
            base(new TypeAttributeProvider(query.GetType()), Messaging.MethodAttributeProvider.FromQuery(query))
        {
            _controller = controller;
            _query = query;
        }              

        public override async Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync(TMessageIn message, IMicroProcessorContext context) =>
            _controller.CreateExecuteAsyncResult(await _query.ExecuteAsync(message, context));

        public override string ToString() =>
            MicroProcessorPipeline.ToString(_query);
    }
}
