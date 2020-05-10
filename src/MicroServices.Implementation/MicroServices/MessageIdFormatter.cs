using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MessageIdFormatter
    {
        private readonly string _messageIdFormat;
        private readonly Func<object, object>[] _getPropertyValueDelegates;

        private MessageIdFormatter(string messageIdFormat, IEnumerable<Func<object, object>> getPropertyValueDelegates)
        {
            _messageIdFormat = messageIdFormat;
            _getPropertyValueDelegates = getPropertyValueDelegates.ToArray();
        }

        public override string ToString() =>
            _messageIdFormat;

        public string FormatMessageId(object content) =>
            string.Format(_messageIdFormat, GetPropertyValuesOf(content).ToArray());

        private IEnumerable<object> GetPropertyValuesOf(object content) =>
            _getPropertyValueDelegates.Select(property => property.Invoke(content));

        #region [====== FromContentType ======]

        public static MessageIdFormatter FromContentType(Type contentType, string messageIdFormat, IEnumerable<string> messageIdProperties) =>
            new MessageIdFormatter(messageIdFormat, CreatePropertyValueDelegates(contentType, messageIdProperties));

        private static IEnumerable<Func<object, object>> CreatePropertyValueDelegates(Type contentType, IEnumerable<string> messageIdProperties) =>
            from messageIdProperty in messageIdProperties
            select CreatePropertyValueDelegate(contentType, messageIdProperty);

        private static Func<object, object> CreatePropertyValueDelegate(Type contentType, string messageIdPropertyName)
        {
            var contentParameter = Expression.Parameter(typeof(object), "content");
            var contentExpression = Expression.Convert(contentParameter, contentType);
            var property = FindPropertyByName(contentType, messageIdPropertyName);
            var propertyValueExpression = Expression.Property(contentExpression, property) as Expression;

            if (property.PropertyType != typeof(object))
            {
                propertyValueExpression = Expression.Convert(propertyValueExpression, typeof(object));
            }
            return Expression.Lambda<Func<object, object>>(propertyValueExpression, contentParameter).Compile();
        }

        private static PropertyInfo FindPropertyByName(Type contentType, string propertyName)
        {
            var properties = FindPropertyCandidates(contentType, propertyName).ToArray();
            if (properties.Length == 0)
            {
                throw NewPropertyNotFoundException(contentType, propertyName);
            }
            if (properties.Length == 1)
            {
                return properties[0];
            }
            throw NewPropertyIsAmbiguousException(contentType, propertyName, properties);
        }

        private static IEnumerable<PropertyInfo> FindPropertyCandidates(Type contentType, string propertyName) =>
            from property in contentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            where property.Name == propertyName && property.CanRead && property.GetIndexParameters().Length == 0
            select property;

        private static Exception NewPropertyNotFoundException(Type contentType, string propertyName)
        {
            var messageFormat = ExceptionMessages.MessageIdFormatter_PropertyNotFound;
            var message = string.Format(messageFormat, propertyName, contentType.FriendlyName());
            return new ArgumentException(message);
        }

        private static Exception NewPropertyIsAmbiguousException(Type contentType, string propertyName, PropertyInfo[] properties)
        {
            var messageFormat = ExceptionMessages.MessageIdFormatter_PropertyIsAmbiguous;
            var message = string.Format(messageFormat, propertyName, contentType.FriendlyName(), FormatProperties(properties));
            return new ArgumentException(message);
        }

        private static string FormatProperties(IEnumerable<PropertyInfo> properties) =>
            string.Join(", ", properties.Select(FormatProperty));

        private static string FormatProperty(PropertyInfo property) =>
            $"{property.DeclaringType.FriendlyName()}.{property.Name}";

        #endregion
    }
}
