using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class ExecuteAsyncMethod<TResponse> : ExecuteAsyncMethod, IQuery<TResponse>
    {
        private readonly IQuery<TResponse> _query;        

        public ExecuteAsyncMethod(IQuery<TResponse> query) :
            base(QueryType.FromInstance(query))
        {
            _query = query;
        }                

        public Task<TResponse> ExecuteAsync(IQueryOperationContext context) =>
            _query.ExecuteAsync(context);

        public override string ToString() =>
            $"{Query.Type.FriendlyName()}.{MethodInfo.Name}(...)";
    }
}
