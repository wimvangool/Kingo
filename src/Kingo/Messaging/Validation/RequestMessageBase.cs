using System;
using System.Collections.Concurrent;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Provides a base-implementation of the <see cref="IRequestMessage" /> interface.
    /// </summary>
    [Serializable]    
    public abstract class RequestMessageBase : IRequestMessage
    {                
        #region [====== Validation ======]                

        /// <inheritdoc />
        public ErrorInfo Validate(bool haltOnFirstError = false) =>
            _Validators.GetOrAdd(GetType(), type => CreateMessageValidator()).Validate(this, haltOnFirstError);

        /// <summary>
        /// Creates and returns the validator for this message.
        /// </summary>        
        /// <returns>The validator of this message.</returns>
        protected virtual IRequestMessageValidator CreateMessageValidator() =>
            new NullValidator();

        private static readonly ConcurrentDictionary<Type, IRequestMessageValidator> _Validators = new ConcurrentDictionary<Type, IRequestMessageValidator>();

        /// <summary>
        /// Attempts to retrieve the validator that has been registered for the specified <paramref name="messageType" />.
        /// </summary>
        /// <param name="messageType">Type of a message.</param>
        /// <param name="validator">
        /// If this method returns <c>true</c>, this parameter will refer to a validator that is capable of validating messages of
        /// the specified <paramref name="messageType"/>; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if a validator for the specified <paramref name="messageType"/> had been registered; otherwise <c>false</c>.
        /// </returns>
        public static bool TryGetMessageValidator(Type messageType, out IRequestMessageValidator validator) =>
            _Validators.TryGetValue(messageType, out validator);

        /// <summary>
        /// Registers the specified <paramref name="validationMethod"/> for the specified message type <typeparamref name="TMessage"/>.
        /// </summary>        
        /// <param name="validationMethod">The validation method to register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validationMethod"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Another validator for the specified message type <typeparamref name="TMessage" /> has already been registered.
        /// </exception>
        public static void Register<TMessage>(Func<TMessage, bool, ErrorInfo> validationMethod) =>
            Register(new DelegateValidator<TMessage>(validationMethod));

        /// <summary>
        /// Registers the specified <paramref name="validator"/> for the specified message type <typeparamref name="TMessage"/>.
        /// </summary>        
        /// <param name="validator">The validator to register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Another validator for the specified message type <typeparamref name="TMessage" /> has already been registered.
        /// </exception>
        public static void Register<TMessage>(IRequestMessageValidator<TMessage> validator) =>
            Register(typeof(TMessage), new RequestRequestMessageValidator<TMessage>(validator));

        private static void Register(Type messageType, IRequestMessageValidator validator)
        {
            if (_Validators.TryAdd(messageType, validator))
            {
                return;
            }
            throw NewValidatorAlreadyRegisteredException(messageType);
        }

        private static Exception NewValidatorAlreadyRegisteredException(Type messageType)
        {
            var messageFormat = ExceptionMessages.Message_ValidatorAlreadyRegisterd;
            var message = string.Format(messageFormat, messageType.FriendlyName());
            return new ArgumentException(message, nameof(messageType));
        }

        #endregion

        /// <inheritdoc />
        public override string ToString() =>
            GetType().FriendlyName();
    }
}
