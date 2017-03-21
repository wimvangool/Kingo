using System;
using System.Collections.Concurrent;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Represents an implementation of the <see cref="IMessage.Validate" /> method which can be built up dynamically.
    /// </summary>
    public sealed class ValidateMethod
    {
        #region [====== Implementation ======]

        private abstract class Implementation
        {
            public virtual Implementation Add(Implementation implementation) =>
                new ImplementationPair(this, implementation);

            public abstract ErrorInfo Invoke(bool haltOnFirstError);
        }

        private sealed class ImplementationPair : Implementation
        {
            private readonly Implementation _left;
            private readonly Implementation _right;

            public ImplementationPair(Implementation left, Implementation right)
            {
                _left = left;
                _right = right;
            }

            public override ErrorInfo Invoke(bool haltOnFirstError)
            {
                var errorInfo = _left.Invoke(haltOnFirstError);
                if (errorInfo.HasErrors && haltOnFirstError)
                {
                    return errorInfo;
                }
                return ErrorInfo.Merge(errorInfo, _right.Invoke(haltOnFirstError));
            }
        }

        private sealed class NullImplementation : Implementation
        {
            public override Implementation Add(Implementation implementation) =>
                implementation;

            public override ErrorInfo Invoke(bool haltOnFirstError) =>
                ErrorInfo.Empty;
        }

        private sealed class Implementation<TMessage> : Implementation where TMessage : class
        {
            private readonly TMessage _message;
            private readonly IMessageValidator<TMessage> _validator;

            public Implementation(TMessage message, IMessageValidator<TMessage> validator)
            {
                if (ReferenceEquals(message, null))
                {
                    throw new ArgumentNullException(nameof(message));
                }
                if (ReferenceEquals(validator, null))
                {
                    throw new ArgumentNullException(nameof(validator));
                }
                _message = message;
                _validator = validator;
            }

            public override ErrorInfo Invoke(bool haltOnFirstError) =>
                _validator.Validate(_message, haltOnFirstError);
        }

        #endregion

        #region [====== Instance Members ======]

        private readonly Implementation _implementation;

        private ValidateMethod(Implementation implementation)
        {
            _implementation = implementation;
        }

        /// <summary>
        /// Adds a new validator to this implementation by either invoking the specified <paramref name="validatorFactory"/> or
        /// by retrieving it from cache, depending on the <paramref name="cacheValidator"/> argument.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to validate.</typeparam>
        /// <param name="message">The message to validate.</param>
        /// <param name="validatorFactory">
        /// The delegate to use to create the validator to add. This delegate is not used if <paramref name="cacheValidator"/> is <c>true</c>
        /// and the validator could be retrieved from cache.
        /// </param>
        /// <param name="cacheValidator">
        /// Indicates whether or not the cache should be used to either retrieve or store the validator.
        /// </param>
        /// <returns>The new implementation where the specified validator has been added.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="validatorFactory"/> is <c>null</c>.
        /// </exception>
        public ValidateMethod Add<TMessage>(TMessage message, Func<IMessageValidator<TMessage>> validatorFactory, bool cacheValidator = false) where TMessage : class
        {
            if (validatorFactory == null)
            {
                throw new ArgumentNullException(nameof(validatorFactory));
            }
            if (cacheValidator)
            {
                return Add(message, GetOrAddValidator(validatorFactory));
            }
            return Add(message, validatorFactory.Invoke());
        }

        /// <summary>
        /// Adds the specified <paramref name="validator" /> for the specified <paramref name="message"/> to this implemention.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to validate.</typeparam>
        /// <param name="message">The message to validate.</param>
        /// <param name="validator">The validator to use.</param>
        /// <returns>The new implementation where the specified validator has been added.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="validator"/> is <c>null</c>.
        /// </exception>
        public ValidateMethod Add<TMessage>(TMessage message, IMessageValidator<TMessage> validator) where TMessage : class =>
            new ValidateMethod(_implementation.Add(new Implementation<TMessage>(message, validator)));

        internal ErrorInfo Invoke(bool haltOnFirstError) =>
            _implementation.Invoke(haltOnFirstError);

        #endregion

        #region [====== Static Members ======]

        internal static readonly ValidateMethod Empty = new ValidateMethod(new NullImplementation());

        private static readonly ConcurrentDictionary<Type, object> _Validators = new ConcurrentDictionary<Type, object>();

        private static IMessageValidator<TMessage> GetOrAddValidator<TMessage>(Func<IMessageValidator<TMessage>> validatorFactory) =>
            (IMessageValidator<TMessage>) _Validators.GetOrAdd(typeof(TMessage), type => validatorFactory.Invoke());

        #endregion
    }
}
