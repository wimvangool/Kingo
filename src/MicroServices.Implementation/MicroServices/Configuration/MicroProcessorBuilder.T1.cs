using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;
using static Kingo.Ensure;

namespace Kingo.MicroServices.Configuration
{
    internal sealed class MicroProcessorBuilder<TProcessor> : IMicroProcessorBuilder where TProcessor : MicroProcessor
    {
        private readonly MicroProcessorSettings _settings;
        private readonly Dictionary<Type, IMicroProcessorComponentCollection> _componentCollections;

        public MicroProcessorBuilder()
        {
            _settings = MicroProcessorSettings.DefaultSettings();
            _componentCollections = new Dictionary<Type, IMicroProcessorComponentCollection>();
        }

        #region [====== IMicroProcessorBuilder ======]

        public IMicroProcessorBuilder ConfigureSettings(Action<MicroProcessorSettings> configurator) =>
            Invoke(configurator, _settings);

        public IMicroProcessorBuilder ConfigureComponents<TCollection>(Action<TCollection> configurator) where TCollection : IMicroProcessorComponentCollection, new() =>
            Invoke(configurator, GetOrAddCollection<TCollection>());

        private IMicroProcessorBuilder Invoke<TArgument>(Action<TArgument> configurator, TArgument argument)
        {
            IsNotNull(configurator, nameof(configurator)).Invoke(argument);
            return this;
        }

        private TCollection GetOrAddCollection<TCollection>() where TCollection : IMicroProcessorComponentCollection, new()
        {
            if (_componentCollections.TryGetValue(typeof(TCollection), out var componentCollection))
            {
                return (TCollection) componentCollection;
            }
            var newCollection = new TCollection();
            _componentCollections.Add(typeof(TCollection), newCollection);
            return newCollection;
        }

        #endregion

        #region [====== BuildServiceCollection ======]

        public IServiceCollection BuildServiceCollection(IServiceCollection services = null)
        {
            if (services == null)
            {
                return BuildServiceCollection(new ServiceCollection());
            }
            if (MicroProcessorType.IsMicroProcessorType(typeof(TProcessor), out var processor))
            {
                return AddComponentsTo(services, _componentCollections.Values.ToArray())
                    .AddSingleton(_settings.Copy())
                    .AddComponent(processor);
            }
            throw NewInvalidProcessorTypeException(typeof(TProcessor));
        }

        private static IServiceCollection AddComponentsTo(IServiceCollection services, IMicroProcessorComponentCollection[] componentCollections)
        {
            // First, all basic components added to the collections are registered. Before registration,
            // though, all components are merged, so that each type is only registered once.
            foreach (var component in MergeComponents(componentCollections))
            {
                services = services.AddComponent(component);
            }

            // Second, we let each collection add its own special types, since some collections need to build
            // and register their own components on the fly.
            foreach (var collection in componentCollections)
            {
                services = collection.AddSpecificComponentsTo(services);
            }
            return services;
        }

        private static IEnumerable<MicroProcessorComponent> MergeComponents(IEnumerable<IMicroProcessorComponentCollection> componentsToMerge)
        {
            var mergedComponents = new Dictionary<Type, MicroProcessorComponent>();

            foreach (var componentToAdd in componentsToMerge.SelectMany(components => components))
            {
                if (mergedComponents.TryGetValue(componentToAdd.Type, out var component))
                {
                    mergedComponents[componentToAdd.Type] = component.MergeWith(componentToAdd);
                }
                else
                {
                    mergedComponents.Add(componentToAdd.Type, componentToAdd);
                }
            }
            return mergedComponents.Values;
        }

        private static Exception NewInvalidProcessorTypeException(Type processorType)
        {
            var messageFormat = ExceptionMessages.MicroProcessorBuilder_InvalidProcessorType;
            var message = string.Format(messageFormat, processorType.FriendlyName());
            return new InvalidOperationException(message);
        }

        #endregion
    }
}
