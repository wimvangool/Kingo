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
        
        private MicroProcessorComponent(Type type)
        {
            _attributeProvider = new TypeAttributeProvider(type ?? throw new ArgumentNullException(nameof(type)));
            _configuration = new Lazy<IMicroProcessorComponentConfiguration>(GetConfiguration);
        }        

        internal MicroProcessorComponent(MicroProcessorComponent component)
        {
            _attributeProvider = component._attributeProvider;
            _configuration = component._configuration;
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

        internal static MicroProcessorComponent FromInstance(object instance) =>
            new MicroProcessorComponent(instance.GetType());
        
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
        
        public static bool IsMicroProcessorComponent(Type type, out MicroProcessorComponent component)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (type.IsAbstract || type.ContainsGenericParameters)
            {
                component = null;
                return false;
            }
            component = new MicroProcessorComponent(type);
            return true;
        }                 

        #endregion
    }
}
