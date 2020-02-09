using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents an item that has a unique identifier or key.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier.</typeparam>
    public interface IKeyedItem<out TKey>
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// Unique identifier of the item.
        /// </summary>
        TKey Id
        {
            get;
        }
    }
}
