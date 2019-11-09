using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a collection of <see cref="MicroServiceBusController"/> types.
    /// </summary>
    public sealed class MicroServiceBusControllerCollection : MicroProcessorComponentCollection
    {
        private readonly List<StoreAndForwardQueueType> _storeAndForwardQueueTypes;
        private readonly List<Type> _serviceBusTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusControllerCollection" /> class.
        /// </summary>
        public MicroServiceBusControllerCollection()
        {
            _storeAndForwardQueueTypes = new List<StoreAndForwardQueueType>();
            _serviceBusTypes = new List<Type>();
        }

        #region [====== AddStoreAndForwardQueue ======]

        /// <summary>
        /// Adds <typeparamref name="TQueue"/> as a <see cref="StoreAndForwardQueue"/> that will be placed on top
        /// of the <see cref="IMicroServiceBus"/> pipeline.
        /// </summary>
        /// <typeparam name="TQueue">The type to register as a controller.</typeparam>
        /// <returns>
        /// <c>true</c> if <typeparamref name="TQueue"/> was added as a controller; otherwise <c>false</c>.
        /// </returns>
        public bool AddStoreAndForwardQueue<TQueue>() where TQueue : StoreAndForwardQueue =>
            AddStoreAndForwardQueue(typeof(TQueue));

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a <see cref="StoreAndForwardQueue"/> that will be placed on top
        /// of the <see cref="IMicroServiceBus"/> pipeline, if and only if <paramref name="type"/> is a <see cref="StoreAndForwardQueue"/>.
        /// </summary>
        /// <param name="type">The type to register as a queue.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> was added as a queue; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public bool AddStoreAndForwardQueue(Type type)
        {
            if (StoreAndForwardQueueType.IsStoreAndForwardQueue(type, out var queueType))
            {
                _storeAndForwardQueueTypes.Add(queueType);
                return true;
            }
            return false;
        }

        #endregion

        #region [====== Add ======]

        /// <summary>
        /// Adds <typeparamref name="TController"/> as a <see cref="IHostedService" />.
        /// </summary>
        /// <typeparam name="TController">The type to register as a controller.</typeparam>
        /// <returns>
        /// <c>true</c> if <typeparamref name="TController"/> was added as a controller; otherwise <c>false</c>.
        /// </returns>
        public new bool Add<TController>() where TController : MicroServiceBusController =>
            base.Add<TController>();

        /// <summary>
        /// Adds <typeparamref name="TController"/> as a <see cref="IHostedService" />.
        /// If <paramref name="isMainController"/> is <c>true</c>, the controller is also registered as
        /// a <see cref="IMicroServiceBus" />.
        /// </summary>
        /// <typeparam name="TController">The type to register as a controller.</typeparam>
        /// <param name="isMainController">
        /// Indicates whether or not the specified controller is owned by the current service.
        /// If <c>true</c>, the specified controller <typeparamref name="TController"/> will be registered
        /// as the <see cref="IMicroServiceBus"/> of this service.
        /// </param>
        /// <returns>
        /// <c>true</c> if <typeparamref name="TController"/> was added as a controller; otherwise <c>false</c>.
        /// </returns>
        public bool Add<TController>(bool isMainController) where TController : MicroServiceBusController =>
            Add(typeof(TController), isMainController);

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a <see cref="IHostedService" /> if and only if
        /// <paramref name="type"/> is a <see cref="MicroServiceBusController" />.
        /// If <paramref name="isMainController"/> is <c>true</c>, the controller is also registered as
        /// a <see cref="IMicroServiceBus" />.
        /// </summary>
        /// <param name="type">The type to register as a controller.</param>
        /// <param name="isMainController">
        /// Indicates whether or not the specified controller is owned by the current service.
        /// If <c>true</c>, the specified controller <paramref name="type"/> will be registered as the <see cref="IMicroServiceBus"/>
        /// of this service.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> was added as a controller; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public bool Add(Type type, bool isMainController)
        {
            if (Add(type))
            {
                if (isMainController)
                {
                    AddServiceBusType(type);
                }
                return true;
            }
            return false;
        }

        private void AddServiceBusType(Type type)
        {
            _serviceBusTypes.Remove(type);
            _serviceBusTypes.Add(type);
        }

        /// <inheritdoc />
        protected override bool Add(MicroProcessorComponent component)
        {
            if (MicroServiceBusControllerType.IsController(component, out var controller))
            {
                return base.Add(controller);
            }
            return false;
        }

        #endregion

        #region [====== AddSpecific ======]

        /// <inheritdoc />
        protected internal override IServiceCollection AddSpecificComponentsTo(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            // We'll place the added StoreAndForwardQueue-types on top of the MicroServiceBus-implementation.
            // We do this in reverse order compared to the order in which they were added to the collection, so that
            // the following configuration...
            // 
            // Components.AddStoreAndForwardQueue<QueueOne>();
            // Components.AddStoreAndForwardQueue<QueueTwo>();
            // 
            // ...will translate the following pipeline:
            //
            // QueueOne --> QueueTwo --> MicroServiceBus.
            //
            // Note that finally, the IMicroServiceBus-mapping will be configured such that it will resolve to the
            // first item of the pipeline, which will be the first queue or just the MicroServiceBus-type if no queues
            // were added.
            IMicroServiceBusResolver microServiceBusResolver = new MicroServiceBusResolver(_serviceBusTypes);
            var components = new List<MicroProcessorComponent>();

            foreach (var queueType in _storeAndForwardQueueTypes.AsEnumerable().Reverse())
            {
                components.Add(queueType.AssignQueueTypeFactory(microServiceBusResolver = new StoreAndForwardQueueResolver(microServiceBusResolver, queueType.Type)));
            }
            return services
                .AddComponents(components)
                .AddSingleton(microServiceBusResolver.ResolveMicroServiceBus);
        }

        #endregion
    }
}
