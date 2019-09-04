namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, contains all configuration settings for a <see cref="IMessageHandler{T}" />.
    /// </summary>
    public interface IMessageHandlerConfiguration
    {        
        /// <summary>
        /// Indicates whether or not the message handler handles commands and events that are
        /// provided to the <see cref="MicroProcessor" /> from external sources.
        /// </summary>
        bool HandlesExternalMessages
        {
            get;
        }

        /// <summary>
        /// Indicates whether or not the message handler handles events in the same (logical)
        /// transaction as they were published in (i.e. as part of the same logical operation).
        /// </summary>
        bool HandlesInternalMessages
        {
            get;
        }
    }
}
