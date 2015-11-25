using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a handle to an item in cache.
    /// </summary>
    /// <typeparam name="T">Type of the item in cache.</typeparam>
    public interface IDependencyCacheEntry<T> : IDisposable
    {
        /// <summary>
        /// Attempts to retrieve the value from cache.
        /// </summary>
        /// <param name="value">
        /// When this method returns <c>true</c>, contains the value that was retrieved from cache;
        /// otherwise, contains the default value of <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> when the item was still in cache; otherwise <c>false</c>.
        /// </returns>
        bool TryGetValue(out T value);        
    }
}
