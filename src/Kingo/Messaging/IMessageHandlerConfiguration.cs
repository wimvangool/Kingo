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
        MessageHandlerLifetime Lifetime
        {
            get;
        }

        /// <summary>
        /// Specifies which source(s) the message is accepted from.
        /// </summary>
        MessageSources Sources
        {
            get;
        }
    }
}
