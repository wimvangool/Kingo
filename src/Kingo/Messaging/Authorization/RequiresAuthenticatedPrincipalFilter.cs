using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging.Authorization
{
    internal sealed class RequiresAuthenticatedPrincipalFilter : MicroProcessorFilterDecorator
    {
        public RequiresAuthenticatedPrincipalFilter(IMicroProcessorFilter nextFilter) :
            base(nextFilter) { }

        protected override Task<TResult> InvokeMessageHandlerOrQueryAsync<TResult>(MessagePipeline<TResult> pipeline, IMicroProcessorContext context)
        {
            if (context.Principal.Identity.IsAuthenticated)
            {
                return pipeline.InvokeNextFilterAsync(context);
            }
            throw NewPrincipalNotAuthenticatedException(context.Principal.Identity, context.Messages.Current.Message);            
        }

        private static Exception NewPrincipalNotAuthenticatedException(IIdentity identity, object failedMessage)
        {
            var messageFormat = ExceptionMessages.RequiresAuthenticatedPrincipalFilter_PrincipalNotAuthenticated;
            var message = string.Format(messageFormat, identity.Name);
            return new UnauthorizedRequestException(failedMessage, message);
        }
    }
}
