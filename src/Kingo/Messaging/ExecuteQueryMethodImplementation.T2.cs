using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class ExecuteQueryMethodImplementation<TMessageIn, TMessageOut> : ExecuteQueryMethod<TMessageOut>
    {
        private readonly IQuery<TMessageIn, TMessageOut> _query;
        private readonly TMessageIn _message;

        internal ExecuteQueryMethodImplementation(MicroProcessor processor, CancellationToken? token, IQuery<TMessageIn, TMessageOut> query, TMessageIn message) :
            base(processor, token)
        {                     
            _query = query;
            _message = message;
        }        

        internal override async Task<InvokeAsyncResult<TMessageOut>> InvokeQueryAsync(MicroProcessor processor, QueryContext context)
        {
            context.StackTraceCore.Push(MicroProcessorOperationTypes.Query, _message);

            try
            {
                return await processor.Pipeline.Build(new QueryDecorator<TMessageIn, TMessageOut>(_query, _message)).InvokeAsync(context);
            }
            finally
            {
                context.StackTraceCore.Pop();
            }
        }                    
    }
}
