using System;
using System.Collections.Concurrent;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// When declared on a property, marks it as the property that carries the identifier of an aggregate.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AggregateIdAttribute : Attribute
    {
        private static readonly ConcurrentDictionary<Type, Property> _Properties = new ConcurrentDictionary<Type, Property>();

        internal static TKey GetValue<TKey>(object instance) =>
            FindAggregateIdProperty<TKey>(instance).GetValue(instance);

        internal static void SetValue<TKey>(object instance, TKey value) =>
            FindAggregateIdProperty<TKey>(instance).SetValue(instance, value);

        internal static string GetPropertyName<TKey>(object instance)
        {
            Property<TKey> property;

            if (TryGetProperty(instance, out property))
            {
                return property.Name;
            }
            return "?";
        }

        private static bool TryGetProperty<TKey>(object instance, out Property<TKey> property)
        {
            try
            {
                property = FindAggregateIdProperty<TKey>(instance);
                return true;
            }
            catch
            {
                property = null;
                return false;
            }
        }        

        private static Property<TKey> FindAggregateIdProperty<TKey>(object instance) =>
            (Property<TKey>) _Properties.GetOrAdd(instance.GetType(), type => Property<TKey>.CreateProperty(type, typeof(AggregateIdAttribute)));
    }
}
