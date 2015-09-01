using System;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.ComponentModel;
using Kingo.BuildingBlocks.ComponentModel.Server;

namespace Kingo.BuildingBlocks.Security
{
    /// <summary>
    /// This module is used to authorize any further processing of messages based on the current thread's identity.
    /// </summary>    
    public abstract class AuthorizationModule : MessageHandlerModule
    {                        
        /// <summary>
        /// Authorizes the current thread's identity to process a message.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>        
        /// <exception cref="SenderNotAuthorizedException">
        /// The current thread's identity was not authorized to process the specified message.
        /// </exception>
        public override async Task InvokeAsync(IMessageHandler handler)
        {               
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            SenderNotAuthorizedException exception;

            if (await IsNotAuthorizedAsync(handler.Message, out exception))
            {
                throw exception;
            }
            await handler.InvokeAsync();
        }

        /// <summary>
        /// Determines whether or not the current identity is allowed/authorized to
        /// further process the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message being authorized.</param>    
        /// <param name="exception">
        /// If this method returns <c>true</c>, this parameter will refer to an instance of
        /// the <see cref="SenderNotAuthorizedException" /> class that contains a message and
        /// information on why the current identity was not authorized.
        /// </param>   
        /// <returns>
        /// <c>true</c> if the current identity is not authorized to further process the specified
        /// <paramref name="message"/>; otherwise <c>false</c>.
        /// </returns>
        protected abstract Task<bool> IsNotAuthorizedAsync(IMessage message, out SenderNotAuthorizedException exception);
    }
}
