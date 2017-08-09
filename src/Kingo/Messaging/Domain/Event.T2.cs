using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IEvent{T, S}"/> interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    [Serializable]
    public abstract class Event<TKey, TVersion> : Event, IEvent<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        TKey IEvent<TKey, TVersion>.AggregateId
        {
            get { return AggregateIdAttribute.GetValue<TKey>(this); }
            set { AggregateIdAttribute.SetValue(this, value); }
        }

        TVersion IEvent<TKey, TVersion>.AggregateVersion
        {
            get { return AggregateVersionAttribute.GetValue<TVersion>(this); }
            set { AggregateVersionAttribute.SetValue(this, value); }
        }

        /// <inheritdoc />
        public override string ToString() =>
            ToString(AggregateIdAttribute.GetPropertyName<TKey>(this), AggregateVersionAttribute.GetPropertyName<TVersion>(this));

        private string ToString(string aggregateIdProperty, string aggregateVersionProperty) =>
            $"{GetType().FriendlyName()} ({aggregateIdProperty}, {aggregateVersionProperty})";
    }
}
