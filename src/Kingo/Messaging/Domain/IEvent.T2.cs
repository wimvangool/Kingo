using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// When implemented by a class, represent an event message that may be published or stored in an event store and contains
    /// the identifier and version of the aggregate that published it.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    public interface IEvent<TKey, TVersion> : IEvent
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// The identifier of the aggregate that published this event.
        /// </summary>
        TKey Id
        {
            get;
            set;
        }

        /// <summary>
        /// The version of the aggregate at the time it published this event.
        /// </summary>
        TVersion Version
        {
            get;
            set;
        }
    }
}
