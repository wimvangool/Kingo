using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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
        protected override Task<TResult> InvokeMessageHandlerOrQueryAsync<TResult>(MessageHandlerOrQuery<TResult> handlerOrQuery, IMicroProcessorContext context)
        {
            foreach (var requiredClaimType in ClaimTypes)
            {
                if (context.ClaimsProvider.HasClaim(requiredClaimType))
                {
                    continue;
                }
                throw ClaimsProvider.NewClaimNotFoundException(context.Principal.Identity, requiredClaimType);
            }
            return base.InvokeMessageHandlerOrQueryAsync(handlerOrQuery, context);
        }                    
    }
}
