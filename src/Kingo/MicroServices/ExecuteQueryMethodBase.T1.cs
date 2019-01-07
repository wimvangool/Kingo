using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{    
    internal abstract class ExecuteQueryMethodBase<TMessageOut> : MicroProcessorMethod<TMessageOut>
    {                   
        /// <summary>
        /// Invokes the query and returns its result.
        /// </summary>
        /// <returns>The result of the query.</returns>
        public override Task<TMessageOut> InvokeAsync() =>
            InvokeAsync(CreateQueryContext());

        private async Task<TMessageOut> InvokeAsync(QueryContext context)
        {
            using (MicroProcessorContext.CreateScope(context))
            {
                context.Token.ThrowIfCancellationRequested();

                try
                {
                    return (await InvokeAsyncCore(context).ConfigureAwait(false)).GetValue();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (MicroProcessorException)
                {
                    throw;
                }
                catch (MessageHandlerException exception)
                {
                    throw exception.AsBadRequestException(exception.Message);
                }
                catch (Exception exception)
                {
                    throw new InternalServerErrorException(ExceptionMessages.ExecuteAsyncMethod_InternalServerError, exception);
                }
                finally
                {
                    context.Token.ThrowIfCancellationRequested();
                }
            }
        }        

        protected abstract Task<InvokeAsyncResult<TMessageOut>> InvokeAsyncCore(QueryContext context);

        protected abstract QueryContext CreateQueryContext();
    }
}
