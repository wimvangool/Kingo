using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Kingo.Serialization
{
    /// <summary>
    /// Represents a <see cref="ISerializer" /> that serializes objects using a <see cref="BinaryFormatter" />.
    /// </summary>
    public class BinaryFormatSerializer : Serializer
    {
        private readonly Lazy<BinaryFormatter> _formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormatSerializer" /> class.
        /// </summary>
        public BinaryFormatSerializer()
        {
            _formatter = new Lazy<BinaryFormatter>(CreateFormatter);
        }

        private BinaryFormatter Formatter =>
            _formatter.Value;

        #region [====== Serialize ======]

        /// <inheritdoc />
        protected override byte[] SerializeContent(object content)
        {
            using (var stream = new MemoryStream())
            {
                Formatter.Serialize(stream, content);
                return stream.ToArray();
            }
        }

        #endregion

        #region [====== Deserialize ======]

        /// <inheritdoc />
        protected override object DeserializeContent(byte[] content, Type contentType)
        {
            using (var stream = new MemoryStream(content))
            {
                return Convert(Formatter.Deserialize(stream), contentType);
            }
        }

        private static object Convert(object content, Type contentType)
        {
            if (contentType.IsInstanceOfType(content))
            {
                return content;
            }
            return System.Convert.ChangeType(content, contentType);
        }

        /// <summary>
        /// Creates and returns the <see cref="BinaryFormatter" /> that will be used
        /// to serialize and deserialize objects.
        /// </summary>
        /// <returns>A new <see cref="BinaryFormatter"/>.</returns>
        protected virtual BinaryFormatter CreateFormatter() =>
            new BinaryFormatter();

        #endregion
    }
}
