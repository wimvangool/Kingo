namespace System.ComponentModel
{
    /// <summary>
    /// Represent a <see cref="IMessageValidator{TMessage}" /> for a specific <see cref="IRequestMessage">message</see>.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to validate.</typeparam>
    public class RequestMessageValidator<TMessage> : IMessageValidator<TMessage> where TMessage : class, IRequestMessage
    {
        /// <inheritdoc />
        public virtual bool TryGetValidationErrors(TMessage message, out ValidationErrorTree errorTree)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return message.TryGetValidationErrors(out errorTree);
        }
    }
}
