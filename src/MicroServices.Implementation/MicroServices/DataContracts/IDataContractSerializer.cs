using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kingo.MicroServices.DataContracts
{
    /// <summary>
    /// When implemented by a class, represents a component that can serialize and deserialize data-contract
    /// objects to and from <see cref="DataContractBlob"/> objects.
    /// </summary>
	public interface IDataContractSerializer
    {
        #region [====== Clone ======]

        /// <summary>
        /// Clones the specified <paramref name="content"/>.
        /// </summary>
        /// <typeparam name="TDataContract">Type of the data-contract to clone.</typeparam>
        /// <param name="content">The (content of the) data-contract to clone.</param>
        /// <returns>A copy or clone of the specified <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The serializer could not determine the content-type of the specified <paramref name="content"/>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Serialization or deserialization of the object failed.
        /// </exception>
        TDataContract Clone<TDataContract>(TDataContract content) =>
            (TDataContract) Deserialize(Serialize(content));

        #endregion

        #region [====== Serialize ======]

        /// <summary>
        /// Serializes the specified <paramref name="content"/> and returns the resulting <see cref="DataContractBlob"/>.
        /// </summary>
        /// <param name="content">The content (data-contract) to serialize.</param>
        /// <returns>A blob containing the serialized content and a content type-identifier.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The serializer could not determine the content-type of the specified <paramref name="content"/>.
        /// </exception>
        DataContractBlob Serialize(object content);

        #endregion

        #region [====== Deserialize ======]

        /// <summary>
        /// Deserializes the specified <paramref name="content"/> into an instance of the type identified
        /// by the specified <paramref name="contentType"/>. If <paramref name="updateToLatestVersion"/>
        /// is <c>true</c>, the returned type may be different if the the data-contract implements the
        /// <see cref="IDataContract"/> interface and a later version of the contract is available.
        /// </summary>
        /// <param name="content">The content to deserialize.</param>
        /// <param name="contentType">Type-identifier of the data-contract.</param>
        /// <param name="updateToLatestVersion">
        /// Indicates whether or not the deserialized object should be updated to the latest version,
        /// provided the object implements <see cref="IDataContract"/> and a newer version exists.
        /// </param>
        /// <returns>The deserialized (and possibly updated) data-contract.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> or <paramref name="contentType"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The serializer is not able to deserialize the specified <paramref name="content"/>
        /// - or -
        /// the specified <paramref name="contentType"/> is not a valid type-identifier.
        /// </exception>
        object Deserialize(IReadOnlyList<byte> content, string contentType, bool updateToLatestVersion = false);

        /// <summary>
        /// Deserializes the specified <paramref name="blob"/> into an instance of the type identified
        /// by the blob's <see cref="DataContractBlob.ContentType"/>. If <paramref name="updateToLatestVersion"/>
        /// is <c>true</c>, the returned type may be different if the the data-contract implements the
        /// <see cref="IDataContract"/> interface and a later version of the contract is available.
        /// </summary>
        /// <param name="blob">The blob to deserialize.</param>
        /// <param name="updateToLatestVersion">
        /// Indicates whether or not the deserialized object should be updated to the latest version,
        /// provided the object implements <see cref="IDataContract"/> and a newer version exists.
        /// </param>
        /// <returns>The deserialized (and possibly updated) data-contract.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="blob"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The serializer is not able to deserialize the specified <paramref name="blob"/>.
        /// </exception>
        object Deserialize(DataContractBlob blob, bool updateToLatestVersion = false);

        #endregion
    }
}
