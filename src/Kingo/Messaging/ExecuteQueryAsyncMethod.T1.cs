using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class ExecuteQueryAsyncMethod<TMessageOut> : ExecuteAsyncMethod<TMessageOut>
    {
        public static Task<TMessageOut> Invoke(MicroProcessor processor, IQuery<TMessageOut> query, CancellationToken? token) =>
            Invoke(new ExecuteQueryAsyncMethod<TMessageOut>(processor, new QueryContext(processor.Principal, token), query));

        private readonly IQuery<TMessageOut> _query;

        private ExecuteQueryAsyncMethod(MicroProcessor processor, QueryContext context, IQuery<TMessageOut> query)
        {
            Processor = processor;
            Context = context;            

            _query = query;
        }

        protected override MicroProcessor Processor
        {
            get;
        }

        protected override QueryContext Context
        {
            get;
        }

        protected override async Task<InvokeAsyncResult<TMessageOut>> InvokeQueryCore()
        {
            Context.StackTraceCore.Push(MessageInfo.FromQuery());

            try
            {
                return await Processor.Pipeline.Build(new QueryDecorator<TMessageOut>(_query)).InvokeAsync(Context);
            }
            finally
            {
                Context.StackTraceCore.Pop();
            }
        }            

        protected override BadRequestException NewBadRequestException(InternalProcessorException exception, string message) =>
            exception.AsBadRequestException(message);

        protected override InternalServerErrorException NewInternalServerErrorException(InternalProcessorException exception, string message) =>
            exception.AsInternalServerErrorException(message);

        protected override InternalServerErrorException NewInternalServerErrorException(Exception exception, string message) =>
            new InternalServerErrorException(message, exception);
    }
}
