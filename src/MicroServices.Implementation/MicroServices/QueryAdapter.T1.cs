using System.Threading.Tasks;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    internal sealed class QueryAdapter<TResponse> : Query<VoidRequest, TResponse>
    {
        private readonly IQuery<TResponse> _query;

        public QueryAdapter(IQuery<TResponse> query)
        {
            _query = IsNotNull(query, nameof(query));
        }

        public override QueryType GetQueryType() =>
            QueryType.FromInstance(_query);

        public override Task<TResponse> ExecuteAsync(VoidRequest message, QueryOperationContext context) =>
            _query.ExecuteAsync(context);

        public override string ToString() =>
            _query.ToString();
    }
}
