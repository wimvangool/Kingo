using System;
using System.Runtime.Serialization;
using Kingo.Reflection;

namespace Kingo.Serialization
{
    /// <summary>
    /// When implemented, represents a serializer that can serialize and deserialize
    /// objects using a specific serialization-protocol.
    /// </summary>
    public abstract class Serializer : ISerializer
    {
        /// <inheritdoc />
        public abstract string Serialize(object instance);

        /// <inheritdoc />
        public abstract object Deserialize(string value, Type type);

        /// <summary>
        /// If the specified <paramref name="instance"/> is not of the specified <paramref name="type"/>,
        /// attempts to convert <paramref name="instance"/> to an instance of this <paramref name="type"/>.
        /// </summary>
        /// <param name="instance">The instance to convert.</param>
        /// <param name="type">The type to convert the instance to.</param>
        /// <returns>
        /// The specified <paramref name="instance"/> if it is already of the specified <paramref name="type"/>;
        /// otherwise the converted value of the specified <paramref name="type"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// The conversion failed.
        /// </exception>
        public static object Convert(object instance, Type type)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (type.IsInstanceOfType(instance))
            {
                return instance;
            }
            try
            {
                return System.Convert.ChangeType(instance, type);
            }
            catch (Exception exception)
            {
                throw NewTypeConversionFailedException(instance.GetType(), type, exception);
            }
        }

        private static Exception NewTypeConversionFailedException(Type sourceType, Type targetType, Exception exception)
        {
            var messageFormat = ExceptionMessages.Serializer_TypeConversionFailed;
            var message = string.Format(messageFormat, sourceType.FriendlyName(), targetType.FriendlyName());
            return new SerializationException(message, exception);
        }
    }
}
