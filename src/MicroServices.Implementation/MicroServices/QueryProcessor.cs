using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
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

        public Task<TResponse> ExecuteQueryAsync<TResponse>(IQuery<TResponse> query) =>
            ExecuteQueryAsync(new QueryAdapter<TResponse>(query), CreateRequest(new VoidRequest()));

        public Task<TResponse> ExecuteQueryAsync<TRequest, TResponse>(IQuery<TRequest, TResponse> query, TRequest message) =>
            ExecuteQueryAsync(new QueryAdapter<TRequest, TResponse>(query), CreateRequest(message));

        private Task<TResponse> ExecuteQueryAsync<TRequest, TResponse>(Query<TRequest, TResponse> query, Message<TRequest> message) =>
            ExecuteOperationAsync(new QueryOperation<TRequest,TResponse>(_context, query, message, _context.Token));

        private Message<TRequest> CreateRequest<TRequest>(TRequest message) =>
            CreateRequestMessage(message).CorrelateWith(_context.StackTrace.CurrentOperation.Message);

        private Message<TRequest> CreateRequestMessage<TRequest>(TRequest message) =>
            _context.Processor.MessageFactory.CreateRequest(MessageDirection.Internal, MessageHeader.Unspecified, message);

        private static async Task<TResponse> ExecuteOperationAsync<TRequest, TResponse>(QueryOperation<TRequest, TResponse> operation) =>
            (await operation.ExecuteAsync()).Output.Content;
    }
}
