using System;
using System.Collections.Concurrent;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// When declared on a property, marks it as the property that carries the version of an aggregate.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AggregateVersionAttribute : Attribute
    {
        private static readonly ConcurrentDictionary<Type, Property> _Properties = new ConcurrentDictionary<Type, Property>();

        internal static TVersion GetValue<TVersion>(object instance) =>
            FindAggregateVersionProperty<TVersion>(instance).GetValue(instance);

        internal static void SetValue<TVersion>(object instance, TVersion value) =>
            FindAggregateVersionProperty<TVersion>(instance).SetValue(instance, value);

        internal static string GetPropertyName<TVersion>(object instance)
        {
            Property<TVersion> property;

            if (TryGetProperty(instance, out property))
            {
                return property.Name;
            }
            return "?";
        }

        private static bool TryGetProperty<TVersion>(object instance, out Property<TVersion> property)
        {
            try
            {
                property = FindAggregateVersionProperty<TVersion>(instance);
                return true;
            }
            catch
            {
                property = null;
                return false;
            }
        }        

        private static Property<TVersion> FindAggregateVersionProperty<TVersion>(object instance) =>
            (Property<TVersion>) _Properties.GetOrAdd(instance.GetType(), type => Property<TVersion>.CreateProperty(type, typeof(AggregateVersionAttribute)));
    }
}
