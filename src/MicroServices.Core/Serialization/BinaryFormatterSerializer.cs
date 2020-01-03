using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Kingo.Serialization
{
    /// <summary>
    /// Represents a <see cref="ISerializer" /> that serializes objects using a <see cref="BinaryFormatter" />.
    /// </summary>
    public class BinaryFormatterSerializer : Serializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormatterSerializer" /> class.
        /// </summary>
        public BinaryFormatterSerializer()
        {
            Formatter = new BinaryFormatter();
        }

        /// <summary>
        /// The formatter that is used to serialize and deserialize objects.
        /// </summary>
        public BinaryFormatter Formatter
        {
            get;
        }

        #region [====== Serialize & Deserialize ======]

        /// <inheritdoc />
        public override string Serialize(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            using (var stream = new MemoryStream())
            {
                Formatter.Serialize(stream, instance);
                return Encode(stream.ToArray());
            }
        }

        /// <inheritdoc />
        public override object Deserialize(string value, Type type)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            using (var stream = new MemoryStream(DecodeSafe(value)))
            {
                return Convert(Formatter.Deserialize(stream), type);
            }
        }

        #endregion

        #region [====== Encode & Decode ======]

        /// <summary>
        /// Encodes the specified <paramref name="data"/> as a string. The default implementation
        /// converts the data into a base64-format.
        /// </summary>
        /// <param name="data">The data to encode.</param>
        /// <returns>A string-representation of the specified <paramref name="data"/>.</returns>
        protected virtual string Encode(byte[] data) =>
            System.Convert.ToBase64String(data);

        private byte[] DecodeSafe(string value)
        {
            try
            {
                return Decode(value);
            }
            catch (Exception exception)
            {
                throw NewDecodeFailedException(value, exception);
            }
        }

        /// <summary>
        /// Decodes the specified <paramref name="value"/> to its raw byte-representation. The default
        /// implementation expects <paramref name="value"/> to be a base64 encoded string.
        /// </summary>
        /// <param name="value">The value to decode.</param>
        /// <returns>The raw byte-representation of the <paramref name="value"/>.</returns>
        protected virtual byte[] Decode(string value) =>
            System.Convert.FromBase64String(value);

        private static Exception NewDecodeFailedException(string value, Exception exception)
        {
            var messageFormat = ExceptionMessages.BinaryFormatterSerializer_DecodeFailed;
            var message = string.Format(messageFormat, value);
            return new SerializationException(message, exception);
        }

        #endregion
    }
}
