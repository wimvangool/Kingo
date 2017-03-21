using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal abstract class ExecuteAsyncMethod<TMessageOut> : MicroProcessorMethod<QueryContext>
    {
        protected static Task<TMessageOut> Invoke(ExecuteAsyncMethod<TMessageOut> method) =>
            method.Invoke();

        private async Task<TMessageOut> Invoke()
        {
            ExecuteAsyncResult<TMessageOut> result;

            using (var scope = MicroProcessorContext.CreateContextScope(Context))
            {
                result = await InvokeCore();
                await scope.CompleteAsync();
            }
            var metadataStream = result.MetadataStream;
            if (metadataStream.Count > 0)
            {
                await HandleMetadataStreamAsyncMethod.Invoke(Processor, Context, metadataStream);
            }
            return result.Message;
        }

        protected abstract Task<ExecuteAsyncResult<TMessageOut>> InvokeCore();        
    }
}
