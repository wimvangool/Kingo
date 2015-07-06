using System.Threading.Tasks;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a sequence containing just a single message.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    public class MessageSequenceNode<TMessage> : MessageSequence where TMessage : class, IMessage<TMessage>
    {
        private readonly TMessage _message;                

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSequenceNode{TMessage}" /> class.
        /// </summary>
        /// <param name="message">The message of this sequence.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageSequenceNode(TMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _message = message;           
        }

        /// <summary>
        /// The message of this sequence.
        /// </summary>
        protected TMessage Message
        {
            get { return _message; }
        }        

        /// <inheritdoc />
        public override Task ProcessWithAsync(IMessageProcessor processor)
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            return processor.HandleAsync(_message);
        }
    }
}
