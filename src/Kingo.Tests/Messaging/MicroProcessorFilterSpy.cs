using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal class MicroProcessorFilterSpy : ProcessingFilterAttribute
    {                
        protected override Task<TResult> InvokeMessageHandlerOrQueryAsync<TResult>(MessageHandlerOrQuery<TResult> handlerOrQuery, IMicroProcessorContext context) =>
             handlerOrQuery.InvokeAsync(context);
    }
}
