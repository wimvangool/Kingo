using System;
using System.Runtime.Serialization;
using static Kingo.Ensure;

namespace Kingo.MicroServices.DataContracts
{
    /// <summary>
    /// Represents a portable type-or schema-identifier of a specific data-contract.
    /// </summary>
    [Serializable]
    public sealed class DataContractType : IEquatable<DataContractType>
    {
        private readonly Uri _contentTypeUri;

        private DataContractType(Uri contentType)
        {
            _contentTypeUri = contentType;
        }

        /// <inheritdoc />
        public override string ToString() => 
            _contentTypeUri.ToString().ToLowerInvariant();

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            Equals(obj as DataContractType);

        /// <inheritdoc />
        public bool Equals(DataContractType other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return _contentTypeUri.Equals(other._contentTypeUri);
        }

        /// <inheritdoc />
        public override int GetHashCode() =>
            _contentTypeUri.GetHashCode();

        #endregion

        #region [====== Parse ======]

        /// <summary>
        /// Parses and converts the specified <paramref name="contentType"/> and turns it into a new <see cref="DataContractType"/>.
        /// </summary>
        /// <param name="contentType">The value to parse.</param>
        /// <returns>A new <see cref="DataContractType"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contentType"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="contentType"/> is not a valid <see cref="Uri"/>.
        /// </exception>
        public static DataContractType Parse(string contentType)
        {
            if (Uri.TryCreate(IsNotNull(contentType, nameof(contentType)), UriKind.Absolute, out var contentTypeUri))
            {
                return new DataContractType(contentTypeUri);
            }
            throw NewContentTypeNotValidException(contentType);
        }

        private static Exception NewContentTypeNotValidException(string contentType)
        {
            var messageFormat = ExceptionMessages.DataContractType_ContentTypeNotValid;
            var message = string.Format(messageFormat, contentType);
            return new ArgumentException(message, nameof(contentType));
        }

        #endregion

        #region [====== FromAttribute ======]

        /// <summary>
        /// Creates and returns a new <see cref="DataContractType" /> that is inferred from the
        /// specified <paramref name="attribute"/>.
        /// </summary>
        /// <param name="attribute">The attribute containing the name and namespace of the data-contract.</param>
        /// <returns>A new <see cref="DataContractType"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="attribute"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="attribute"/>'s name is not set
        /// - or -
        /// The <paramref name="attribute"/>'s namespace is not a valid <see cref="Uri"/>.
        /// </exception>
        public static DataContractType FromAttribute(DataContractAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }
            try
            {
                return FromName(attribute.Name, attribute.Namespace);
            }
            catch (ArgumentException exception)
            {
                throw NewAttributeNotValidException(attribute, exception);
            }
        }

        private static Exception NewAttributeNotValidException(DataContractAttribute attribute, Exception exception)
        {
            var messageFormat = ExceptionMessages.DataContractType_AttributeNotValid;
            var message = string.Format(messageFormat, attribute.Name, attribute.Namespace);
            return new ArgumentException(message, nameof(attribute), exception);
        }

        #endregion

        #region [====== FromName ======]

        private static readonly Uri _DefaultNamespace = new Uri("https://schemas.kingo.net/2020/05/");

        /// <summary>
        /// Creates and returns a new <see cref="DataContractType" /> that is inferred from the
        /// specified <paramref name="contentTypeName"/> and <paramref name="contentTypeNamespace" />.
        /// </summary>
        /// <param name="contentTypeName">Specific name of the type-identifier of the data-contract.</param>
        /// <param name="contentTypeNamespace">Specific namespace in which the type-identifier resides.</param>
        /// <returns>A new <see cref="DataContractType"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contentTypeName"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="contentTypeName"/> is not a valid Uri (relative)
        /// - or -
        /// <paramref name="contentTypeNamespace"/> is not a valid Uri (absolute).
        /// </exception>
        public static DataContractType FromName(string contentTypeName, string contentTypeNamespace = null) =>
            FromName(ParseContentTypeName(contentTypeName), ParseContentTypeNamespace(contentTypeNamespace));

        private static DataContractType FromName(Uri contentTypeName, Uri contentTypeNamespace) =>
            new DataContractType(new Uri(contentTypeNamespace, contentTypeName));

        private static Uri ParseContentTypeName(string contentTypeName)
        {
            if (Uri.TryCreate(IsNotNull(contentTypeName, nameof(contentTypeName)), UriKind.Relative, out var contentTypeNameUri))
            {
                return contentTypeNameUri;
            }
            throw NewContentTypeNameNotValidException(contentTypeName);
        }

        private static Uri ParseContentTypeNamespace(string contentTypeNamespace)
        {
            if (contentTypeNamespace == null)
            {
                return _DefaultNamespace;
            }
            if (Uri.TryCreate(contentTypeNamespace, UriKind.Absolute, out var contentTypeNamespaceUri))
            {
                return contentTypeNamespaceUri;
            }
            throw NewContentTypeNamespaceNotValidException(contentTypeNamespace);
        }

        private static Exception NewContentTypeNameNotValidException(string contentTypeName)
        {
            var messageFormat = ExceptionMessages.DataContractType_ContentTypeNameNotValid;
            var message = string.Format(messageFormat, contentTypeName);
            return new ArgumentException(message, nameof(contentTypeName));
        }

        private static Exception NewContentTypeNamespaceNotValidException(string contentTypeNamespace)
        {
            var messageFormat = ExceptionMessages.DataContractType_ContentTypeNamespaceNotValid;
            var message = string.Format(messageFormat, contentTypeNamespace);
            return new ArgumentException(message, nameof(contentTypeNamespace));
        }

        #endregion
    }
}
