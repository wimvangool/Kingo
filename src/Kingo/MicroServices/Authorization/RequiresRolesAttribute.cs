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
        public IEnumerable<string> Roles
        {
            get;
        }        

        /// <inheritdoc />
        protected override Task<InvokeAsyncResult<TResult>> InvokeAsync<TResult>(IMessageHandlerOrQuery<TResult> handlerOrQuery)
        {
            var principal = handlerOrQuery.Context.Principal;
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
                return base.InvokeAsync(handlerOrQuery);
            }
            throw NewIdentityNotAuthenticatedException(principal.Identity);
        }        

        private static Exception NewPrincipalNotInRoleException(IIdentity identity, string requiredRole)
        {
            var messageFormat = ExceptionMessages.RequiresRoleAttribute_MissingRole;
            var message = string.Format(messageFormat, identity.Name, requiredRole);
            return new UnauthorizedRequestException(message);
        }
    }
}
