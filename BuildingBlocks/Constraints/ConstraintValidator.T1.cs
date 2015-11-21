using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Represents a <see cref="IValidator{T}" /> that is implemented using constraints.
    /// </summary>    
    public class ConstraintValidator<T> : MemberConstraintSet<T>, IValidator<T>
    {                
        private readonly IFormatProvider _formatProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintValidator{T}" /> class.
        /// </summary>        
        /// <param name="haltOnFirstError">
        /// Indicates whether or not this constraint set should stop evaluating constraints once a constraint has failed.
        /// </param>
        /// <param name="formatProvider">Optional <see cref="IFormatProvider" /> to use when formatting error messages.</param>
        public ConstraintValidator(bool haltOnFirstError = false, IFormatProvider formatProvider = null)
            : base(haltOnFirstError)
        {                                  
            _formatProvider = formatProvider;
        }                     

        #region [====== Validate ======]

        /// <inheritdoc />
        public ErrorInfo Validate(T instance)
        {
            if (ReferenceEquals(instance, null))
            {
                throw new ArgumentNullException("instance");
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

        #region [====== Append ======]
        
        IValidator<T> IValidator<T>.Append(IValidator<T> validator, bool haltOnFirstError)
        {
            return CompositeValidator<T>.Append(this, validator, haltOnFirstError);
        }

        #endregion
    }
}
