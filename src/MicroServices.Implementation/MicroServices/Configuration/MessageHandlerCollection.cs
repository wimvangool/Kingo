using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// Represents a collection of message handler instances and types.
    /// </summary>
    public sealed class MessageHandlerCollection : MicroProcessorComponentCollection
    {
        private readonly Dictionary<Type, MessageHandlerType> _messageHandlerTypes;
        private readonly List<MessageHandlerComponent> _messageHandlerInstances;
        private readonly IServiceCollection _instanceCollection;

        public MessageHandlerCollection()
        {
            _messageHandlerTypes = new Dictionary<Type, MessageHandlerType>();
            _messageHandlerInstances = new List<MessageHandlerComponent>();
            _instanceCollection = new ServiceCollection();
        }

        /// <inheritdoc />
        public override IEnumerator<MicroProcessorComponent> GetEnumerator() =>
            _messageHandlerTypes.Values.GetEnumerator();

        #region [====== AddSpecificComponentsTo ======]

        /// <inheritdoc />
        protected override IServiceCollection AddSpecificComponentsTo(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            foreach (var service in _instanceCollection)
            {
                services.Add(service);
            }
            return services.AddSingleton(BuildMessageBusEndpointFactory());
        }

        // When building the factory, we filter out all types that have also been registered as an instance.
        // If we don't do this, resolving a specific MessageHandler is undeterministic. As such, we simply
        // decide that types that were added as instance hide any regular type-registrations.
        private IMessageBusEndpointFactory BuildMessageBusEndpointFactory() =>
            new MessageBusEndpointFactory(_messageHandlerInstances.Concat(MessageHandlerTypes));

        private IEnumerable<MessageHandlerComponent> MessageHandlerTypes =>
            this.OfType<MessageHandlerType>().Where(IsNotRegisteredAsInstance);

        private bool IsNotRegisteredAsInstance(MicroProcessorComponent messageHandler) =>
            _messageHandlerInstances.All(instance => instance.Type != messageHandler.Type);

        #endregion

        #region [====== Add ======]

        /// <summary>
        /// Adds the specified <paramref name="messageHandler"/> as a singleton instance for every <see cref="IMessageHandler{TMessage}"/>
        /// implementation it has. If <paramref name="messageHandler"/> does not implement this interface, it is simply ignored.
        /// </summary>
        /// <param name="messageHandler">The handler to register.</param>
        /// <returns><c>true</c> if the instance was added; otherwise <c>false</c>.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public bool AddInstance(object messageHandler)
        {
            if (MessageHandlerInstance.IsMessageHandlerInstance(messageHandler, out var instance))
            {
                _messageHandlerInstances.Add(instance);
                _instanceCollection.AddComponent(instance, messageHandler);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the specified <paramref name="messageHandler" /> as a singleton instance. NB: this message handler will only
        /// receive internal messages (events).
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the specified <paramref name="messageHandler"/>.</typeparam>
        /// <param name="messageHandler">The handler to register.</param>
        /// <returns><c>true</c> if the instance was added; otherwise <c>false</c>.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public bool AddInstance<TMessage>(Action<TMessage, MessageHandlerOperationContext> messageHandler) =>
            AddInstance(new MessageHandlerInstance<TMessage>(messageHandler));

        /// <summary>
        /// Adds the specified <paramref name="messageHandler" /> as a singleton instance.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the specified <paramref name="messageHandler"/>.</typeparam>
        /// <param name="messageHandler">The handler to register.</param>
        /// <returns><c>true</c> if the instance was added; otherwise <c>false</c>.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public bool AddInstance<TMessage>(Func<TMessage, MessageHandlerOperationContext, Task> messageHandler) =>
            AddInstance(new MessageHandlerInstance<TMessage>(messageHandler));

        /// <summary>
        /// Adds the specified <paramref name="messageHandler" /> as a singleton instance.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the specified <paramref name="messageHandler"/>.</typeparam>
        /// <param name="messageHandler">The handler to register.</param>
        /// <returns><c>true</c> if the instance was added; otherwise <c>false</c>.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public bool AddInstance<TMessage>(IMessageHandler<TMessage> messageHandler) =>
            AddInstance(new MessageHandlerInstance<TMessage>(messageHandler));

        private bool AddInstance<TMessage>(MessageHandlerInstance<TMessage> messageHandler)
        {
            _messageHandlerInstances.Add(messageHandler);
            _instanceCollection.AddComponent(messageHandler, messageHandler);
            return true;
        }

        /// <inheritdoc />
        protected override bool Add(MicroProcessorComponent component)
        {
            if (MessageHandlerType.IsMessageHandler(component, out var messageHandler))
            {
                _messageHandlerTypes[messageHandler.Type] = messageHandler;
                return true;
            }
            return false;
        }

        #endregion
    }
}
