using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents an operation that is being performed by a <see cref="MicroProcessor" />.
    /// </summary>
    public sealed class MicroProcessorOperation : IAttributeProvider<Type>
    {
        #region [====== NullAttributeProvider ======]

        private sealed class NullAttributeProvider : IAttributeProvider<Type>
        {
            public Type Target =>
                null;

            public bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
            {
                attribute = null;
                return false;
            }


            public IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() where TAttribute : class =>
                Enumerable.Empty<TAttribute>();
        }

        #endregion

        private readonly IAttributeProvider<Type> _attributeProvider;

        internal MicroProcessorOperation(MicroProcessorOperationTypes type, object message) :
            this(type, message, null) { }

        private MicroProcessorOperation(MicroProcessorOperationTypes type, object message, MicroProcessorOperation previousOperation)
        {
            PreviousOperation = previousOperation;
            Type = type;
            Message = message;

            _attributeProvider = CreateAttributeProvider(message);
        }

        #region [====== Operation / StackTrace ======]

        /// <summary>
        /// Returns the previous operation of the current stack trace.
        /// </summary>
        public MicroProcessorOperation PreviousOperation
        {
            get;
        }

        internal MicroProcessorOperation Push(MicroProcessorOperationTypes type, object message) =>
            new MicroProcessorOperation(type, message, this);

        /// <summary>
        /// Returns the entire stack-trace of operations from start to current.
        /// </summary>
        /// <returns>A collection of operations in the order they were executed.</returns>
        public IEnumerable<MicroProcessorOperation> StackTrace()
        {
            if (PreviousOperation != null)
            {
                foreach (var operation in PreviousOperation.StackTrace())
                {
                    yield return operation;                        
                }
            }
            yield return this;
        }

        #endregion

        #region [====== IAttributeProvider<Type> ======]

        Type IAttributeProvider<Type>.Target =>
            MessageType;

        /// <summary>
        /// Returns the type of the message that is associated with this operation, or <c>null</c> if <see cref="Message"/> is <c>null</c>.
        /// </summary>
        public Type MessageType =>
            _attributeProvider.Target;

        /// <summary>
        /// Attempts to retrieve a single attribute of type <typeparamref name="TAttribute"/> defined on the associated <see cref="MessageType"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <param name="attribute">
        /// When this method returns <c>true</c>, this parameter will refer to the retrieved attribute;
        /// otherwise it will be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the attribute was retrieved; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Multiple attribute defined of the method are assignable to type <typeparamref name="TAttribute"/>.
        /// </exception>
        public bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class =>
            _attributeProvider.TryGetAttributeOfType(out attribute);

        /// <summary>
        /// Retrieves all attributes of type <typeparamref name="TAttribute"/> that are declared on the associated <see cref="MessageType"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <returns>A collection of attributes.</returns>
        public IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() where TAttribute : class =>
            _attributeProvider.GetAttributesOfType<TAttribute>();

        private static IAttributeProvider<Type> CreateAttributeProvider(object message)
        {
            if (message == null)
            {
                return new NullAttributeProvider();
            }
            return new TypeAttributeProvider(message.GetType());
        }

        #endregion

        /// <summary>
        /// Returns the type of this operation.
        /// </summary>
        public MicroProcessorOperationTypes Type
        {
            get;
        }        

        /// <summary>
        /// Returns the message associated with this operation.
        /// </summary>
        public object Message
        {
            get;
        }

        internal bool IsSupported(MicroProcessorOperationTypes supportedOperationTypes) =>
            supportedOperationTypes.HasFlag(Type);

        /// <inheritdoc />
        public override string ToString() =>
            $"[{Type}]" + (Message == null ? string.Empty : " " + Message.GetType().FriendlyName());        
    }
}
