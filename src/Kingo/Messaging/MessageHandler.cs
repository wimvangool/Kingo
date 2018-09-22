namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a message handler as part of the <see cref="MicroProcessor" />'s pipeline.
    /// </summary>
    public abstract class MessageHandler : MessageHandlerOrQuery<IMessageStream>
    {
        /// <summary>
        /// Creates and returns a result that can be returned by the message handler pipeline
        /// without invoking the actual message handler.
        /// </summary>
        /// <param name="context">Context of the <see cref="IMicroProcessor" />.</param>        
        /// <returns>A result containing all events that were published.</returns>
        public InvokeAsyncResult<IMessageStream> Yield(MicroProcessorContext context) =>
            new HandleAsyncResult(context.EventBus);
    }
}
