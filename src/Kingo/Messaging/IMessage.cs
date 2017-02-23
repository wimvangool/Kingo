namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a message that can validate and copy itself.
    /// </summary>
    public interface IMessage : IMessageStream
    {
        /// <summary>
        /// Validates this message and returns an <see cref="ErrorInfo"/> instance
        /// that contains error messages for the instance and all invalid members.
        /// </summary>                
        /// <returns>
        /// A <see cref="ErrorInfo" /> instance that contains all validation-errors (if any).
        /// </returns>   
        ErrorInfo Validate();
    }
}
