using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Authorization
{
    /// <summary>
    /// Ensures the current principal is in a specific set of roles.
    /// </summary>
    public sealed class RequiresRolesAttribute : AuthorizationStageAttribute
    {      
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresRolesAttribute" /> class.
        /// </summary>
        /// <param name="roles">A collection of roles the current principal must have in order to process a message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="roles"/> is <c>null</c>.
        /// </exception>
        public RequiresRolesAttribute(params string[] roles)
        {
            Roles = roles ?? throw new ArgumentNullException(nameof(roles));
        }

        /// <summary>
        /// A collection of roles the current principal must have in order to process a message.
        /// </summary>
        public IReadOnlyList<string> Roles
        {
            get;
        }        

        /// <inheritdoc />
        protected override async Task<InvokeAsyncResult<TResult>> InvokeAsync<TResult>(IMessageHandlerOrQuery<TResult> handlerOrQuery)
        {
            if (Roles.Count == 0)
            {
                return await base.InvokeAsync(handlerOrQuery);
            }
            var principal = handlerOrQuery.Context.Principal;
            if (principal == null)
            {
                throw NewPrincipalNotSetException(handlerOrQuery);
            }            
            if (principal.Identity.IsAuthenticated)
            {
                foreach (var requiredRole in Roles)
                {
                    if (principal.IsInRole(requiredRole))
                    {
                        continue;
                    }
                    throw NewPrincipalNotInRoleException(principal.Identity, requiredRole);
                }
                return await base.InvokeAsync(handlerOrQuery);
            }
            throw NewIdentityNotAuthenticatedException(handlerOrQuery, principal.Identity);
        }        

        private static Exception NewPrincipalNotInRoleException(IIdentity identity, string requiredRole)
        {
            var messageFormat = ExceptionMessages.RequiresRoleAttribute_MissingRole;
            var message = string.Format(messageFormat, identity.Name, requiredRole);
            return new UnauthorizedRequestException(message);
        }
    }
}
