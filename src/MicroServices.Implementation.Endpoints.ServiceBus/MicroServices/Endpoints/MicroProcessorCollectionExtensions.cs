using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// Contains extensions methods for instances of type <see cref="MicroProcessorComponentCollection" />.
    /// </summary>
    public static class MicroProcessorCollectionExtensions
    {
        #region [====== AddMicroServiceBusController ======]

        /// <summary>
        /// Automatically registers all types that are a <see cref="MicroServiceBusController" />. Each controller
        /// will also be registered as a <see cref="Microsoft.Extensions.Hosting.IHostedService"/> that will be
        /// started and stopped automatically.
        /// </summary>
        /// <param name="collection">A collection of components.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static void AddMicroServiceBusControllers(this MicroProcessorComponentCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            collection.AddComponents(MicroServiceBusControllerType.FromComponent);
        }

        /// <summary>
        /// Adds <typeparamref name="TController"/> as a <see cref="MicroServiceBusController"/>. If
        /// <typeparamref name="TController"/> implements <see cref="Microsoft.Extensions.Hosting.IHostedService"/>,
        /// it is also registered as a hosted service that will be started and stopped automatically.
        /// </summary>
        /// <typeparam name="TController">The type to register as a controller.</typeparam>
        /// <param name="collection">A collection of components.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static void AddMicroServiceBusController<TController>(this MicroProcessorComponentCollection collection) where TController : MicroServiceBusController =>
            collection.AddMicroServiceBusController(typeof(TController));

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a <see cref="MicroServiceBusController"/> if and only if
        /// the specified <paramref name="type"/> actually is a <see cref="MicroServiceBusController"/>. If <paramref name="type"/>
        /// implements <see cref="Microsoft.Extensions.Hosting.IHostedService"/>, it is also registered
        /// as a hosted service that will be started and stopped automatically.
        /// </summary>
        /// <param name="collection">A collection of components.</param>
        /// <param name="type">The type to register as a controller.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static void AddMicroServiceBusController(this MicroProcessorComponentCollection collection, Type type)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            collection.AddComponent(type, MicroServiceBusControllerType.FromComponent);
        }

        #endregion
    }
}
