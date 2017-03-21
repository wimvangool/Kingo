using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal class MicroProcessorPipelineSpy : MicroProcessorPipeline
    {                
        protected override Task<TResult> HandleOrExecuteAsync<TResult>(MessageHandlerOrQuery<TResult> handlerOrQuery, IMicroProcessorContext context) =>
             handlerOrQuery.HandleMessageOrExecuteQueryAsync(context);
    }
}
