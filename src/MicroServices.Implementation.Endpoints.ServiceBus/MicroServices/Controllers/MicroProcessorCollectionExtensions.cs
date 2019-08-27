using System;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Contains extensions methods for instances of type <see cref="MicroProcessorComponentCollection" />.
    /// </summary>
    public static class MicroProcessorCollectionExtensions
    {
        #region [====== AddMicroServiceBusController ======]

        /// <summary>
        /// Adds <typeparamref name="TController"/> as a <see cref="MicroServiceBusController"/>. If
        /// <typeparamref name="TController"/> implements <see cref="Microsoft.Extensions.Hosting.IHostedService"/>,
        /// it is also registered as a hosted service that will be started and stopped automatically.
        /// </summary>
        /// <typeparam name="TController">The type to register as a controller.</typeparam>
        /// <param name="collection">A collection of components.</param>
        /// <param name="isMainController">
        /// Indicates whether or not the specified controller is owned by the current service.
        /// If <c>true</c>, the specified controller <typeparamref name="TController"/> will be registered
        /// as the <see cref="IMicroServiceBus"/> of this service.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static void AddMicroServiceBusController<TController>(this MicroProcessorComponentCollection collection, bool isMainController = false) where TController : MicroServiceBusController =>
            collection.AddMicroServiceBusController(typeof(TController), isMainController);

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a <see cref="MicroServiceBusController"/> if and only if
        /// the specified <paramref name="type"/> actually is a <see cref="MicroServiceBusController"/>. If <paramref name="type"/>
        /// implements <see cref="Microsoft.Extensions.Hosting.IHostedService"/>, it is also registered
        /// as a hosted service that will be started and stopped automatically.
        /// </summary>
        /// <param name="collection">A collection of components.</param>
        /// <param name="type">The type to register as a controller.</param>
        /// <param name="isMainController">
        /// Indicates whether or not the specified controller is owned by the current service.
        /// If <c>true</c>, the specified controller <paramref name="type"/> will be registered as the <see cref="IMicroServiceBus"/>
        /// of this service.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static void AddMicroServiceBusController(this MicroProcessorComponentCollection collection, Type type, bool isMainController = false) =>
            collection.AddComponent(type, component => MicroServiceBusControllerType.FromComponent(component, isMainController));

        #endregion
    }
}
