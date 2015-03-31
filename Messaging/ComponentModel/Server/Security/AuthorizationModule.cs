namespace System.ComponentModel.Server.Security
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
        /// <exception cref="MessageDeniedException">
        /// The current thread's identity was not authorized to process the specified message.
        /// </exception>
        protected override void InvokeHandler(IMessageHandler handler)
        {                        
            MessageDeniedException exception;

            if (IsNotAuthorized(handler.Message, out exception))
            {
                throw exception;
            }
            handler.Invoke();
        }

        /// <summary>
        /// Determines whether or not the current identity is allowed/authorized to
        /// further process the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message being authorized.</param>    
        /// <param name="exception">
        /// If this method returns <c>true</c>, this parameter will refer to an instance of
        /// the <see cref="MessageDeniedException" /> class that contains a message and
        /// information on why the current identity was not authorized.
        /// </param>   
        /// <returns>
        /// <c>true</c> if the current identity is not authorized to further process the specified
        /// <paramref name="message"/>; otherwise <c>false</c>.
        /// </returns>
        protected abstract bool IsNotAuthorized(IMessage message, out MessageDeniedException exception);
    }
}
