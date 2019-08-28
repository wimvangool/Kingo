using System;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Contains extensions methods for instances of type <see cref="MicroProcessorComponentCollection" />.
    /// </summary>
    public static class MicroProcessorCollectionExtensions
    {
        #region [====== AddMicroServiceBusController ======]

        /// <summary>
        /// Adds <typeparamref name="TController"/> as a <see cref="IHostedService" />.
        /// If <paramref name="isMainController"/> is <c>true</c>, the controller is also registered as
        /// a <see cref="IMicroServiceBus" />.
        /// </summary>
        /// <typeparam name="TController">The type to register as a controller.</typeparam>
        /// <param name="collection">A collection of components.</param>
        /// <param name="isMainController">
        /// Indicates whether or not the specified controller is owned by the current service.
        /// If <c>true</c>, the specified controller <typeparamref name="TController"/> will be registered
        /// as the <see cref="IMicroServiceBus"/> of this service.
        /// </param>
        /// <returns>
        /// <c>true</c> if <typeparamref name="TController"/> was added as a controller; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static bool AddMicroServiceBusController<TController>(this MicroProcessorComponentCollection collection, bool isMainController = false) where TController : MicroServiceBusController =>
            collection.AddMicroServiceBusController(typeof(TController), isMainController);

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a <see cref="IHostedService" /> if and only if
        /// <paramref name="type"/> is a <see cref="MicroServiceBusController" />.
        /// If <paramref name="isMainController"/> is <c>true</c>, the controller is also registered as
        /// a <see cref="IMicroServiceBus" />.
        /// </summary>
        /// <param name="collection">A collection of components.</param>
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
        /// <paramref name="collection"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool AddMicroServiceBusController(this MicroProcessorComponentCollection collection, Type type, bool isMainController = false)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (collection.AddComponent(type, MicroServiceBusControllerType.FromComponent))
            {
                if (isMainController)
                {
                    collection.AddMicroServiceBus(type);
                }
                return true;
            }
            return false;
        }
            

        #endregion
    }
}
