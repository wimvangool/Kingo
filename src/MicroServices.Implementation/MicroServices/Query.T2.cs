using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal abstract class Query<TRequest, TResponse> : IQuery<TRequest, TResponse>
    {
        public abstract QueryType GetQueryType();

        public abstract Task<TResponse> ExecuteAsync(TRequest message, QueryOperationContext context);
    }
}
