using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging.Authorization
{
    /// <summary>
    /// Ensures the current principal is in a specific set of roles.
    /// </summary>
    public sealed class RequiresRolesAttribute : AuthorizationFilterAttribute
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
        protected override Task<TResult> InvokeMessageHandlerOrQueryAsync<TResult>(MessageHandlerOrQuery<TResult> handlerOrQuery, IMicroProcessorContext context)
        {
            foreach (var requiredRole in Roles)
            {
                if (context.Principal.IsInRole(requiredRole))
                {
                    continue;
                }
                throw NewPrincipalNotInRoleException(context.Principal.Identity, requiredRole, context.Messages.Current.Message);
            }
            return base.InvokeMessageHandlerOrQueryAsync(handlerOrQuery, context);                       
        }                

        private static Exception NewPrincipalNotInRoleException(IIdentity identity, string requiredRole, object failedMessage)
        {
            var messageFormat = ExceptionMessages.RequiresRoleAttribute_MissingRole;
            var message = string.Format(messageFormat, identity.Name, requiredRole);
            return new UnauthorizedRequestException(failedMessage, message);
        }
    }
}
