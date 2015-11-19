using System;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// Represents a cache to which items can be added.
    /// </summary>
    public interface IDependencyCache
    {
        /// <summary>
        /// Add the specified value to the cache and returns a handle with which the item can be retrieved
        /// or removed again.
        /// </summary>
        /// <typeparam name="T">Type of the value to add.</typeparam>
        /// <param name="value">The value to add.</param>
        /// <returns>A handle with which the item can be retrieved or removed again.</returns>
        IDependencyCacheEntry<T> Add<T>(T value);

        /// <summary>
        /// Add the specified value to the cache and returns a handle with which the item can be retrieved
        /// or removed again.
        /// </summary>
        /// <typeparam name="T">Type of the value to add.</typeparam>
        /// <param name="value">The value to add.</param>
        /// <param name="valueRemovedCallback">
        /// The callback that is called as soon as the item is removed from the cache.
        /// </param>
        /// <returns>A handle with which the item can be retrieved or removed again.</returns>
        IDependencyCacheEntry<T> Add<T>(T value, Action<T> valueRemovedCallback);
    }
}
