using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents an attribute that can be applied on message-types to configure the <see cref="MicroServices.MessageKind"/>
    /// and an optional message-id format that can used to generate a message-specific <see cref="IMessage.Id"/>
    /// at run-time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class MessageAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageAttribute" /> class.
        /// </summary>
        /// <param name="messageKind">Indicates what <see cref="MicroServices.MessageKind">kind</see> of message this is.</param>
        /// <param name="messageIdFormat">
        /// If specified, represents a format string that will be used to generate the <see cref="IMessage.Id"/>
        /// of the message at run-time.
        /// </param>
        /// <param name="messageIdProperties">
        /// A collection of property-names that will be used to insert the required values into the
        /// specified <paramref name="messageIdFormat"/> when formatting the <see cref="IMessage.Id" />.
        /// </param>
        public MessageAttribute(MessageKind messageKind, string messageIdFormat = null, params string[] messageIdProperties)
        {
            MessageKind = messageKind;
            MessageIdFormat = messageIdFormat;
            MessageIdProperties = messageIdProperties;
        }

        #region [====== MessageKind ======]

        /// <summary>
        /// Indicates what <see cref="MicroServices.MessageKind">kind</see> of message the associated message is.
        /// </summary>
        public MessageKind MessageKind
        {
            get;
        }

        #endregion

        #region [====== MessageId ======]

        /// <summary>
        /// If specified, represents a format string that will be used to generate the <see cref="IMessage.Id"/>
        /// of the message at run-time.
        /// </summary>
        public string MessageIdFormat
        {
            get;
        }

        /// <summary>
        /// A collection of property-names that will be used to insert the required values into the
        /// specified <see cref="MessageIdFormat"/> when formatting the <see cref="IMessage.Id" />.
        /// </summary>
        public IReadOnlyList<string> MessageIdProperties
        {
            get;
        }

        #endregion

        #region [====== TryGetAttribute ======]

        private static readonly ConcurrentDictionary<Type, MessageAttribute> _Attributes = new ConcurrentDictionary<Type, MessageAttribute>();

        /// <summary>
        /// Attempts to obtain the <see cref="MessageAttribute" /> declared on the specified <paramref name="contentType"/>.
        /// </summary>
        /// <param name="contentType">Type of the content of a message.</param>
        /// <param name="inherit">Indicates if inherited attributes may also be returned.</param>
        /// <param name="attribute">
        /// If an attribute was declared, this parameter will be set to the attribute that was found; otherwise <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if an attribute was declared; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contentType"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetMessageAttribute(Type contentType, bool inherit, out MessageAttribute attribute) =>
            (attribute = _Attributes.GetOrAdd(IsNotNull(contentType, nameof(contentType)), type => GetMessageAttribute(type, inherit))) != null;

        private static MessageAttribute GetMessageAttribute(Type contentType, bool inherit) =>
            contentType.GetCustomAttributes(typeof(MessageAttribute), inherit).Cast<MessageAttribute>().SingleOrDefault();

        #endregion
    }
}
