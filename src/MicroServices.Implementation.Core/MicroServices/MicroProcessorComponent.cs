using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a type that can be registered as a component of a <see cref="MicroProcessor" />.
    /// </summary>
    public class MicroProcessorComponent : IEquatable<MicroProcessorComponent>, ITypeAttributeProvider, IMicroProcessorComponentConfiguration
    {
        private readonly TypeAttributeProvider _attributeProvider;
        private readonly Lazy<IMicroProcessorComponentConfiguration> _configuration;
        private readonly Type[] _serviceTypes;
        
        private MicroProcessorComponent(Type type)
        {
            _attributeProvider = new TypeAttributeProvider(type ?? throw new ArgumentNullException(nameof(type)));
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
            _attributeProvider = component._attributeProvider;
            _configuration = component._configuration;
            _serviceTypes = serviceTypes.ToArray();
        }

        internal MicroProcessorComponent MergeWith(MicroProcessorComponent component) =>
            new MicroProcessorComponent(this, _serviceTypes.Concat(component._serviceTypes));

        internal bool TryRemoveServiceType(Type serviceType, out MicroProcessorComponent component)
        {
            var componentWithoutServiceType = new MicroProcessorComponent(this, _serviceTypes.Where(type => type != serviceType));
            if (componentWithoutServiceType._serviceTypes.Length < _serviceTypes.Length)
            {
                component = componentWithoutServiceType;
                return true;
            }
            component = null;
            return false;
        }

        #region [====== IMicroProcessorComponentConfiguration ======]

        /// <inheritdoc />
        public ServiceLifetime Lifetime =>
            _configuration.Value.Lifetime;

        /// <inheritdoc />
        public IEnumerable<Type> ServiceTypes =>
            _configuration.Value.ServiceTypes.Concat(_serviceTypes).Where(serviceType => serviceType.IsAssignableFrom(Type)).Distinct();        

        private IMicroProcessorComponentConfiguration GetConfiguration()
        {
            if (TryGetAttributeOfType(out MicroProcessorComponentAttribute attribute))
            {
                return attribute;
            }
            return new MicroProcessorComponentAttribute();
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

        #region [====== ITypeAttributeProvider ======]        

        /// <summary>
        /// Type of the component.
        /// </summary>
        public Type Type =>
            _attributeProvider.Type;

        /// <inheritdoc />
        public bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class =>
            _attributeProvider.TryGetAttributeOfType(out attribute);

        /// <inheritdoc />
        public IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() where TAttribute : class =>
            _attributeProvider.GetAttributesOfType<TAttribute>();

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
