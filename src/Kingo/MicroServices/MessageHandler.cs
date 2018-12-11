using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a message handler as part of the <see cref="MicroProcessor" />'s pipeline.
    /// </summary>
    public abstract class MessageHandler : MessageHandlerOrQuery, IMessageHandlerOrQuery<MessageStream>
    {
        MicroProcessorContext IMessageHandlerOrQuery<MessageStream>.Context =>
            Context;        

        /// <summary>
        /// Returns the context in which the message handler is invoked.
        /// </summary>
        public abstract MessageHandlerContext Context
        {
            get;
        }

        /// <inheritdoc />
        public abstract MessageHandlerOrQueryMethod<MessageStream> Method
        {
            get;
        }
    }
}
