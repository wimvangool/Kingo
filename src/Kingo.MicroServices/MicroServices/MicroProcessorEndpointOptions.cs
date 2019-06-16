using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a set of options that can be set for the endpoints of a <see cref="MicroProcessor" />.
    /// </summary>
    public sealed class MicroProcessorEndpointOptions
    {        
        internal MicroProcessorEndpointOptions()
        {
            MessageKindResolver = new MessageKindResolver();
        }

        private MicroProcessorEndpointOptions(MicroProcessorEndpointOptions options)
        {
            MessageKindResolver = options.MessageKindResolver;
        }

        internal MicroProcessorEndpointOptions Copy() =>
            new MicroProcessorEndpointOptions(this);

        #region [====== MessageKindResolver ======]        

        private IMessageKindResolver _messageKindResolver;

        /// <summary>
        /// Gets or sets the <see cref="IMessageKindResolver" /> used to resolve the message kind
        /// of endpoints of which the message kind was left unspecified.
        /// </summary>
        public IMessageKindResolver MessageKindResolver
        {
            get => _messageKindResolver;
            set => _messageKindResolver = value ?? throw new ArgumentNullException(nameof(value));
        }

        #endregion
    }
}
