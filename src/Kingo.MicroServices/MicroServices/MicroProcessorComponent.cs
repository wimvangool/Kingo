using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorComponent" /> class.
        /// </summary>
        /// <param name="type">Type of the component.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="type"/> is not a supported component type.
        /// </exception>
        public MicroProcessorComponent(Type type) :
            this(type, CanBeCreatedFrom(type)) { }

        private MicroProcessorComponent(Type type, bool canBeCreatedFromType)
        {
            if (canBeCreatedFromType)
            {
                _attributeProvider = new TypeAttributeProvider(type ?? throw new ArgumentNullException(nameof(type)));
                _configuration = new Lazy<IMicroProcessorComponentConfiguration>(GetConfiguration);
            }
            else
            {
                throw NewUnsupportedTypeException(type);
            }
        }

        internal MicroProcessorComponent(MicroProcessorComponent component)
        {
            _attributeProvider = component._attributeProvider;
            _configuration = component._configuration;
        }        

        private static Exception NewUnsupportedTypeException(Type type)
        {
            var messageFormat = ExceptionMessages.MicroProcessorComponent_UnsupportedType;
            var message = string.Format(messageFormat, type.FriendlyName());
            return new ArgumentException(message, nameof(type));
        }

        #region [====== IMicroProcessorComponentConfiguration ======]

        /// <inheritdoc />
        public ServiceLifetime Lifetime =>
            _configuration.Value.Lifetime;

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

        /// <summary>
        /// Creates and returns a collection of <see cref="MicroProcessorComponent">components</see>
        /// based on a collection of regular <paramref name="types"/>. The returned collection contains
        /// one component for each type that can serve as a component.
        /// </summary>
        /// <param name="types">A collection of types.</param>
        /// <returns>A new collection of components.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<MicroProcessorComponent> FromTypes(IEnumerable<Type> types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }
            foreach (var type in types)
            {
                if (CanBeCreatedFrom(type))
                {
                    yield return new MicroProcessorComponent(type, true);
                }
            }
        }

        /// <summary>
        /// Determines whether or not the specified <paramref name="type"/> can be used as a <see cref="MicroProcessorComponent" />.
        /// </summary>
        /// <param name="type">The type to check.</param>        
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a non-null, non-abstract type and does not represent an open generic type.
        /// Otherwise <c>false</c>.
        /// </returns>        
        public static bool CanBeCreatedFrom(Type type) =>
            type != null && !type.IsAbstract && !type.ContainsGenericParameters;

        #endregion
    }
}
