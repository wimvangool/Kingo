using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Kingo.Reflection;
using static Kingo.Ensure;

namespace Kingo.MicroServices.DataContracts
{
    /// <summary>
    /// Represents a portable type- or schema-identifier of a specific data-contract.
    /// </summary>
    [Serializable]
    public sealed class DataContractContentType : IEquatable<DataContractContentType>
    {
        private readonly Uri _contentTypeUri;

        private DataContractContentType(Uri contentType)
        {
            _contentTypeUri = contentType;
        }

        /// <inheritdoc />
        public override string ToString() => 
            _contentTypeUri.ToString();

        internal bool IsSystemType(out Type type)
        {
            if (_contentTypeUri.ToString().TryRemovePrefix(DefaultNamespace, out var typeNameUri))
            {
                type = FromDefaultTypeName(typeNameUri);
                return type != null;
            }
            type = null;
            return false;
        }

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            Equals(obj as DataContractContentType);

        /// <inheritdoc />
        public bool Equals(DataContractContentType other)
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
        /// Parses and converts the specified <paramref name="contentType"/> and turns it into a new <see cref="DataContractContentType"/>.
        /// </summary>
        /// <param name="contentType">The value to parse.</param>
        /// <returns>A new <see cref="DataContractContentType"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contentType"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="contentType"/> is not a valid <see cref="Uri"/>.
        /// </exception>
        public static DataContractContentType Parse(string contentType)
        {
            if (Uri.TryCreate(IsNotNull(contentType, nameof(contentType)), UriKind.Absolute, out var contentTypeUri))
            {
                return new DataContractContentType(contentTypeUri);
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

        #region [====== FromType ======]

        /// <summary>
        /// Creates and returns a new <see cref="DataContractContentType" /> that is inferred from the
        /// specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>A new <see cref="DataContractContentType"/> that was inferred from the specified <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static DataContractContentType FromType(Type type)
        {
            if (IsUnsupportedType(IsNotNull(type, nameof(type))))
            {
                throw NewUnsupportedTypeException(type);
            }
            if (type.TryGetAttributeOfType(out DataContractAttribute attribute))
            {
                return FromType(type, attribute);
            }
            return FromName(ToDefaultContentTypeName(type), ToDefaultContentTypeNamespace(type));
        }

        private static bool IsUnsupportedType(Type type) =>
            type.IsAbstract || type.IsGenericTypeDefinition;

        private static Exception NewUnsupportedTypeException(Type type)
        {
            var messageFormat = ExceptionMessages.DataContractContentType_UnsupportedType;
            var message = string.Format(messageFormat, type.FriendlyName());
            return new ArgumentException(message);
        }

        internal static DataContractContentType FromType(Type type, DataContractAttribute attribute)
        {
            if (attribute.Namespace == null)
            {
                attribute.Namespace = ToDefaultContentTypeNamespace(type);
            }
            if (attribute.Name == null)
            {
                attribute.Name = ToDefaultContentTypeName(type);
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

        private static string ToDefaultContentTypeNamespace(Type type)
        {
            if (TryGetContentTypeNamespaceFromClrNamespace(type, out var contentTypeNamespace))
            {
                return contentTypeNamespace;
            }
            return DefaultNamespace;
        }

        private static bool TryGetContentTypeNamespaceFromClrNamespace(Type type, out string contentTypeNamespace)
        {
            foreach (var attribute in GetContractNamespaceAttributesFor(type))
            {
                if (attribute.ClrNamespace == null || attribute.ClrNamespace == type.Namespace)
                {
                    contentTypeNamespace = attribute.ContractNamespace;
                    return true;
                }
            }
            contentTypeNamespace = null;
            return false;
        }

        private static IEnumerable<ContractNamespaceAttribute> GetContractNamespaceAttributesFor(Type type) =>
            from attribute in type.Assembly.GetAttributesOfType<ContractNamespaceAttribute>()
            where attribute.ContractNamespace != null
            orderby attribute.ClrNamespace descending
            select attribute;

        private static string ToDefaultContentTypeName(Type type) =>
            ToUriTypeName(type.FullName);

        private static Type FromDefaultTypeName(string typeNameUri)
        {
            try
            {
                return Type.GetType(ToClrTypeName(typeNameUri), false, true);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private static string ToUriTypeName(string typeNameClr) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(typeNameClr));

        private static string ToClrTypeName(string typeNameUri) =>
            Encoding.UTF8.GetString(Convert.FromBase64String(typeNameUri));

        private static Exception NewAttributeNotValidException(DataContractAttribute attribute, Exception exception)
        {
            var messageFormat = ExceptionMessages.DataContractType_AttributeNotValid;
            var message = string.Format(messageFormat, attribute.Name, attribute.Namespace);
            return new ArgumentException(message, nameof(attribute), exception);
        }

        #endregion

        #region [====== FromName ======]

        internal const string DefaultNamespace = "http://schemas.datacontract.org/2004/07/";

        /// <summary>
        /// Creates and returns a new <see cref="DataContractContentType" /> that is inferred from the
        /// specified <paramref name="contentTypeName"/> and <paramref name="contentTypeNamespace" />.
        /// </summary>
        /// <param name="contentTypeName">Specific name of the type-identifier of the data-contract.</param>
        /// <param name="contentTypeNamespace">Specific namespace in which the type-identifier resides.</param>
        /// <returns>A new <see cref="DataContractContentType"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contentTypeName"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="contentTypeName"/> is not a valid Uri (relative)
        /// - or -
        /// <paramref name="contentTypeNamespace"/> is not a valid Uri (absolute).
        /// </exception>
        public static DataContractContentType FromName(string contentTypeName, string contentTypeNamespace = null) =>
            FromName(ParseContentTypeName(contentTypeName), ParseContentTypeNamespace(contentTypeNamespace));

        private static DataContractContentType FromName(Uri contentTypeName, Uri contentTypeNamespace) =>
            new DataContractContentType(new Uri(contentTypeNamespace, contentTypeName));

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
                return new Uri(DefaultNamespace);
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
