using System;

namespace Kingo.Messaging.Constraints
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator{T}" /> that is implemented using constraints.
    /// </summary>    
    public class ConstraintMessageValidator<T> : MemberConstraintSet<T>, IMessageValidator<T> where T : class
    {                
        private readonly IFormatProvider _formatProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintMessageValidator{T}" /> class.
        /// </summary>        
        /// <param name="haltOnFirstError">
        /// Indicates whether or not this constraint set should stop evaluating constraints once a constraint has failed.
        /// </param>
        /// <param name="formatProvider">Optional <see cref="IFormatProvider" /> to use when formatting error messages.</param>
        public ConstraintMessageValidator(bool haltOnFirstError = false, IFormatProvider formatProvider = null)
            : base(haltOnFirstError)
        {                                  
            _formatProvider = formatProvider;
        }                     

        #region [====== Validate ======]

        ErrorInfo IMessageValidator.Validate(object message)
        {
            return Validate(message as T);
        }

        /// <inheritdoc />
        public ErrorInfo Validate(T instance)
        {
            if (instance == null)
            {
                return ErrorInfo.Empty;
            }
            var builder = CreateErrorInfoBuilder(_formatProvider);

            WriteErrorMessages(instance, builder);

            return builder.BuildErrorInfo();
        }     
        
        /// <summary>
        /// Creates and returns a new <see cref="ErrorInfoBuilder" /> that will be used to collect all error messages during validation.
        /// </summary>
        /// <param name="formatProvider">
        /// The format provider that is used to format all error messages.
        /// </param>
        /// <returns>A new <see cref="ErrorInfoBuilder" />.</returns>
        protected virtual ErrorInfoBuilder CreateErrorInfoBuilder(IFormatProvider formatProvider)
        {
            return new ErrorInfoBuilder(formatProvider);
        }

        #endregion        
    
        #region [====== MergeWith ======]

        /// <inheritdoc />
        public IMessageValidator MergeWith(IMessageValidator validator, bool haltOnFirstError = false)
        {
            return CompositeValidator.Merge(this, validator, haltOnFirstError);
        }

        #endregion
    }
}
