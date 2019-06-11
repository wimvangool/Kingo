using System;
using System.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represent a component that implements one or more variations of the <see cref="IMessageHandler{TMessage}"/> interface.
    /// </summary>
    public abstract class MessageHandler : MicroProcessorComponent, IMessageHandlerConfiguration
    {
        private readonly MessageHandlerInterface[] _interfaces;
        private readonly Lazy<IMessageHandlerConfiguration> _configuration;        

        internal MessageHandler(MicroProcessorComponent component, params MessageHandlerInterface[] interfaces) :
            base(component)
        {
            _interfaces = interfaces;
            _configuration = new Lazy<IMessageHandlerConfiguration>(GetConfiguration);
        }

        /// <summary>
        /// Returns the <see cref="IMessageHandler{TMessage}"/> interfaces that are implemented by this message handler.
        /// </summary>
        public IReadOnlyCollection<MessageHandlerInterface> Interfaces =>
            _interfaces;

        #region [====== IMessageHandlerConfiguration ======]

        /// <inheritdoc />
        public virtual bool HandlesExternalMessages =>
            _configuration.Value.HandlesExternalMessages;

        /// <inheritdoc />
        public virtual bool HandlesInternalMessages =>
            _configuration.Value.HandlesInternalMessages;

        private IMessageHandlerConfiguration GetConfiguration()
        {
            if (TryGetAttributeOfType(out MessageHandlerAttribute attribute))
            {
                return attribute;
            }
            return new MessageHandlerAttribute();
        }

        internal MicroProcessorOperationKinds GetSupportedOperations()
        {
            var supportedOperationKinds = MicroProcessorOperationKinds.None;

            if (HandlesExternalMessages)
            {
                supportedOperationKinds |= MicroProcessorOperationKinds.RootOperation;
            }
            if (HandlesInternalMessages)
            {
                supportedOperationKinds |= MicroProcessorOperationKinds.BranchOperation;
            }
            return supportedOperationKinds;
        }

        #endregion

        /// <inheritdoc />
        public override string ToString() =>
            $"{Type.FriendlyName()} ({_interfaces.Length} interface(s) implemented)";        
    }
}
