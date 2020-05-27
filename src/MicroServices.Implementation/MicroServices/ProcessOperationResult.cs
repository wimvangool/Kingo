namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the result of an operation that was provided with a <see cref="IMessage"/> to process.
    /// </summary>
    public enum ProcessOperationResult
    {
        /// <summary>
        /// Indicates the message was accepted and processed successfully.
        /// </summary>
        Accepted,

        /// <summary>
        /// Indicates the message was ignored because its <see cref="IMessage.Kind"/> was not supported.
        /// </summary>
        MessageKindNotSupported,

        /// <summary>
        /// Indicates the message was ignored because its <see cref="IMessage.Direction"/> was not supported.
        /// </summary>
        MessageDirectionNotSupported,

        /// <summary>
        /// Indicates the message was ignored because its <see cref="IMessage.Content"/> was not supported.
        /// </summary>
        MessageContentNotSupported
    }
}
