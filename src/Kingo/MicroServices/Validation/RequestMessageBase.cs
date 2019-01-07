using System;

namespace Kingo.MicroServices.Validation
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
            CreateMessageValidator().Validate(this, haltOnFirstError);

        /// <summary>
        /// Creates and returns the validator for this message.
        /// </summary>        
        /// <returns>The validator of this message.</returns>
        protected virtual IRequestMessageValidator CreateMessageValidator() =>
            new NullValidator();

        #endregion

        /// <inheritdoc />
        public override string ToString() =>
            GetType().FriendlyName();
    }
}
