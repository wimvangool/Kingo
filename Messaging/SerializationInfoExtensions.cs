using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace System.ComponentModel
{
    /// <summary>
    /// Contains extension-methods for the <see cref="SerializationInfo" /> class.
    /// </summary>
    public static class SerializationInfoExtensions
    {
        /// <summary>
        /// Adds a collection into the <see cref="SerializationInfo" /> store, where <paramref name="collection"/> is associated with <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the values within the <paramref name="collection"/>.</typeparam>
        /// <param name="info">The serialization info.</param>
        /// <param name="name">Name of the collection to store.</param>
        /// <param name="collection">The collection to store.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> or <paramref name="name"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// A value has already been associated with <paramref name="name"/>.
        /// </exception>
        public static void AddCollection<TValue>(this SerializationInfo info, string name, ObservableCollection<TValue> collection)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue(name, collection.ToArray(), typeof(TValue[]));
        }
    }
}
