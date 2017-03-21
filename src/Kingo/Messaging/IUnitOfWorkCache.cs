using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a cache in which items can be stored that may have be accessed during a single unit of work.
    /// </summary>
    public interface IUnitOfWorkCache
    {
        /// <summary>
        /// Stores an item to or retrieves an item from the cache using its unique <paramref name="key"/>.
        /// </summary>
        /// <param name="key">A unique key identifying a certain instance.</param>
        /// <returns>
        /// The object associated with the specified <paramref name="key"/>, or <c>null</c> if no item
        /// with the specified <paramref name="key"/> was found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> or <paramref name="value"/> is <c>null</c>.
        /// </exception>
        object this[string key]
        {
            get;
            set;
        }

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key">A unique key identifying a certain instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is <c>null</c>.
        /// </exception>
        void Remove(string key);
    }
}
