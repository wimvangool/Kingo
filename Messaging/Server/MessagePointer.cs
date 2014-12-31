using System.Threading;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents the stack of messages that is being handled by the processor.
    /// </summary>
    public sealed class MessagePointer
    {
        /// <summary>
        /// The message that is associated to this <see cref="MessagePointer" />.
        /// </summary>
        public readonly IMessage Message;              

        /// <summary>
        /// The parent MessagePointer.
        /// </summary>
        public readonly MessagePointer ParentPointer;

        private readonly CancellationToken? _token;        
        
        internal MessagePointer(IMessage message, CancellationToken? token)
        {
            Message = message;

            _token = token;            
        }

        private MessagePointer(IMessage message, CancellationToken? token, MessagePointer parentPointer)
        {            
            Message = message;
            ParentPointer = parentPointer;

            _token = token;            
        }

        internal MessagePointer CreateChildPointer(IMessage message, CancellationToken? token)
        {
            return new MessagePointer(message, token, this);
        }

        /// <summary>
        /// Indicates whether or not <see cref="Message" /> is of the specified type.
        /// </summary>
        /// <typeparam name="TMessage">Type to check.</typeparam>
        /// <returns><c>true</c> if this message is of the specified type; otherwise <c>false</c>.</returns>
        public bool PointsToA<TMessage>()
        {
            return PointsToA(typeof(TMessage));
        }

        /// <summary>
        /// Indicates whether or not <see cref="Message" /> is of the specified type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns><c>true</c> if this message is of the specified type; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public bool PointsToA(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type.IsInstanceOfType(Message);
        }

        /// <summary>
        /// Attempts to cast the <see cref="Message" /> to which this pointer refers to the specified type.
        /// </summary>
        /// <typeparam name="TMessage">Type to cast to.</typeparam>
        /// <param name="message">
        /// When the cast succeeds, this parameter will point to the casted instance; otherwise <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the cast succeed; otherwise <c>false</c>.</returns>
        public bool TryCastMessageTo<TMessage>(out TMessage message) where TMessage : class
        {
            return (message = Message as TMessage) != null;
        }

        #region [====== Cancellation ======]

        /// <summary>
        /// Traverses up the stack looking for a token that indicates that a cancellation is requested and
        /// if found, throws an <see cref="OperationCanceledException" />.
        /// </summary>
        public void ThrowIfCancellationRequested()
        {
            MessagePointer stack = this;

            do
            {
                stack._token.ThrowIfCancellationRequested();
            } while ((stack = stack.ParentPointer) != null);
        }

        #endregion        
    }
}
