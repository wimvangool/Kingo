using System.Threading.Tasks;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    internal sealed class QueryAdapter<TRequest, TResponse> : Query<TRequest, TResponse>
    {
        private readonly IQuery<TRequest, TResponse> _query;

        public QueryAdapter(IQuery<TRequest, TResponse> query)
        {
            _query = IsNotNull(query, nameof(query));
        }

        public override QueryType GetQueryType() =>
            QueryType.FromInstance(_query);

        public override Task<TResponse> ExecuteAsync(TRequest message, QueryOperationContext context) =>
            _query.ExecuteAsync(message, context);

        public override string ToString() =>
            _query.ToString();
    }
}
