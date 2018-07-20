using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class ExecuteQueryAsyncMethod<TMessageOut> : ExecuteAsyncMethod<TMessageOut>
    {
        public static Task<TMessageOut> Invoke(MicroProcessor processor, IQuery<TMessageOut> query, CancellationToken? token) =>
            Invoke(new ExecuteQueryAsyncMethod<TMessageOut>(processor, new QueryContext(processor.Principal, token), query));        

        private ExecuteQueryAsyncMethod(MicroProcessor processor, QueryContext context, IQuery<TMessageOut> query)
        {
            Processor = processor;
            Context = context;            
            Query = query;
        }

        protected override MicroProcessor Processor
        {
            get;
        }

        protected override QueryContext Context
        {
            get;
        }

        private IQuery<TMessageOut> Query
        {
            get;
        }

        protected override async Task<InvokeAsyncResult<TMessageOut>> InvokeQueryCore()
        {           
            Context.StackTraceCore.Push(MicroProcessorOperationTypes.Query, null);

            try
            {
                return await Processor.Pipeline.Build(new QueryDecorator<TMessageOut>(Query)).InvokeAsync(Context);
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
