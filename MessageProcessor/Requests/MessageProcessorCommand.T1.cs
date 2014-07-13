using System;
using System.Threading;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a command that is executed on a <see cref="IMessageProcessor" />.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that will be handled by the processor.</typeparam>
    public class MessageProcessorCommand<TMessage> : Command<TMessage> where TMessage : class, IMessage<TMessage>
    {
        private readonly IMessageProcessor _processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessorCommand" /> class.
        /// </summary>
        /// <param name="message">The message that serves as the execution-parameter of this command.</param>
        /// <param name="processor">The processor used to handle this command's message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="processor"/> is <c>null</c>.
        /// </exception>
        public MessageProcessorCommand(TMessage message, IMessageProcessor processor) : base(message)
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            _processor = processor;
        }

        /// <summary>
        /// The processor used to handle this command's <see cref="Message" />.
        /// </summary>
        protected virtual IMessageProcessor Processor
        {
            get { return _processor; }
        }

        protected override void Execute(TMessage message, CancellationToken? token)
        {
            Processor.Handle(message, token);
        }
    }
}
