using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using Kingo.DynamicMethods;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Provides a base-implementation of the <see cref="IMessage" /> interface.
    /// </summary>
    [Serializable]    
    public abstract class Message : DataTransferObject, IMessage
    {
        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return EqualsMethod.Invoke(this, obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeMethod.Invoke(this);
        }

        #endregion

        #region [====== Copy ======]

        private static readonly ConcurrentDictionary<Type, Func<object, object>> _CopyMethods = new ConcurrentDictionary<Type, Func<object, object>>();

        object ICloneable.Clone()
        {
            return Copy();
        }
        
        IMessage IMessage.Copy()
        {
            return Copy();
        }

        /// <summary>
        /// Creates and returns a copy of this message. The default implementation uses
        /// the <see cref="DataContractSerializer" /> to copy this instance.
        /// </summary>
        /// <returns>A copy of this message.</returns>
        public virtual Message Copy()
        {
            return (Message) _CopyMethods.GetOrAdd(GetType(), DetermineCopyMethod).Invoke(this);
        }           

        private static Func<object, object> DetermineCopyMethod(Type messageType)
        {
            if (HasAttribute(messageType, typeof(DataContractAttribute)))
            {
                return CopyWithDataContractSerializer;
            }
            if (HasAttribute(messageType, typeof(SerializableAttribute)))
            {
                return CopyWithBinaryFormatter;
            }
            return CopyNotSupported;
        }

        private static bool HasAttribute(Type messageType, Type attributeType)
        {
            return messageType.GetCustomAttributes(attributeType).Any();
        }

        private static object CopyWithDataContractSerializer(object instance)
        {
            using (var memoryStream = new MemoryStream())
            {
                var writer = XmlDictionaryWriter.CreateBinaryWriter(memoryStream);
                var reader = XmlDictionaryReader.CreateBinaryReader(memoryStream, XmlDictionaryReaderQuotas.Max);
                var serializer = new DataContractSerializer(instance.GetType());

                serializer.WriteObject(writer, instance);
                writer.Flush();
                memoryStream.Position = 0;

                return serializer.ReadObject(reader);
            }
        }

        private static object CopyWithBinaryFormatter(object instance)
        {
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, instance);
                memoryStream.Position = 0;
                return formatter.Deserialize(memoryStream);
            }
        }

        private static object CopyNotSupported(object instance)
        {
            var messageFormat = ExceptionMessages.Message_CopyNotSupported;
            var message = string.Format(messageFormat, instance.GetType());
            throw new NotSupportedException(message);
        }

        #endregion              

        #region [====== Attributes ======]

        private static readonly ConcurrentDictionary<Type, Attribute[]> _MessageAttributeCache;

        static Message()
        {
            _MessageAttributeCache = new ConcurrentDictionary<Type, Attribute[]>();
        }

        /// <summary>
        /// Attempts to retrieve a single attribute of type <typeparamref name="TStrategy"/> from a certain message.
        /// </summary>
        /// <typeparam name="TStrategy">Type of attribute to retrieve.</typeparam>
        /// <param name="message">Message to retrieve the attribute from.</param>
        /// <param name="attribute">
        /// When this method returns <c>true</c>, refers to the attribute that was retrieved;
        /// will be <c>null</c> otherwise.
        /// </param>
        /// <returns><c>true</c> if the attribute was retrieved; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Multiple attributes of type <typeparamref name="TStrategy"/> were found on the specified <paramref name="message"/>.
        /// </exception>
        public static bool TryGetStrategyFromAttribute<TStrategy>(object message, out TStrategy attribute) where TStrategy : class
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            var messageType = message.GetType();
            var attributes = SelectAttributesOfType<TStrategy>(messageType);

            try
            {
                return (attribute = attributes.SingleOrDefault()) != null;
            }
            catch (InvalidOperationException)
            {
                throw NewAmbiguousAttributeMatchException(messageType, typeof(TStrategy));
            }
        }

        private static Exception NewAmbiguousAttributeMatchException(Type messageType, Type attributeType)
        {
            var messageFormat = ExceptionMessages.Message_AmbiguousAttributeMatch;
            var message = string.Format(messageFormat, messageType, attributeType);
            return new AmbiguousMatchException(message);
        }

        /// <summary>
        /// Returns the collections of <see cref="Attribute">Attributes</see> that are declared on the specified <paramref name="message"/>
        /// and are assignable to <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attributes to select.</typeparam>
        /// <param name="message">The message on which the attributes are declared.</param>
        /// <returns>A collection of <typeparamref name="TAttribute"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<TAttribute> SelectAttributesOfType<TAttribute>(object message) where TAttribute : class
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return SelectAttributesOfType<TAttribute>(message.GetType());
        }

        /// <summary>
        /// Returns the collections of <see cref="Attribute">Attributes</see> that are declared on the specified <paramref name="messageType"/>
        /// and are assignable to <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attributes to select.</typeparam>
        /// <param name="messageType">The <see cref="Type" /> on which the attributes are declared.</param>
        /// <returns>A collection of <typeparamref name="TAttribute"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageType"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<TAttribute> SelectAttributesOfType<TAttribute>(Type messageType) where TAttribute : class
        {
            if (messageType == null)
            {
                throw new ArgumentNullException(nameof(messageType));
            }
            return from attribute in _MessageAttributeCache.GetOrAdd(messageType, GetDeclaredAttributesOn)
                   let targetAttribute = attribute as TAttribute
                   where targetAttribute != null
                   select targetAttribute;
        }

        private static Attribute[] GetDeclaredAttributesOn(Type messageType)
        {
            return messageType.GetCustomAttributes(typeof(Attribute), true).Cast<Attribute>().ToArray();
        }

        #endregion
    }
}
