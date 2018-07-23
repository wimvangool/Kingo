using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class ExecuteQueryMethodImplementation<TMessageOut> : ExecuteQueryMethod<TMessageOut>
    {
        private readonly IQuery<TMessageOut> _query;

        internal ExecuteQueryMethodImplementation(MicroProcessor processor, CancellationToken? token, IQuery<TMessageOut> query) :
            base(processor, token)
        {                 
            _query = query;
        }        

        internal override async Task<InvokeAsyncResult<TMessageOut>> InvokeQueryAsync(MicroProcessor processor, QueryContext context)
        {           
            context.StackTraceCore.Push(MicroProcessorOperationTypes.Query, null);

            try
            {
                return await processor.Pipeline.Build(new QueryDecorator<TMessageOut>(_query)).InvokeAsync(context);
            }
            finally
            {
                context.StackTraceCore.Pop();                
            }
        }                    
    }
}
