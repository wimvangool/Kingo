using System;
using System.Security.Principal;

namespace Kingo.MicroServices.Authorization
{
    /// <summary>
    /// Serves as a base class for all filter-attributes that are executed during the <see cref="MicroProcessorFilterStage.AuthorizationStage" />
    /// of the microprocessor pipeline and are designed to authorize the current principle.
    /// </summary>
    public abstract class AuthorizationStageAttribute : MicroProcessorFilterAttribute
    {                
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationStageAttribute" /> class.
        /// </summary>
        protected AuthorizationStageAttribute() :
            base(MicroProcessorFilterStage.AuthorizationStage) { }

        /// <summary>
        /// Creates and returns an exception that signals that the specified <paramref name="identity"/> is not authenticated.
        /// </summary>
        /// <param name="identity">The identity that hasn't been authenticated.</param>
        /// <returns>A new exception to throw.</returns>
        protected static Exception NewIdentityNotAuthenticatedException(IIdentity identity)
        {
            var messageFormat = ExceptionMessages.AuthorizationStageAttribute_IdentityNotAuthenticated;
            var message = string.Format(messageFormat, identity.Name);
            return new UnauthorizedRequestException(message);
        }
    }
}
