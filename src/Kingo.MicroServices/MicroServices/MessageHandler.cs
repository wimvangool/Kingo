namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a message handler as part of the <see cref="MicroProcessor" />'s pipeline.
    /// </summary>
    public abstract class MessageHandler : MessageHandlerOrQuery, IMessageHandlerOrQuery<HandleAsyncResult>
    {
        MicroProcessorContext IMessageHandlerOrQuery<HandleAsyncResult>.Context =>
            Context;        

        /// <summary>
        /// Returns the context in which the message handler is invoked.
        /// </summary>
        public abstract MessageHandlerContext Context
        {
            get;
        }

        /// <inheritdoc />
        public abstract MessageHandlerOrQueryMethod<HandleAsyncResult> Method
        {
            get;
        }
    }
}
