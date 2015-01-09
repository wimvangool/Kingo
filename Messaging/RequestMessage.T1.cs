namespace System.ComponentModel
{
    /// <summary>
    /// Represent a message that can validate itself.
    /// </summary>
    [Serializable]
    public abstract class RequestMessage<TMessage> : Message<TMessage>, IRequestMessage
        where TMessage : RequestMessage<TMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage{TMessage}" /> class.
        /// </summary>
        protected RequestMessage() { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="message">The message to copy.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected RequestMessage(TMessage message) : base(message) { }

        #region [====== RequestMessage ======]

        bool IRequestMessage.TryGetValidationErrors(out ValidationErrorTree errorTree)
        {
            return TryGetValidationErrors(out errorTree);
        }

        /// <summary>
        /// Validates all values of this message and returns whether or not any errors were found.
        /// </summary>        
        /// <param name="errorTree">
        /// If this method returns <c>true</c>, this parameter will contain all the validation-errors.
        /// </param>
        /// <returns><c>true</c> if any errors were found during validation; otherwise <c>false</c>.</returns>
        protected abstract bool TryGetValidationErrors(out ValidationErrorTree errorTree);

        #endregion
    }
}
