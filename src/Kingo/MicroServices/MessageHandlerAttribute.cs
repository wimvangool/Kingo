using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This attribute must be put on each <see cref="IMessageHandler{T}" /> class to support auto-registration of it
    /// by the <see cref="MessageHandlerFactory" /> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class MessageHandlerAttribute : Attribute, IMessageHandlerConfiguration
    {
        private MessageHandlerConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerAttribute" /> class.
        /// </summary>
        /// <param name="lifetime">The lifetime of the <see cref="IMessageHandler{T}" />.</param>
        public MessageHandlerAttribute(ServiceLifetime lifetime)
        {
            _configuration = new MessageHandlerConfiguration(lifetime, MicroProcessorOperationTypes.InputStream);
        }

        /// <inheritdoc />
        public ServiceLifetime Lifetime =>
            _configuration.Lifetime;

        /// <summary>
        /// Indicates whether or not the handler will respond to messages from the processor's input-stream
        /// (messages fed to the processor directly).
        /// </summary>        
        public bool HandleInputMessages
        {
            get => HasOperationType(MicroProcessorOperationTypes.InputStream);
            set => SetOperationType(MicroProcessorOperationTypes.InputStream, value);
        }

        /// <summary>
        /// Indicates whether or not the handler will respond to messages from the processor's output-stream
        /// (events published while processing other messages).
        /// </summary>        
        public bool HandleOutputMessages
        {
            get => HasOperationType(MicroProcessorOperationTypes.OutputStream);
            set => SetOperationType(MicroProcessorOperationTypes.OutputStream, value);
        }

        private bool HasOperationType(MicroProcessorOperationTypes operationType) =>
            SupportedOperationTypes.HasFlag(operationType);

        private void SetOperationType(MicroProcessorOperationTypes operationType, bool supportsOperation) =>
            _configuration = supportsOperation ? _configuration.Add(operationType) : _configuration.Remove(operationType);

        /// <inheritdoc />
        public MicroProcessorOperationTypes SupportedOperationTypes =>
            _configuration.SupportedOperationTypes;

        /// <inheritdoc />
        public override string ToString() =>
            _configuration.ToString();
    }
}
