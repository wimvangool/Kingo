using System;

namespace Kingo.Serialization
{
    /// <summary>
    /// When implemented by a class, represents a factory of <see cref="ISerializer"/>-objects
    /// that are created based on a specific, associated type.
    /// </summary>
    public interface ISerializerFactory
    {
        /// <summary>
        /// Creates and returns a serializer that is associated with the specified <paramref name="associatedType"/>.
        /// If <paramref name="associatedType"/> is <c>null</c>, or no serializer is associated with the specified
        /// type, the default serializer is returned.
        /// </summary>
        /// <param name="associatedType">Type that is associated with the serializer to return..</param>
        /// <returns>The serializer that is associated with the specified <paramref name="associatedType"/>.</returns>
        ISerializer CreateSerializerFor(Type associatedType = null);
    }
}
