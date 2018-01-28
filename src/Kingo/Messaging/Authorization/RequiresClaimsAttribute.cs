using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging.Authorization
{
    /// <summary>
    /// Ensures the current principal is a <see cref="ClaimsPrincipal" /> and carries a specific set of claims.
    /// </summary>
    public sealed class RequiresClaimsAttribute : AuthorizationFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresClaimsAttribute" /> class.
        /// </summary>
        /// <param name="claimTypes">A collection of claims the current principal must have in order to process a message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="claimTypes"/> is <c>null</c>.
        /// </exception>
        public RequiresClaimsAttribute(params string[] claimTypes)
        {
            ClaimTypes = claimTypes ?? throw new ArgumentNullException(nameof(claimTypes));
        }

        /// <summary>
        /// A collection of roles the current principal must have in order to process a message.
        /// </summary>
        public IEnumerable<string> ClaimTypes
        {
            get;
        }

        /// <inheritdoc />
        protected override Task<TResult> HandleOrExecuteAsync<TResult>(MessageHandlerOrQuery<TResult> handlerOrQuery, IMicroProcessorContext context)
        {
            //foreach (var requiredClaimType in ClaimTypes)
            //{
            //    if (HasClaim(principal, requiredClaimType))
            //    {
            //        continue;
            //    }
            //    throw NewMissingClaimTypeException(principal.Identity, requiredClaimType, context.Messages.Current.Message);
            //}
            //return base.HandleOrExecuteAsync(handlerOrQuery, context);
            throw new NotImplementedException();
        }            

        private static Exception NewMissingClaimTypeException(IIdentity identity, string requiredClaimType, object failedMessage)
        {
            var messageFormat = ExceptionMessages.RequiresClaimsAttribute_MissingClaim;
            var message = string.Format(messageFormat, identity.Name, requiredClaimType);
            return new UnauthorizedRequestException(failedMessage, message);
        }
    }
}
