using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Kingo.DynamicMethods;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Provides a base-implementation of the <see cref="IMessage" /> interface.
    /// </summary>
    [Serializable]    
    public abstract class Message : IMessage
    {
        #region [====== IReadOnlyList<IMessage> ======]

        private const int _Count = 1;

        /// <inheritdoc />
        int IReadOnlyCollection<IMessage>.Count => _Count;

        /// <inheritdoc />
        IMessage IReadOnlyList<IMessage>.this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return this;
                }
                throw MessageStream.NewIndexOutOfRangeException(index, _Count);
            }
        }

        /// <inheritdoc />
        IEnumerator<IMessage> IEnumerable<IMessage>.GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return this;
        }

        #endregion

        #region [====== IMessageStream ======]

        private static readonly ConcurrentDictionary<Type, Action<Message, IMessageHandler>> _HandleMethods = new ConcurrentDictionary<Type, Action<Message, IMessageHandler>>();
        private static readonly Lazy<MethodInfo> _HandleMethodDefinition = new Lazy<MethodInfo>(FindHandleMethodDefinition, true);

        private static MethodInfo FindHandleMethodDefinition()
        {
            var methods =
                from method in typeof(IMessageHandler).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                where method.IsGenericMethodDefinition && method.Name == nameof(IMessageHandler.Handle)
                select method;

            return methods.Single();
        }

        IMessageStream IMessageStream.Append(IMessageStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (stream.Count == 0)
            {
                return this;
            }
            return new MessageStream(this, stream);
        }        
        
        void IMessageStream.Accept(IMessageHandler handler)
        {
            if (handler != null)
            {
                Accept(handler);
            }
        }

        /// <summary>
        /// Lets the specified <paramref name="handler"/> handle all messages of this stream.
        /// </summary>
        /// <param name="handler">A handler of messages.</param>  
        protected virtual void Accept(IMessageHandler handler)
        {
            _HandleMethods.GetOrAdd(GetType(), CreateHandleMethod).Invoke(this, handler);
        }

        private static Action<Message, IMessageHandler> CreateHandleMethod(Type messageType)
        {
            var messageParameter = Expression.Parameter(typeof(Message), "message");
            var handlerParameter = Expression.Parameter(typeof(IMessageHandler), "handler");

            var message = Expression.Convert(messageParameter, messageType);
            var handleMethod = _HandleMethodDefinition.Value.MakeGenericMethod(messageType);
            var handleMethodCall = Expression.Call(handlerParameter, handleMethod, message);

            return Expression.Lambda<Action<Message, IMessageHandler>>(handleMethodCall, messageParameter, handlerParameter).Compile();
        }

        #endregion

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

        #region [====== Validation ======]

        private static readonly ConcurrentDictionary<Type, IMessageValidator> _Validators = new ConcurrentDictionary<Type, IMessageValidator>();

        /// <inheritdoc />
        public ErrorInfo Validate()
        {
            return GetOrAddValidator(CreateValidator).Validate(this);
        }

        internal IMessageValidator GetOrAddValidator(Func<IMessageValidator> validatorFactory)
        {
            return _Validators.GetOrAdd(GetType(), type => validatorFactory.Invoke());
        }

        /// <summary>
        /// Creates and returns a <see cref="IMessageValidator" /> that can be used to validate this message.        
        /// </summary>
        /// <returns>A new <see cref="IMessageValidator" /> that can be used to validate this message.</returns>
        protected virtual IMessageValidator CreateValidator()
        {
            return new NullValidator();
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
