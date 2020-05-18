using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Kingo.Serialization
{
    /// <summary>
    /// Represents a <see cref="ISerializer" /> that uses XML as a serialization format.
    /// </summary>
    public class XmlFormatSerializer : TextFormatSerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFormatSerializer" /> class.
        /// </summary>
        /// <param name="encoder">Optional encoder that will be used by the serializer to convert string-values to and from byte-arrays.</param>
        public XmlFormatSerializer(ITextEncoder encoder = null) :
            base(encoder) { }

        #region [====== Serialize ======]

        /// <inheritdoc />
        protected override string SerializeToString(object content)
        {
            var value = new StringBuilder();

            using (var writer = CreateXmlWriter(value))
            {
                WriteObject(writer, content);
            }
            return value.ToString();
        }

        /// <summary>
        /// Creates and returns a new <see cref="XmlWriter"/> that will write all xml-data
        /// to the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> to which all xml-data is to be written.</param>
        /// <returns>A new <see cref="XmlWriter"/>.</returns>
        protected virtual XmlWriter CreateXmlWriter(StringBuilder value) =>
            XmlWriter.Create(value);

        private void WriteObject(XmlWriter writer, object content) =>
            CreateXmlSerializer(content.GetType()).WriteObject(writer, content);

        #endregion

        #region [====== Deserialize ======]

        /// <inheritdoc />
        protected override object DeserializeFromString(string content, Type contentType)
        {
            using (var reader = CreateXmlReader(content))
            {
                return ReadObject(reader, contentType);
            }
        }

        /// <summary>
        /// Creates and returns a new <see cref="XmlReader"/> that will read all xml-data
        /// from the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The xml-data in string format.</param>
        /// <returns>A new <see cref="XmlReader"/>.</returns>
        protected virtual XmlReader CreateXmlReader(string value) =>
            XmlReader.Create(new StringReader(value));

        private object ReadObject(XmlReader reader, Type type) =>
            CreateXmlSerializer(type).ReadObject(reader);

        #endregion

        /// <summary>
        /// Creates and returns a new <see cref="XmlObjectSerializer" /> that can serialize
        /// and deserialize objects using a specific xml-format.
        /// </summary>
        /// <param name="type">Type of the object that will be serialized or deserialized.</param>
        /// <returns>A new <see cref="XmlObjectSerializer"/>.</returns>
        protected virtual XmlObjectSerializer CreateXmlSerializer(Type type) =>
            new DataContractSerializer(type);
    }
}
