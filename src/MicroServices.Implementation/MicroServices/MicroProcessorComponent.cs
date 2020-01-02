using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.MicroServices.Configuration;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a type that can be registered as a component of a <see cref="MicroProcessor" />.
    /// </summary>
    public class MicroProcessorComponent : IEquatable<MicroProcessorComponent>, IMicroProcessorComponentConfiguration
    {
        private readonly Lazy<IMicroProcessorComponentConfiguration> _configuration;
        private readonly Type _type;
        private readonly Type[] _serviceTypes;
        
        private MicroProcessorComponent(Type type)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _configuration = new Lazy<IMicroProcessorComponentConfiguration>(GetConfiguration);
            _serviceTypes = new Type[0];
        }        

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorComponent" /> class.
        /// </summary>
        /// <param name="component">Component to copy.</param>
        /// <param name="serviceTypes">A collection of service-types.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="component"/> or <paramref name="serviceTypes"/> is <c>null</c>.
        /// </exception>
        protected MicroProcessorComponent(MicroProcessorComponent component, IEnumerable<Type> serviceTypes)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }
            _type = component._type;
            _configuration = component._configuration;
            _serviceTypes = serviceTypes.ToArray();
        }

        /// <summary>
        /// Type of the component.
        /// </summary>
        public Type Type =>
            _type;

        internal MicroProcessorComponent MergeWith(MicroProcessorComponent component) =>
            Copy(_serviceTypes.Concat(component._serviceTypes));

        /// <summary>
        /// Copies the current component while assigning the <paramref name="serviceTypes"/> to it.
        /// </summary>
        /// <param name="serviceTypes">A collection of service types to assign to the copied component.</param>
        /// <returns>The copied component.</returns>
        protected virtual MicroProcessorComponent Copy(IEnumerable<Type> serviceTypes) =>
            new MicroProcessorComponent(this, serviceTypes);

        #region [====== IMicroProcessorComponentConfiguration ======]

        /// <inheritdoc />
        public ServiceLifetime Lifetime =>
            _configuration.Value.Lifetime;

        /// <inheritdoc />
        public IEnumerable<Type> ServiceTypes =>
            _configuration.Value.ServiceTypes.Concat(_serviceTypes).Where(serviceType => serviceType.IsAssignableFrom(Type)).Distinct();        

        private IMicroProcessorComponentConfiguration GetConfiguration()
        {
            if (Type.TryGetAttributeOfType(out MicroProcessorComponentAttribute attribute))
            {
                return attribute;
            }
            return new MicroProcessorComponentAttribute();
        }

        #endregion

        #region [====== AddTo ======]

        /// <summary>
        /// Adds this component to the specified <paramref name="services"/> with a mapping from all <see cref="ServiceTypes"/>.
        /// </summary>
        /// <param name="services">A collection of services this component will be added to.</param>
        /// <param name="instance">
        /// If specified, a direct mapping to this instance is made; otherwise a type
        /// mapping with the appropriate <see cref="ServiceLifetime"/> is made.
        /// </param>
        /// <returns>The updated service collection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="services"/> is <c>null</c>.
        /// </exception>
        public IServiceCollection AddTo(IServiceCollection services, object instance = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            // First, we add mappings from the serviceTypes - which are presumably interfaces or abstract classes -
            // to the component type by factory, so that at run-time they will all resolve the same instance if requested.
            // This will ensure that no matter through which (service) type the component is resolved, it will always
            // be resolved correctly (that is, with the appropriate lifetime).
            foreach (var serviceType in ServiceTypes)
            {
                services = services.AddTransient(serviceType, provider => provider.GetRequiredService(Type));
            }

            // Secondly, we add the component by its own type.
            // If no instance is provided, the component is registered with the specified lifetime.
            if (instance == null)
            {
                switch (Lifetime)
                {
                    case ServiceLifetime.Transient:
                        return AddTransientTypeMappingTo(services);
                    case ServiceLifetime.Scoped:
                        return AddScopedTypeMappingTo(services);
                    case ServiceLifetime.Singleton:
                        return AddSingletonTypeMappingTo(services);
                    default:
                        throw NewLifetimeNotSupportedException();
                }
            }
            // If an instance is provided, the lifetime is always implicitly set to singleton (because by definition, you always
            // resolve the same instance). However, the mapping itself uses Transient because that will trigger the provided factory
            // for each invocation.
            return services.AddTransient(Type, provider => instance);
        }

        /// <summary>
        /// Adds a type-mapping to the specified <paramref name="services"/> for this component based on a transient lifetime.
        /// </summary>
        /// <param name="services">A collection of services this component will be added to.</param>
        /// <returns>The updated service collection.</returns>
        /// <exception cref="InvalidOperationException">
        /// This component does not support the <see cref="ServiceLifetime.Transient"/> lifetime.
        /// </exception>
        protected virtual IServiceCollection AddTransientTypeMappingTo(IServiceCollection services) =>
            services.AddTransient(Type);

        /// <summary>
        /// Adds a type-mapping to the specified <paramref name="services"/> for this component based on a scoped lifetime.
        /// </summary>
        /// <param name="services">A collection of services this component will be added to.</param>
        /// <returns>The updated service collection.</returns>
        /// <exception cref="InvalidOperationException">
        /// This component does not support the <see cref="ServiceLifetime.Scoped"/> lifetime.
        /// </exception>
        protected virtual IServiceCollection AddScopedTypeMappingTo(IServiceCollection services) =>
            services.AddScoped(Type);

        /// <summary>
        /// Adds a type-mapping to the specified <paramref name="services"/> for this component based on a singleton lifetime.
        /// </summary>
        /// <param name="services">A collection of services this component will be added to.</param>
        /// <returns>The updated service collection.</returns>
        /// <exception cref="InvalidOperationException">
        /// This component does not support the <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </exception>
        protected virtual IServiceCollection AddSingletonTypeMappingTo(IServiceCollection services) =>
            services.AddSingleton(Type);

        /// <summary>
        /// Creates and returns an exception that indicates that this component
        /// could not be added to a <see cref="IServiceCollection"/> because the <see cref="Lifetime"/> configured for
        /// this component is not supported for this type.
        /// </summary>
        /// <returns>A new exception to throw.</returns>
        protected Exception NewLifetimeNotSupportedException()
        {
            var messageFormat = ExceptionMessages.MicroProcessorComponent_InvalidComponentLifetime;
            var message = string.Format(messageFormat, Type.FriendlyName(), Lifetime);
            return new InvalidOperationException(message);
        }

        #endregion

        #region [====== Equals, GetHashCode & ToString ======]

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            Equals(obj as MicroProcessorComponent);

        /// <inheritdoc />
        public virtual bool Equals(MicroProcessorComponent other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return Type == other.Type;
        }

        /// <inheritdoc />
        public override int GetHashCode() =>
            Type.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Type.FriendlyName();        

        #endregion

        #region [====== FromTypes ======]

        internal static MicroProcessorComponent FromInstance(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (IsMicroProcessorComponent(instance.GetType(), out var component))
            {
                return component;
            }
            throw NewTypeNotSupportedException(instance);
        }        

        internal static IEnumerable<MicroProcessorComponent> FromTypes(IEnumerable<Type> types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }
            foreach (var type in types)
            {
                if (IsMicroProcessorComponent(type, out var component))
                {
                    yield return component;
                }
            }
        }
        
        internal static bool IsMicroProcessorComponent(Type type, out MicroProcessorComponent component)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (type.IsValueType || type.IsAbstract || type.ContainsGenericParameters)
            {
                component = null;
                return false;
            }
            component = new MicroProcessorComponent(type);
            return true;
        }

        private static Exception NewTypeNotSupportedException(object instance)
        {
            var messageFormat = ExceptionMessages.MicroProcessorComponent_TypeNotSupported;
            var message = string.Format(messageFormat, instance.GetType().FriendlyName());
            return new ArgumentException(message, nameof(instance));
        }

        #endregion
    }
}
