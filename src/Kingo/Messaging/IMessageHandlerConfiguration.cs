namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, contains all configuration settings for a <see cref="IMessageHandler{T}" />.
    /// </summary>
    public interface IMessageHandlerConfiguration
    {
        /// <summary>
        /// The lifetime of the <see cref="IMessageHandler{T}" />.
        /// </summary>
        InstanceLifetime Lifetime
        {
            get;
        }

        /// <summary>
        /// Specifies during which operation types this handler should be used (input-stream, output-stream or both).
        /// </summary>
        MicroProcessorOperationTypes OperationTypes
        {
            get;
        }
    }
}
