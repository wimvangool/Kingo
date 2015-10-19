using System;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// Serves as a simple base-implementation of the <see cref="IMessage{TMessage}" /> interface.
    /// </summary>
    [Serializable]
    public abstract class Message<TMessage> : Message, IMessage<TMessage> where TMessage : Message<TMessage>
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="Message{TMessage}" /> class.
        /// </summary>
        protected Message() { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="message">The message to copy.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected Message(TMessage message)
            : base(message) { }        

        #region [====== Copy ======]

        internal override IMessage CopyMessage()
        {
            return Copy();
        }

        TMessage IMessage<TMessage>.Copy()
        {
            return Copy();
        }

        /// <summary>
        /// Creates and returns a copy of this message.
        /// </summary>
        /// <returns>A copy of this message.</returns>
        public abstract TMessage Copy();        

        #endregion               

        #region [====== Validation ======]

        /// <inheritdoc />
        public override DataErrorInfo Validate()
        {
            var validator = CreateValidator();
            if (validator == null)
            {
                return DataErrorInfo.Empty;
            }
            return validator.Validate(Copy());
        }

        /// <summary>
        /// Creates and returns a <see cref="IMessageValidator{T}" /> that can be used to validate this message,
        /// or <c>null</c> if this message does not require validation.
        /// </summary>
        /// <returns>A new <see cref="IMessageValidator{T}" /> that can be used to validate this message.</returns>
        protected virtual IMessageValidator<TMessage> CreateValidator()
        {
            return null;
        }

        #endregion
    }
}
