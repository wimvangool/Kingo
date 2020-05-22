using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Kingo.Ensure;

namespace Kingo.MicroServices.DataContracts
{
    /// <summary>
    /// Represents a blob that contains the serialized data of a data-contract.
    /// </summary>
    [Serializable]
    public sealed class DataContractBlob : IEquatable<DataContractBlob>
    {
        private DataContractBlob(DataContractType contentType, byte[] content)
        {
            ContentType = contentType;
            Content = new MemoryStream(content, false);
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{ContentType} ({Content.Length:N0} byte(s))";

        #region [====== ContentType & Content ======]

        /// <summary>
        /// Gets the type-identifier of the <see cref="Content"/>.
        /// </summary>
        public DataContractType ContentType
        {
            get;
        }

        /// <summary>
        /// Gets the stream containing the content of the blob.
        /// </summary>
        public MemoryStream Content
        {
            get;
        }

        #endregion

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            Equals(obj as DataContractBlob);

        /// <inheritdoc />
        public bool Equals(DataContractBlob other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return
                ContentType.Equals(other.ContentType) &&
                Content.Length == other.Content.Length &&
                Content.ToArray().SequenceEqual(other.Content.ToArray());
        }

        /// <inheritdoc />
        public override int GetHashCode() =>
            ContentType.GetHashCode();

        #endregion

        #region [====== FromStream ======]

        /// <summary>
        /// Creates and returns a new <see cref="DataContractBlob"/> with the specified <paramref name="contentType"/> and <paramref name="content"/>.
        /// </summary>
        /// <param name="contentType">Type-identifier of the content.</param>
        /// <param name="content">The content as a (readable) stream of bytes.</param>
        /// <returns>A new <see cref="DataContractBlob"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contentType"/> or <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="contentType"/> is not a valid type-identifier.
        /// - or -
        /// <paramref name="content"/> could not be read.
        /// </exception>
        public static DataContractBlob FromStream(string contentType, Stream content) =>
            FromStream(DataContractType.Parse(contentType), content);

        /// <summary>
        /// Creates and returns a new <see cref="DataContractBlob"/> with the specified <paramref name="contentType"/> and <paramref name="content"/>.
        /// </summary>
        /// <param name="contentType">Type-identifier of the content.</param>
        /// <param name="content">The content as a (readable) stream of bytes.</param>
        /// <returns>A new <see cref="DataContractBlob"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contentType"/> or <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="content"/> could not be read.
        /// </exception>
        public static DataContractBlob FromStream(DataContractType contentType, Stream content) =>
            new DataContractBlob(IsNotNull(contentType, nameof(contentType)), ReadBytes(IsNotNull(content)));

        private static byte[] ReadBytes(Stream content)
        {
            using (var contentBuffer = new MemoryStream())
            {
                try
                {
                    content.CopyTo(contentBuffer);
                }
                catch (NotSupportedException exception)
                {
                    throw NewContentCouldNotBeReadException(content, exception);
                }
                catch (IOException exception)
                {
                    throw NewContentCouldNotBeReadException(content, exception);
                }
                catch (ObjectDisposedException exception)
                {
                    throw NewContentCouldNotBeReadException(content, exception);
                }
                return contentBuffer.ToArray();
            }
        }

        /// <summary>
        /// Creates and returns a new <see cref="DataContractBlob"/> with the specified <paramref name="contentType"/> and <paramref name="content"/>.
        /// </summary>
        /// <param name="contentType">Type-identifier of the content.</param>
        /// <param name="content">The content as a (readable) stream of bytes.</param>
        /// <returns>A new <see cref="DataContractBlob"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contentType"/> or <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="contentType"/> is not a valid type-identifier.
        /// - or -
        /// <paramref name="content"/> could not be read.
        /// </exception>
        public static Task<DataContractBlob> FromStreamAsync(string contentType, Stream content) =>
            FromStreamAsync(DataContractType.Parse(contentType), content);

        /// <summary>
        /// Creates and returns a new <see cref="DataContractBlob"/> with the specified <paramref name="contentType"/> and <paramref name="content"/>.
        /// </summary>
        /// <param name="contentType">Type-identifier of the content.</param>
        /// <param name="content">The content as a (readable) stream of bytes.</param>
        /// <returns>A new <see cref="DataContractBlob"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contentType"/> or <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="content"/> could not be read.
        /// </exception>
        public static async Task<DataContractBlob> FromStreamAsync(DataContractType contentType, Stream content) =>
            new DataContractBlob(IsNotNull(contentType, nameof(contentType)), await ReadBytesAsync(IsNotNull(content)));

        private static async Task<byte[]> ReadBytesAsync(Stream content)
        {
            using (var contentBuffer = new MemoryStream())
            {
                try
                {
                    await content.CopyToAsync(contentBuffer);
                }
                catch (NotSupportedException exception)
                {
                    throw NewContentCouldNotBeReadException(content, exception);
                }
                catch (IOException exception)
                {
                    throw NewContentCouldNotBeReadException(content, exception);
                }
                catch (ObjectDisposedException exception)
                {
                    throw NewContentCouldNotBeReadException(content, exception);
                }
                return contentBuffer.ToArray();
            }
        }

        private static Exception NewContentCouldNotBeReadException(Stream content, Exception exception)
        {
            var messageFormat = ExceptionMessages.DataContractBlob_ContentCouldNotBeRead;
            var message = string.Format(messageFormat, content.Length);
            return new ArgumentException(message, nameof(content), exception);
        }

        #endregion

        #region [====== FromBytes ======]

        /// <summary>
        /// Creates and returns a new <see cref="DataContractBlob"/> with the specified <paramref name="contentType"/> and <paramref name="content"/>.
        /// </summary>
        /// <param name="contentType">Type-identifier of the content.</param>
        /// <param name="content">The content as a set of raw bytes.</param>
        /// <returns>A new <see cref="DataContractBlob"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contentType"/> or <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="contentType"/> is not a valid type-identifier.
        /// </exception>
        public static DataContractBlob FromBytes(string contentType, IEnumerable<byte> content) =>
            FromBytes(DataContractType.Parse(contentType), content);

        /// <summary>
        /// Creates and returns a new <see cref="DataContractBlob"/> with the specified <paramref name="contentType"/> and <paramref name="content"/>.
        /// </summary>
        /// <param name="contentType">Type-identifier of the content.</param>
        /// <param name="content">The content as a set of raw bytes.</param>
        /// <returns>A new <see cref="DataContractBlob"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contentType"/> or <paramref name="content"/> is <c>null</c>.
        /// </exception>
        public static DataContractBlob FromBytes(DataContractType contentType, IEnumerable<byte> content) =>
            new DataContractBlob(IsNotNull(contentType, nameof(contentType)), IsNotNull(content, nameof(content)).ToArray());

        /// <summary>
        /// Creates and returns a new <see cref="DataContractBlob"/> with the specified <paramref name="contentType"/> and <paramref name="content"/>.
        /// </summary>
        /// <param name="contentType">Type-identifier of the content.</param>
        /// <param name="content">The content as a set of raw bytes.</param>
        /// <returns>A new <see cref="DataContractBlob"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contentType"/> or <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="contentType"/> is not a valid type-identifier.
        /// </exception>
        public static DataContractBlob FromBytes(string contentType, byte[] content) =>
            FromBytes(DataContractType.Parse(contentType), content);

        /// <summary>
        /// Creates and returns a new <see cref="DataContractBlob"/> with the specified <paramref name="contentType"/> and <paramref name="content"/>.
        /// </summary>
        /// <param name="contentType">Type-identifier of the content.</param>
        /// <param name="content">The content as a set of raw bytes.</param>
        /// <returns>A new <see cref="DataContractBlob"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contentType"/> or <paramref name="content"/> is <c>null</c>.
        /// </exception>
        public static DataContractBlob FromBytes(DataContractType contentType, byte[] content) =>
            new DataContractBlob(IsNotNull(contentType, nameof(contentType)), CopyBytes(IsNotNull(content, nameof(content))));

        private static byte[] CopyBytes(byte[] content)
        {
            var contentCopy = new byte[content.LongLength];
            Array.Copy(content, contentCopy, content.LongLength);
            return contentCopy;
        }

        #endregion
    }
}
