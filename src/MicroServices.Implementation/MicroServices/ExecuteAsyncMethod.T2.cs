using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class ExecuteAsyncMethod<TRequest, TResponse> : ExecuteAsyncMethod, IQuery<TRequest, TResponse>
    {
        private readonly Query<TRequest, TResponse> _query;        

        public ExecuteAsyncMethod(Query<TRequest, TResponse> query) : base(query.GetQueryType())
        {
            _query = query;
        }                

        public Task<TResponse> ExecuteAsync(TRequest message, QueryOperationContext context) =>
            _query.ExecuteAsync(message, context);
    }
}
