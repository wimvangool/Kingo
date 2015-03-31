using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace System.ComponentModel.DataAnnotations
{
    internal sealed class RequestMessageLabelProvider
    {
        private readonly object _message;
        private readonly Lazy<Dictionary<string, string>> _labels;

        private RequestMessageLabelProvider(object message)
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

            Debug.WriteLine("Message Type = {0}", _message.GetType().Name as object);

            foreach (var property in FindAllPropertiesWith(propertyNameSuffix))
            {                
                var key = property.Name.Substring(0, property.Name.Length - propertyNameSuffix.Length);
                var value = property.GetValue(_message, null) as string;

                labels.Add(key, value);

                Debug.WriteLine("\tProperty {0} = {1}", property.Name, value);
            }
            return labels;
        }

        private IEnumerable<PropertyInfo> FindAllPropertiesWith(string propertyNameSuffix)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

            return from property in _message.GetType().GetProperties(bindingFlags)
                   where property.PropertyType == typeof(string) && property.Name.EndsWith(propertyNameSuffix)
                   select property;
        }

        private static readonly Dictionary<object, RequestMessageLabelProvider> _Labels;      

        static RequestMessageLabelProvider()
        {
            _Labels = new Dictionary<object, RequestMessageLabelProvider>();
        }

        internal static void Add(object message)
        {
            lock (_Labels)
            {
                _Labels.Add(message, new RequestMessageLabelProvider(message));
            }
        }

        internal static void Remove(object message)
        {
            lock (_Labels)
            {
                _Labels.Remove(message);
            }
        }

        internal static RequestMessageLabelProvider For(object message)
        {
            lock (_Labels)
            {
                RequestMessageLabelProvider labels;

                if (_Labels.TryGetValue(message, out labels))
                {
                    return labels;
                }
                return new RequestMessageLabelProvider(message);
            }
        }
    }
}
