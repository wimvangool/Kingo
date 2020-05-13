using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class ExecuteAsyncMethod<TRequest, TResponse> : ExecuteAsyncMethod, IQuery<TRequest, TResponse>
    {
        private readonly IQuery<TRequest, TResponse> _query;        

        public ExecuteAsyncMethod(IQuery<TRequest, TResponse> query) :
            base(QueryType.FromInstance(query))
        {
            _query = query;
        }                

        public Task<TResponse> ExecuteAsync(TRequest message, QueryOperationContext context) =>
            _query.ExecuteAsync(message, context);

        public override string ToString() =>
            $"{Query.Type.FriendlyName()}.{MethodInfo.Name}({MessageParameterInfo.ParameterType.FriendlyName()}, ...)";
    }
}
