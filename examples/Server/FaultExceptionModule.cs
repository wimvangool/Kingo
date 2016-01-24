using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Security;

namespace Kingo.Samples.Chess
{
    internal sealed class FaultExceptionModule : MessageHandlerModule
    {
        public override async Task InvokeAsync(IMessageHandlerWrapper handler)
        {
            try
            {
                await handler.InvokeAsync();
            }
            catch (AuthorizationException exception)
            {                
                throw new FaultException(exception.Message);
            }
            catch (InvalidMessageException exception)
            {
                throw new FaultException(GetErrorMessage(exception));
            }
            catch (CommandExecutionException exception)
            {
                throw new FaultException(exception.Message);
            }
            catch
            {
                var subCode = new FaultCode(handler.Message.GetType().Name);
                var code = FaultCode.CreateReceiverFaultCode(subCode);
                throw new FaultException("Internal server error.", code);
            }
        }

        private static string GetErrorMessage(InvalidMessageException exception)
        {
            var errorInfo = exception.ErrorInfo;
            if (errorInfo.MemberErrors.Count == 0)
            {
                return errorInfo.Error;
            }
            return errorInfo.MemberErrors.Values.First();
        }
    }
}
