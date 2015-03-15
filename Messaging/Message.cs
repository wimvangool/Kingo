using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides a base-implementation of the <see cref="IMessage" /> interface.
    /// </summary>
    [Serializable]
    public abstract class Message : IMessage, IExtensibleDataObject
    {
        private ExtensionDataObject _extensionData;

        internal Message() { }

        internal Message(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _extensionData = message._extensionData;
        }

        #region [====== TypeId ======]

        string IMessage.TypeId
        {
            get { return TypeIdOf(GetType()); }
        }

        /// <summary>
        /// Returns the type-id of the specified <paramref name="messageType"/>.
        /// </summary>
        /// <param name="messageType">Type of a message.</param>
        /// <returns>The type-id of the specified <paramref name="messageType"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageType"/> is <c>null</c>.
        /// </exception>
        public static string TypeIdOf(Type messageType)
        {
            if (messageType == null)
            {
                throw new ArgumentNullException("messageType");
            }
            return messageType.Name;
        }

        #endregion

        #region [====== ExtensibleObject ======]

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ExtensionDataObject IExtensibleDataObject.ExtensionData
        {
            get { return _extensionData; }
            set { _extensionData = value; }
        }

        #endregion

        #region [====== Copy ======]

        IMessage IMessage.Copy()
        {
            return CopyMessage();
        }

        internal abstract IMessage CopyMessage();

        #endregion

        #region [====== Equals & GetHashCode ======]

        /// <summary>
        /// Determines whether two strings are equal or not.
        /// </summary>
        /// <param name="left">The left string.</param>
        /// <param name="right">The right string.</param>
        /// <returns><c>true</c>if the two specified strings are equal; otherwise <c>false</c>.</returns>
        public static bool Equals(string left, string right)
        {
            return left == right;
        }

        /// <summary>
        /// Determines whether two structures are equal or not.
        /// </summary>
        /// <typeparam name="T">Type of the structures to compare.</typeparam>
        /// <param name="left">The left structure.</param>
        /// <param name="right">The right structure.</param>
        /// <returns><c>true</c> if the specified structures are equal; otherwise <c>false</c>.</returns>
        public static bool Equals<T>(T left, T right) where T : struct, IEquatable<T>
        {
            return left.Equals(right);
        }        

        /// <summary>
        /// Returns the hashcode of the specified instance, or <c>0</c> if <paramref name="a"/> is <c>null</c>.
        /// </summary>
        /// <param name="a">The instance to get the hashcode for.</param>
        /// <returns>The hashcode of the specified instance.</returns>
        public static int GetHashCodeOf(object a)
        {
            return a == null ? 0 : a.GetHashCode();
        }

        /// <summary>
        /// Returns the combined hashcode of the specified instances using a bitwise XOR operation.
        /// </summary>
        /// <param name="a">A certain instance.</param>
        /// <param name="b">Another instance.</param>
        /// <returns>A combined hashcode of the specified instances.</returns>
        public static int GetHashCodeOf(object a, object b)
        {
            return GetHashCodeOf(a) ^ GetHashCodeOf(b);
        }

        /// <summary>
        /// Returns the combined hashcode of the specified instances using a bitwise XOR operation.
        /// </summary>
        /// <param name="a">A certain instance.</param>
        /// <param name="b">Another instance.</param>
        /// <param name="c">Yet another instance.</param>
        /// <returns>A combined hashcode of the specified instances.</returns>
        public static int GetHashCodeOf(object a, object b, object c)
        {
            return GetHashCodeOf(a) ^ GetHashCodeOf(b) ^ GetHashCodeOf(c);
        }

        /// <summary>
        /// Returns the combined hashcode of the specified instances using a bitwise XOR operation.
        /// </summary>
        /// <param name="instances">A collection of instances.</param>
        /// <returns>A combined hashcode of the specified instances.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instances"/> is <c>null</c>.
        /// </exception>
        public static int GetHashCodeOf(params object[] instances)
        {
            if (instances == null)
            {
                throw new ArgumentNullException("instances");
            }
            var hashCode = 0;

            for (int index = 0; index < instances.Length; index++)
            {
                hashCode ^= GetHashCodeOf(instances[index]);
            }
            return hashCode;
        }

        #endregion

        #region [====== Validation ======]        

        /// <inheritdoc />
        public ValidationErrorTree Validate()
        {
            var validationStrategy = CreateValidationStrategy();
            if (validationStrategy == null)
            {
                return ValidationErrorTree.NoErrors(this);
            }
            return validationStrategy.Validate();
        }                

        /// <summary>
        /// Creates and returns a <see cref="IMessageValidator" /> that can be used to validate this message,
        /// or <c>null</c> if this message does not require validation.
        /// </summary>
        /// <returns>A new <see cref="IMessageValidator" /> that can be used to validate this message.</returns>
        protected virtual IMessageValidator CreateValidationStrategy()
        {
            return null;
        }

        #endregion

        #region [====== Attributes ======]

        private static readonly ConcurrentDictionary<Type, Attribute[]> _MessageAttributeCache;

        static Message()
        {
            _MessageAttributeCache = new ConcurrentDictionary<Type, Attribute[]>();
        }

        internal static bool TryGetStrategyFromAttribute<TStrategy>(object message, out TStrategy atribute) where TStrategy : class
        {
            var messageType = message.GetType();
            var attributes = SelectAttributesOfType<TStrategy>(messageType);

            try
            {
                return (atribute = attributes.SingleOrDefault()) != null;
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
                throw new ArgumentNullException("message");
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
                throw new ArgumentNullException("messageType");
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
