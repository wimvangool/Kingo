using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class ExecuteQueryAsyncMethod<TMessageIn, TMessageOut> : ExecuteAsyncMethod<TMessageOut>
    {
        public static Task<TMessageOut> Invoke(MicroProcessor processor, IQuery<TMessageIn, TMessageOut> query, TMessageIn message, CancellationToken? token) =>
            Invoke(new ExecuteQueryAsyncMethod<TMessageIn, TMessageOut>(processor, new QueryContext(processor.Principal, token), query, message));        

        private ExecuteQueryAsyncMethod(MicroProcessor processor, QueryContext context, IQuery<TMessageIn, TMessageOut> query, TMessageIn message)
        {
            Processor = processor;
            Context = context;            
            Query = query;
            Message = message;
        }

        protected override MicroProcessor Processor
        {
            get;
        }

        protected override QueryContext Context
        {
            get;
        }

        private IQuery<TMessageIn, TMessageOut> Query
        {
            get;
        }

        private TMessageIn Message
        {
            get;
        }

        protected override async Task<InvokeAsyncResult<TMessageOut>> InvokeQueryCore()
        {
            Context.StackTraceCore.Push(MicroProcessorOperationTypes.Query, Message);

            try
            {
                return await Processor.Pipeline.Build(new QueryDecorator<TMessageIn, TMessageOut>(Query, Message)).InvokeAsync(Context);
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
