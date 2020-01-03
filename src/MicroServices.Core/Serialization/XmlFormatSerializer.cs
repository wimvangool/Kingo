using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Kingo.Reflection;

namespace Kingo.Serialization
{
    /// <summary>
    /// Represents a <see cref="ISerializer" /> that uses XML as a serialization format.
    /// </summary>
    public class XmlFormatSerializer : Serializer
    {
        #region [====== Serialize & Deserialize ======]

        /// <inheritdoc />
        public override string Serialize(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            var value = new StringBuilder();

            using (var writer = CreateXmlWriter(value))
            {
                WriteObject(writer, instance);
            }
            return value.ToString();
        }

        private void WriteObject(XmlWriter writer, object instance)
        {
            try
            {
                CreateXmlSerializer(instance.GetType()).WriteObject(writer, instance);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw NewSerializationFailedException(instance.GetType(), exception);
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
            using (var reader = CreateXmlReader(value))
            {
                return ReadObject(reader, type);
            }
        }

        private object ReadObject(XmlReader reader, Type type)
        {
            try
            {
                return CreateXmlSerializer(type).ReadObject(reader);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw NewDeserializationFailedException(type, exception);
            }
        }

        /// <summary>
        /// Creates and returns a new <see cref="XmlWriter"/> that will write all xml-data
        /// to the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> to which all xml-data is to be written.</param>
        /// <returns>A new <see cref="XmlWriter"/>.</returns>
        protected virtual XmlWriter CreateXmlWriter(StringBuilder value) =>
            XmlWriter.Create(value);

        /// <summary>
        /// Creates and returns a new <see cref="XmlReader"/> that will read all xml-data
        /// from the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The xml-data in string format.</param>
        /// <returns>A new <see cref="XmlReader"/>.</returns>
        protected virtual XmlReader CreateXmlReader(string value) =>
            XmlReader.Create(new StringReader(value));

        /// <summary>
        /// Creates and returns a new <see cref="XmlObjectSerializer" /> that can serialize
        /// and deserialize objects using a specific xml-format.
        /// </summary>
        /// <param name="type">Type of the object that will be serialized or deserialized.</param>
        /// <returns>A new <see cref="XmlObjectSerializer"/>.</returns>
        protected virtual XmlObjectSerializer CreateXmlSerializer(Type type) =>
            new DataContractSerializer(type);

        private static Exception NewSerializationFailedException(Type type, Exception exception)
        {
            var messageFormat = ExceptionMessages.XmlFormatSerializer_SerializationFailed;
            var message = string.Format(messageFormat, type.FriendlyName());
            return new SerializationException(message, exception);
        }

        private static Exception NewDeserializationFailedException(Type type, Exception exception)
        {
            var messageFormat = ExceptionMessages.XmlFormatSerializer_DeserializationFailed;
            var message = string.Format(messageFormat, type.FriendlyName());
            return new SerializationException(message, exception);
        }

        #endregion
    }
}
