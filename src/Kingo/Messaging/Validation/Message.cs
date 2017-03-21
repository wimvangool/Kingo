using System;
using Kingo.DynamicMethods;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Provides a base-implementation of the <see cref="IMessage" /> interface.
    /// </summary>
    [Serializable]    
    public abstract class Message : IMessage
    {        
        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            EqualsMethod.Invoke(this, obj);

        /// <inheritdoc />
        public override int GetHashCode() =>
            GetHashCodeMethod.Invoke(this);

        #endregion

        #region [====== Validation ======]                     

        /// <inheritdoc />
        public ErrorInfo Validate(bool haltOnFirstError = false) =>
            Implement(ValidateMethod.Empty).Invoke(haltOnFirstError);

        /// <summary>
        /// Implements the specified <paramref name="method"/> and returns this implementation by adding
        /// all appropriate <see cref="IMessageValidator{T}">validators</see>.
        /// </summary>
        /// <param name="method">The method to implement.</param>
        /// <returns>The implemented method.</returns>
        protected virtual ValidateMethod Implement(ValidateMethod method) =>
            method;

        #endregion              
    }
}
