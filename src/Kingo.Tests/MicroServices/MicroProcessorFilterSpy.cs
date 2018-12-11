using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class MicroProcessorFilterSpy : IMicroProcessorFilter
    {
        public MicroProcessorFilterSpy(MicroProcessorFilterStage stage = MicroProcessorFilterStage.ProcessingStage)
        {
            Stage = stage;
        }

        public MicroProcessorFilterStage Stage
        {
            get;
        }

        public bool IsEnabled(MicroProcessorContext context) =>
            true;

        public Task<InvokeAsyncResult<MessageStream>> InvokeMessageHandlerAsync(MessageHandler handler) =>
            handler.Method.InvokeAsync();

        public Task<InvokeAsyncResult<TMessageOut>> InvokeQueryAsync<TMessageOut>(Query<TMessageOut> query) =>
            query.Method.InvokeAsync();
    }
}
