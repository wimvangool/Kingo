using System;
using System.Collections.Concurrent;

namespace Kingo
{
    /// <summary>
    /// Represents an object which main purpose is to carry data.
    /// </summary>
    [Serializable]
    public abstract class DataTransferObject : IDataTransferObject
    {
        #region [====== Validation ======]

        private static readonly ConcurrentDictionary<Type, IValidator> _Validators = new ConcurrentDictionary<Type, IValidator>();

        /// <inheritdoc />
        public ErrorInfo Validate()
        {
            return GetOrAddValidator(CreateValidator).Validate(this);
        }

        internal IValidator GetOrAddValidator(Func<IValidator> validatorFactory)
        {
            return _Validators.GetOrAdd(GetType(), type => validatorFactory.Invoke());
        }

        /// <summary>
        /// Creates and returns a <see cref="IValidator" /> that can be used to validate this message.        
        /// </summary>
        /// <returns>A new <see cref="IValidator" /> that can be used to validate this message.</returns>
        protected virtual IValidator CreateValidator()
        {
            return new NullValidator();
        }

        #endregion
    }
}
