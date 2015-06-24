using System.Collections.Generic;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a mapping of specific <see cref="IMessageHandler{T}">MessageHandler</see> types to
    /// their <see cref="IMessageHandlerConfiguration">configuration settings</see>.
    /// </summary>
    public sealed class MessageHandlerToConfigurationMapping : Dictionary<Type, IMessageHandlerConfiguration>
    {        
        private readonly IMessageHandlerConfiguration _defaultConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerToConfigurationMapping" /> class.
        /// </summary>
        /// <param name="defaultConfiguration">
        /// The configuration to apply for those types that are not mapped to their own specific configuration.
        /// </param>
        public MessageHandlerToConfigurationMapping(IMessageHandlerConfiguration defaultConfiguration = null)
        {
            _defaultConfiguration = defaultConfiguration ?? MessageHandlerConfiguration.Default;
        }

        /// <summary>
        /// The configuration to apply for those types that are not mapped to their own specific configuration.
        /// </summary>
        public IMessageHandlerConfiguration DefaultConfiguration
        {
            get { return _defaultConfiguration; }
        }
    }
}
