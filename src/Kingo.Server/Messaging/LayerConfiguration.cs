using System;
using System.IO;
using System.Reflection;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a set of logically distinguished <see cref="Layer">Layers</see> of an application.
    /// </summary>
    public sealed class LayerConfiguration
    {
        private readonly Layer _apiLayer;
        private readonly Layer _serviceLayer;
        private readonly Layer _applicationLayer;
        private readonly Layer _domainLayer;
        private readonly Layer _dataAccessLayer;

        private LayerConfiguration(Layer apiLayer, Layer serviceLayer, Layer applicationLayer, Layer domainLayer, Layer dataAccessLayer)
        {
            _apiLayer = apiLayer;
            _serviceLayer = serviceLayer;
            _applicationLayer = applicationLayer;
            _domainLayer = domainLayer;
            _dataAccessLayer = dataAccessLayer;
        }        

        /// <summary>
        /// Returns the layer that contains the API of the application,
        /// inclusing all messages that are received, published and returned through the API.
        /// </summary>
        public Layer ApiLayer
        {
            get { return _apiLayer; }
        }

        /// <summary>
        /// Returns the layer that contains the implementation of the API.
        /// </summary>
        public Layer ServiceLayer
        {
            get { return _serviceLayer; }
        }

        /// <summary>
        /// Returns the layer that contains all message handlers.
        /// </summary>
        public Layer ApplicationLayer
        {
            get { return _applicationLayer; }
        }

        /// <summary>
        /// Returns the layer that contains all domain objects, including any repository interfaces.
        /// </summary>
        public Layer DomainLayer
        {
            get { return _domainLayer; }
        }

        /// <summary>
        /// Returns the layer that contains all data access logic, including all repository implementations.
        /// </summary>
        public Layer DataAccessLayer
        {
            get { return _dataAccessLayer; }
        }

        #region [====== Replacements ======]

        /// <summary>
        /// Replaces the <see cref="ApiLayer" /> with a layer that consists of the specified <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="assemblies">A collection of assemblies.</param>
        /// <returns>A new configuration with the updated layer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public LayerConfiguration ReplaceApiLayer(params Assembly[] assemblies)
        {
            return ReplaceApiLayer(Layer.FromAssemblies(assemblies));
        }

        /// <summary>
        /// Replaces the <see cref="ApiLayer" /> with the specified <paramref name="layer"/>.
        /// </summary>
        /// <param name="layer">The new API layer.</param>
        /// <returns>A new configuration with the updated layer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="layer"/> is <c>null</c>.
        /// </exception>
        public LayerConfiguration ReplaceApiLayer(Layer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException(nameof(layer));
            }
            return new LayerConfiguration(layer, _serviceLayer, _applicationLayer, _domainLayer, _dataAccessLayer);
        }

        /// <summary>
        /// Replaces the <see cref="ServiceLayer" /> with a layer that consists of the specified <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="assemblies">A collection of assemblies.</param>
        /// <returns>A new configuration with the updated layer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public LayerConfiguration ReplaceServiceLayer(params Assembly[] assemblies)
        {
            return ReplaceServiceLayer(Layer.FromAssemblies(assemblies));
        }

        /// <summary>
        /// Replaces the <see cref="ServiceLayer" /> with the specified <paramref name="layer"/>.
        /// </summary>
        /// <param name="layer">The new service layer.</param>
        /// <returns>A new configuration with the updated layer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="layer"/> is <c>null</c>.
        /// </exception>
        public LayerConfiguration ReplaceServiceLayer(Layer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException(nameof(layer));
            }
            return new LayerConfiguration(_apiLayer, layer, _applicationLayer, _domainLayer, _dataAccessLayer);
        }

        /// <summary>
        /// Replaces the <see cref="ApplicationLayer" /> with a layer that consists of the specified <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="assemblies">A collection of assemblies.</param>
        /// <returns>A new configuration with the updated layer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public LayerConfiguration ReplaceApplicationLayer(params Assembly[] assemblies)
        {
            return ReplaceApplicationLayer(Layer.FromAssemblies(assemblies));
        }

        /// <summary>
        /// Replaces the <see cref="ApplicationLayer" /> with the specified <paramref name="layer"/>.
        /// </summary>
        /// <param name="layer">The new application layer.</param>
        /// <returns>A new configuration with the updated layer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="layer"/> is <c>null</c>.
        /// </exception>
        public LayerConfiguration ReplaceApplicationLayer(Layer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException(nameof(layer));
            }
            return new LayerConfiguration(_apiLayer, _serviceLayer, layer, _domainLayer, _dataAccessLayer);
        }

        /// <summary>
        /// Replaces the <see cref="DomainLayer" /> with a layer that consists of the specified <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="assemblies">A collection of assemblies.</param>
        /// <returns>A new configuration with the updated layer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public LayerConfiguration ReplaceDomainLayer(params Assembly[] assemblies)
        {
            return ReplaceDomainLayer(Layer.FromAssemblies(assemblies));
        }

        /// <summary>
        /// Replaces the <see cref="DomainLayer" /> with the specified <paramref name="layer"/>.
        /// </summary>
        /// <param name="layer">The new domain layer.</param>
        /// <returns>A new configuration with the updated layer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="layer"/> is <c>null</c>.
        /// </exception>
        public LayerConfiguration ReplaceDomainLayer(Layer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException(nameof(layer));
            }
            return new LayerConfiguration(_apiLayer, _serviceLayer, _applicationLayer, layer, _dataAccessLayer);
        }

        /// <summary>
        /// Replaces the <see cref="DataAccessLayer" /> with a layer that consists of the specified <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="assemblies">A collection of assemblies.</param>
        /// <returns>A new configuration with the updated layer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public LayerConfiguration ReplaceDataAccessLayer(params Assembly[] assemblies)
        {
            return ReplaceApiLayer(Layer.FromAssemblies(assemblies));
        }

        /// <summary>
        /// Replaces the <see cref="DataAccessLayer" /> with the specified <paramref name="layer"/>.
        /// </summary>
        /// <param name="layer">The new data access layer.</param>
        /// <returns>A new configuration with the updated layer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="layer"/> is <c>null</c>.
        /// </exception>
        public LayerConfiguration ReplaceDataAccessLayer(Layer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException(nameof(layer));
            }
            return new LayerConfiguration(_apiLayer, _serviceLayer, _applicationLayer, _domainLayer, layer);
        }

        #endregion

        #region [====== Default Configuration ======]

        internal static LayerConfiguration CreateDefaultConfiguration(Assembly serviceLayerAssembly)
        {                       
            var apiLayer = CreateApiLayer(serviceLayerAssembly);
            var serviceLayer = CreateServiceLayer(serviceLayerAssembly);
            var applicationLayer = CreateApplicationLayer(serviceLayerAssembly);
            var domainLayer = CreateDomainLayer(serviceLayerAssembly);
            var dataAccessLayer = CreateDataAccessLayer(serviceLayerAssembly);

            return new LayerConfiguration(apiLayer, serviceLayer, applicationLayer, domainLayer, dataAccessLayer);            
        }

        private static Layer CreateApiLayer(Assembly serviceLayerAssembly)
        {
            return CreateLayer(serviceLayerAssembly, "*.Api.dll");
        }

        private static Layer CreateServiceLayer(Assembly serviceLayerAssembly)
        {
            return Layer.FromAssemblies(serviceLayerAssembly);
        }        

        private static Layer CreateApplicationLayer(Assembly serviceLayerAssembly)
        {
            return CreateLayer(serviceLayerAssembly, "*.Application.dll");
        }

        private static Layer CreateDomainLayer(Assembly serviceLayerAssembly)
        {
            return CreateLayer(serviceLayerAssembly, "*.Domain.dll");
        }

        private static Layer CreateDataAccessLayer(Assembly serviceLayerAssembly)
        {
            return CreateLayer(serviceLayerAssembly, "*.DataAccess.dll");
        }

        private static Layer CreateLayer(Assembly serviceLayerAssembly, string searchPattern)
        {
            return Layer.FromDirectory(Path.GetDirectoryName(serviceLayerAssembly.Location), searchPattern);
        }

        #endregion
    }
}
