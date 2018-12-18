using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class MicroProcessorFilterSpyAttribute : MicroProcessorFilterAttribute
    {             
        public MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage stage) :
            base(stage) { }
        
        public int Id
        {
            get;
            set;
        }

        public override async Task<InvokeAsyncResult<MessageStream>> InvokeMessageHandlerAsync(MessageHandler handler)
        {
            handler.Context.EventBus.Publish(Id);
            
            return await handler.Method.InvokeAsync();
        }

        public override async Task<InvokeAsyncResult<TMessageOut>> InvokeQueryAsync<TMessageOut>(Query<TMessageOut> query)
        {
            var serviceBus = MicroServiceBusStub.Current;
            if (serviceBus != null)
            {
                await serviceBus.PublishAsync(Id);
            }            
            return await query.Method.InvokeAsync();
        }
    }
}
