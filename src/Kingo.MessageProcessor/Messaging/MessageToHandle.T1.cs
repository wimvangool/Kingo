using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a sequence containing just a single message.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    public class MessageToHandle<TMessage> : MessageSequence where TMessage : class, IMessage
    {
        private readonly TMessage _message;
        private readonly IMessageHandler<TMessage> _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageToHandle{TMessage}" /> class.
        /// </summary>
        /// <param name="message">The message of this sequence.</param>          
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageToHandle(TMessage message)
            : this(message, null as IMessageHandler<TMessage>) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageToHandle{TMessage}" /> class.
        /// </summary>
        /// <param name="message">The message of this sequence.</param>  
        /// <param name="handler">The handler that will handle the message. Specify <c>null</c> if the handler is implicit.</param>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageToHandle(TMessage message, Action<TMessage> handler)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            _message = message;
            _handler = new MessageHandlerDelegate<TMessage>(handler);  
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageToHandle{TMessage}" /> class.
        /// </summary>
        /// <param name="message">The message of this sequence.</param>  
        /// <param name="handler">The handler that will handle the message. Specify <c>null</c> if the handler is implicit.</param>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageToHandle(TMessage message, Func<TMessage, Task> handler)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            _message = message;
            _handler = new MessageHandlerDelegate<TMessage>(handler);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageToHandle{TMessage}" /> class.
        /// </summary>
        /// <param name="message">The message of this sequence.</param>  
        /// <param name="handler">The handler that will handle the message. Specify <c>null</c> if the handler is implicit.</param>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageToHandle(TMessage message, IMessageHandler<TMessage> handler)            
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            _message = message;
            _handler = handler;            
        }        

        /// <summary>
        /// The message of this sequence.
        /// </summary>
        public TMessage Message
        {
            get { return _message; }
        }        

        /// <inheritdoc />
        public override Task ProcessWithAsync(IMessageProcessor processor, CancellationToken token)
        {
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor));
            }
            return processor.HandleAsync(_message, _handler, token);
        }

        /// <summary>
        /// Implicitly converts a message to a message sequence.
        /// </summary>
        /// <param name="message">Message to convert.</param>
        public static implicit operator MessageToHandle<TMessage>(TMessage message)
        {
            return message == null ? null : new MessageToHandle<TMessage>(message);
        }
    }
}
