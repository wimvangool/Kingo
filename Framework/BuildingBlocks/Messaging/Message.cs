using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// Provides a base-implementation of the <see cref="IMessage" /> interface.
    /// </summary>
    [Serializable]
    [DataContract]
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

        #region [====== ExtensibleObject ======]

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ExtensionDataObject IExtensibleDataObject.ExtensionData
        {
            get { return _extensionData; }
            set { _extensionData = value; }
        }

        #endregion

        #region [====== Copy ======]

        object ICloneable.Clone()
        {
            return CopyMessage();
        }

        IMessage IMessage.Copy()
        {
            return CopyMessage();
        }

        internal abstract IMessage CopyMessage();       

        #endregion      

        #region [====== Validation ======]

        private static readonly ConcurrentDictionary<Type, object> _Validators = new ConcurrentDictionary<Type, object>();
            
        ErrorInfo IValidateable.Validate()
        {
            return ValidateMessage();
        }

        public ErrorInfo Validate()
        {
            return ValidateMessage();
        }

        internal abstract ErrorInfo ValidateMessage();

        internal IValidator<TMessage> GetOrAddValidator<TMessage>(Func<IValidator<TMessage>> validatorFactory)
        {
            return _Validators.GetOrAdd(GetType(), validatorFactory) as IValidator<TMessage>;
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
                throw new ArgumentNullException("message");
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
