using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="ISnapshot{T, S}"/> interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    [Serializable]
    public abstract class Snapshot<TKey, TVersion> : Snapshot, ISnapshot<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        TKey ISnapshot<TKey, TVersion>.AggregateId =>
            AggregateIdAttribute.GetValue<TKey>(this);        

        TVersion ISnapshot<TKey, TVersion>.AggregateVersion =>
            AggregateVersionAttribute.GetValue<TVersion>(this);                

        /// <inheritdoc />
        public override string ToString() =>
            ToString(AggregateIdAttribute.GetPropertyName<TKey>(this), AggregateVersionAttribute.GetPropertyName<TVersion>(this));

        private string ToString(string aggregateIdProperty, string aggregateVersionProperty) =>
            $"{GetType().FriendlyName()} ({aggregateIdProperty}, {aggregateVersionProperty})";
    }
}
