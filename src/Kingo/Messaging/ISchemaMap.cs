using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a one-to-one map from types to type-identifiers. A <see cref="ISchemaMap" /> can be used
    /// to maintain a relation between .NET types and serialized data.
    /// </summary>
    public interface ISchemaMap
    {
        /// <summary>
        /// Retrieves the <see cref="Type"/> that is mapped to the specified <paramref name="typeId"/>.
        /// </summary>
        /// <param name="typeId">Identifier of the requested type.</param>
        /// <returns>The type that corresponds to the specified <paramref name="typeId"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="typeId"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// No type was mapped to the specified <paramref name="typeId"/>.
        /// </exception>
        Type GetType(string typeId);

        /// <summary>
        /// Retrieves the type-id that is mapped to the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type that is mapped to the requested type-id.</param>
        /// <returns>The type-id that corresponds to the specified <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// No type-id was mapped to the specified <paramref name="type"/>.
        /// </exception>
        string GetTypeId(Type type);
    }
}
