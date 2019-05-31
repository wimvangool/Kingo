using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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

        public override async Task<HandleAsyncResult> InvokeMessageHandlerAsync(MessageHandler handler)
        {
            AddIdentifier(handler.Context);
            
            handler.Context.EventBus.Publish(new object());

            return await handler.Method.InvokeAsync();
        }

        public override async Task<ExecuteAsyncResult<TResponse>> InvokeQueryAsync<TResponse>(Query<TResponse> query)
        {
            AddIdentifier(query.Context);

            return await query.Method.InvokeAsync();
        }

        private void AddIdentifier(MicroProcessorContext context) =>
            context.ServiceProvider.GetService<ICollection<int>>()?.Add(Id);
    }
}
