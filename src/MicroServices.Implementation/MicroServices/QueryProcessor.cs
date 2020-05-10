using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class QueryProcessor : IQueryProcessor
    {
        private readonly MicroProcessorOperationContext _context;

        public QueryProcessor(MicroProcessorOperationContext context)
        {
            _context = context;
        }

        private CancellationToken? Token =>
            _context.StackTrace.CurrentOperation?.Token;

        public Task<TResponse> ExecuteQueryAsync<TResponse>(IQuery<TResponse> query) =>
            ExecuteOperationAsync(new QueryOperationImplementation<TResponse>(_context, query, Token));

        public Task<TResponse> ExecuteQueryAsync<TRequest, TResponse>(IQuery<TRequest, TResponse> query, TRequest message) =>
            ExecuteQueryAsync(query, _context.Processor.MessageFactory.CreateRequest(MessageDirection.Internal, MessageHeader.Unspecified, message));

        private Task<TResponse> ExecuteQueryAsync<TRequest, TResponse>(IQuery<TRequest, TResponse> query, Message<TRequest> message) =>
            ExecuteOperationAsync(new QueryOperationImplementation<TRequest, TResponse>(_context, query, message.CorrelateWith(_context.StackTrace.CurrentOperation.Message), Token));

        private static async Task<TResponse> ExecuteOperationAsync<TResponse>(QueryOperation<TResponse> operation) =>
            GetResponse(await operation.ExecuteAsync());

        private static TResponse GetResponse<TResponse>(QueryOperationResult<TResponse> result) =>
            result.Output.Content;
    }
}
