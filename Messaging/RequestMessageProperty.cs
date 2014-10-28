using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.ComponentModel.Messaging
{
    internal sealed class RequestMessageProperty
    {
        private readonly PropertyInfo _property;
        private readonly PropertyChangedOption _option;

        private RequestMessageProperty(PropertyInfo property, PropertyChangedOption option)
        {
            _property = property;
            _option = option;
        }

        public PropertyChangedOption Option
        {
            get { return _option; }
        }

        public string Name
        {
            get { return _property.Name; }
        }

        public object GetValue(object instance)
        {
            return _property.GetValue(instance, null);
        }

        #region [====== Reflection Methods ======]

        private static readonly ConcurrentDictionary<Type, RequestMessageProperty[]> _PropertiesPerType = new ConcurrentDictionary<Type, RequestMessageProperty[]>();

        public static IEnumerable<RequestMessageProperty> GetProperties(Type messageType)
        {
            if (messageType == null)
            {
                throw new ArgumentNullException("messageType");
            }
            return _PropertiesPerType.GetOrAdd(messageType, GetPropertiesOfType);
        }

        private static RequestMessageProperty[] GetPropertiesOfType(Type messageType)
        {
            // This method returns all public or internal properties of the specified type.
            const BindingFlags propertyFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var properties = from property in messageType.GetProperties(propertyFlags)
                             where IsPublicOrInternal(property.GetGetMethod(true))
                             let option = GetOptionOf(property)
                             select new RequestMessageProperty(property, option);

            return properties.ToArray();
        }

        private static bool IsPublicOrInternal(MethodBase property)
        {
            return property.IsPublic || property.IsAssembly || property.IsFamilyOrAssembly;
        }

        private static PropertyChangedOption GetOptionOf(PropertyInfo property)
        {
            var options = from attribute in property.GetCustomAttributes(typeof(RequestMessagePropertyAttribute), true)
                          let propertyAttribute = (RequestMessagePropertyAttribute) attribute
                          select propertyAttribute.Option;

            return options.SingleOrDefault();
        }

        internal static PropertyChangedOption GetPropertyChangeOption(Type messageType, string propertyName)
        {
            var property = GetPropertyByName(messageType, propertyName);
            if (property == null)
            {
                return PropertyChangedOption.None;
            }
            return property.Option;
        }

        private static RequestMessageProperty GetPropertyByName(Type messageType, string propertyName)
        {
            return GetProperties(messageType).FirstOrDefault(property => string.CompareOrdinal(property.Name, propertyName) == 0);
        }

        #endregion
    }
}
