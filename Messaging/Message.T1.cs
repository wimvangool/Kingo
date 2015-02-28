using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel
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

        /// <summary>
        /// Creates and returns a (materialized) copy of the specified collection.
        /// </summary>
        /// <typeparam name="T">Type of the messages in the collection.</typeparam>
        /// <param name="messages">The collection to copy.</param>
        /// <returns>The copied collection, or <c>null</c> if <paramref name="messages" /> was <c>null</c>.</returns>
        public static IList<T> Copy<T>(IEnumerable<T> messages) where T : class, IMessage<T>
        {
            return messages == null ? null : messages.Select(Copy).ToArray();
        }

        /// <summary>
        /// Creates and returns a copy of the specified <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="T">Type of the message to copy.</typeparam>
        /// <param name="message">The message to copy.</param>
        /// <returns>
        /// A copy of the specified <paramref name="message"/>, or <c>null</c> if <paramref name="message"/> was <c>null</c>.
        /// </returns>
        public static T Copy<T>(T message) where T : class, IMessage<T>
        {
            return message == null ? null : message.Copy();
        }

        #endregion

        #region [====== Validation ======]

        internal override bool TryGetValidationErrors(out ValidationErrorTree errorTree)
        {
            var validator = CreateValidator();
            if (validator != null)
            {
                return validator.TryGetValidationErrors((TMessage)this, out errorTree);
            }
            errorTree = null;
            return false;
        }

        /// <summary>
        /// Creates and returns a <see cref="IMessageValidator{TMessage}" /> that can be used to validate this message,
        /// or <c>null</c> if this message does not require validation.
        /// </summary>
        /// <returns>A new <see cref="IMessageValidator{TMessage}" /> that can be used to validate this message.</returns>
        protected virtual IMessageValidator<TMessage> CreateValidator()
        {
            return null;
        }

        #endregion        
    }
}
