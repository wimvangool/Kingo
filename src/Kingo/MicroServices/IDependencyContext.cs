using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a context in which resolved dependencies can be stored,
    /// so that their lifetime is associated to the lifetime of the context.
    /// </summary>
    public interface IDependencyContext : IDisposable
    {
        /// <summary>
        /// Stores the <paramref name="value"/> in the context.
        /// </summary>
        /// <param name="key">Unique key to associate with the <paramref name="value"/>.</param>
        /// <param name="value">The value to store.</param>
        /// <exception cref="ObjectDisposedException">
        /// This context has been disposed.
        /// </exception>        
        void SetValue(Guid key, object value);

        /// <summary>
        /// Retrieves the value that was stored using the specified <paramref name="key"/>, or <c>null</c>
        /// if the value was not found in this context.
        /// </summary>
        /// <param name="key">The key to which the value was associated.</param>
        /// <returns>The stored value, or <c>null</c> if the value was not found.</returns>
        /// <exception cref="ObjectDisposedException">
        /// This context has been disposed.
        /// </exception>
        object GetValue(Guid key);

        /// <summary>
        /// Removes and disposes the stored value (if it implements <see cref="IDisposable" />.
        /// </summary>
        /// <param name="key">The key to which the value was associated.</param>
        /// <returns>
        /// <c>true</c> if a value was stored with the specified <paramref name="key"/> and is now removed;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// This context has been disposed.
        /// </exception>
        bool RemoveValue(Guid key);
    }
}
