using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YellowFlare.MessageProcessing
{
    internal sealed class MessagePropertyLabelCollection
    {
        private readonly object _message;
        private readonly Lazy<Dictionary<string, string>> _labels;

        private MessagePropertyLabelCollection(object message)
        {
            _message = message;
            _labels = new Lazy<Dictionary<string, string>>(ConstructLabelMapping, true);
        }

        public string FindLabelFor(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }
            string labelName;

            if (_labels.Value.TryGetValue(propertyName, out labelName))
            {
                return labelName;
            }
            return propertyName;
        }

        private Dictionary<string, string> ConstructLabelMapping()
        {
            const string propertyNameSuffix = "Label";

            var labels = new Dictionary<string, string>();

            foreach (var property in FindAllPropertiesWith(propertyNameSuffix))
            {
                var key = property.Name.Substring(0, property.Name.Length - propertyNameSuffix.Length);
                var value = property.GetValue(_message, null) as string;

                labels.Add(key, value);
            }
            return labels;
        }

        private IEnumerable<PropertyInfo> FindAllPropertiesWith(string propertyNameSuffix)
        {
            return from property in _message.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                   where property.PropertyType == typeof(string) && property.Name.EndsWith(propertyNameSuffix)
                   select property;
        }

        private static readonly Dictionary<object, MessagePropertyLabelCollection> _Labels;      

        static MessagePropertyLabelCollection()
        {
            _Labels = new Dictionary<object, MessagePropertyLabelCollection>();
        }

        internal static void Add(object message)
        {
            lock (_Labels)
            {
                _Labels.Add(message, new MessagePropertyLabelCollection(message));
            }
        }

        internal static void Remove(object message)
        {
            lock (_Labels)
            {
                _Labels.Remove(message);
            }
        }

        internal static MessagePropertyLabelCollection For(object message)
        {
            lock (_Labels)
            {
                MessagePropertyLabelCollection labels;

                if (_Labels.TryGetValue(message, out labels))
                {
                    return labels;
                }
                return new MessagePropertyLabelCollection(message);
            }
        }
    }
}
