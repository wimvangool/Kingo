using System;

namespace Kingo.MicroServices.DataContracts
{
    /// <summary>
    /// When implemented by a class, represents a factory of <see cref="IDataContractSerializer"/> instances
    /// based on a specific name.
    /// </summary>
	public interface IDataContractSerializerFactory
    {
        /// <summary>
        /// Creates and returns a serializer that is associated with the specified <paramref name="associatedType"/>.
        /// If <paramref name="associatedType"/> is <c>null</c>, or no serializer is associated with the specified
        /// type, the default serializer is returned.
        /// </summary>
        /// <param name="associatedType">Type that is associated with the serializer to return..</param>
        /// <returns>The serializer that is associated with the specified <paramref name="associatedType"/>.</returns>
        IDataContractSerializer CreateSerializer(Type associatedType = null);
    }
}
