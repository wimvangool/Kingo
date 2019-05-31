using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class MicroProcessorFilterStub : MicroProcessorFilterAttribute
    {
        public MicroProcessorFilterStub(MicroProcessorFilterStage stage)
            : base(stage) { }

        public override async Task<HandleAsyncResult> InvokeMessageHandlerAsync(MessageHandler handler)
        {
            handler.Context.EventBus.Publish(Stage);

            return await base.InvokeMessageHandlerAsync(handler);
        }            
    }
}
