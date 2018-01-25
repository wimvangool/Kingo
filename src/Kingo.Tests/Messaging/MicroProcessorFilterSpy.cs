using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal class MicroProcessorFilterSpy : MicroProcessorFilter
    {                
        protected override Task<TResult> HandleOrExecuteAsync<TResult>(MessageHandlerOrQuery<TResult> handlerOrQuery, IMicroProcessorContext context) =>
             handlerOrQuery.HandleMessageOrExecuteQueryAsync(context);
    }
}
