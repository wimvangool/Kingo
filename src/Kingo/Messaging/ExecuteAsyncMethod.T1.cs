using System;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Kingo.Resources;

namespace Kingo.Messaging
{
    internal abstract class ExecuteAsyncMethod<TMessageOut> : MicroProcessorMethod<QueryContext>
    {
        protected static Task<TMessageOut> Invoke(ExecuteAsyncMethod<TMessageOut> method) =>
            method.Invoke();

        private async Task<TMessageOut> Invoke()
        {
            try
            {
                return await InvokeCore();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (ExternalProcessorException)
            {
                throw;
            }
            catch (ConcurrencyException exception)
            {
                throw NewInternalServerErrorException(exception, ExceptionMessages.ExecuteAsyncMethod_InternalServerError);
            }
            catch (Exception exception)
            {
                throw NewInternalServerErrorException(exception, ExceptionMessages.ExecuteAsyncMethod_InternalServerError);
            }
        }

        private async Task<TMessageOut> InvokeCore()
        {            
            using (MicroProcessorContext.CreateScope(Context))
            {
                return await InvokeQuery();                
            }            
        }             

        private async Task<TMessageOut> InvokeQuery()
        {
            Context.Token.ThrowIfCancellationRequested();

            try
            {
                return (await InvokeQueryCore()).Value;
            }
            catch (InternalProcessorException exception)
            {
                throw NewBadRequestException(exception, ExceptionMessages.ExecuteAsyncMethod_BadRequest);
            }
            finally
            {
                Context.Token.ThrowIfCancellationRequested();
            }
        }

        protected abstract Task<InvokeAsyncResult<TMessageOut>> InvokeQueryCore();

        protected abstract BadRequestException NewBadRequestException(InternalProcessorException exception, string message);

        protected abstract InternalServerErrorException NewInternalServerErrorException(InternalProcessorException exception, string message);

        protected abstract InternalServerErrorException NewInternalServerErrorException(Exception exception, string message);
    }
}
