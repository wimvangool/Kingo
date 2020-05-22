using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Serialization;
using static Kingo.Ensure;

namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// When implemented by a class, represents a mapping between a <see cref="ISerializer"/> and a set of associated types.
    /// </summary>
    public interface ISerializerTypeMapping
    {
        /// <summary>
        /// Associates the specified <typeparamref name="TAssociatedType"/> with the serializer.
        /// </summary>
        /// <typeparam name="TAssociatedType">The type to associate with the serializer.</typeparam>
        /// <returns>The new mapping where the type has added.</returns>
        ISerializerTypeMapping For<TAssociatedType>() =>
            For(typeof(TAssociatedType));

        /// <summary>
        /// Associates the specified <paramref name="associatedTypes"/> with the serializer.
        /// </summary>
        /// <param name="associatedTypes">The types to associate with the serializer.</param>
        /// <returns>The new mapping where the types have added.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="associatedTypes"/> is <c>null</c>.
        /// </exception>
        ISerializerTypeMapping For(params Type[] associatedTypes) =>
            For(IsNotNull(associatedTypes, nameof(associatedTypes)).AsEnumerable());

        /// <summary>
        /// Associates the specified <paramref name="associatedTypes"/> with the serializer.
        /// </summary>
        /// <param name="associatedTypes">The types to associate with the serializer.</param>
        /// <returns>The new mapping where the types have added.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="associatedTypes"/> is <c>null</c>.
        /// </exception>
        ISerializerTypeMapping For(IEnumerable<Type> associatedTypes);
    }
}
