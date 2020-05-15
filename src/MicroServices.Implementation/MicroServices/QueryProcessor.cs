using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class QueryProcessor : InternalMessageProcessor<MicroProcessorOperationContext>, IQueryProcessor
    {
        private readonly MicroProcessorOperationContext _context;

        public QueryProcessor(MicroProcessorOperationContext context)
        {
            _context = context;
        }

        protected override MicroProcessorOperationContext Context =>
            _context;

        public Task<TResponse> ExecuteQueryAsync<TResponse>(IQuery<TResponse> query) =>
            ExecuteQueryAsync(new QueryAdapter<TResponse>(query), CreateRequest(new VoidRequest()));

        public Task<TResponse> ExecuteQueryAsync<TRequest, TResponse>(IQuery<TRequest, TResponse> query, TRequest message) =>
            ExecuteQueryAsync(new QueryAdapter<TRequest, TResponse>(query), CreateRequest(message));

        private Task<TResponse> ExecuteQueryAsync<TRequest, TResponse>(Query<TRequest, TResponse> query, Message<TRequest> message) =>
            ExecuteOperationAsync(new QueryOperation<TRequest,TResponse>(_context, query, message, _context.Token));

        private Message<TRequest> CreateRequest<TRequest>(TRequest message) =>
            CreateMessage(MessageKind.Request, message);

        private static async Task<TResponse> ExecuteOperationAsync<TRequest, TResponse>(QueryOperation<TRequest, TResponse> operation) =>
            (await operation.ExecuteAsync()).Output.Content;
    }
}
