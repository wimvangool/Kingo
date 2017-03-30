using System;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator{T}" /> that is implemented using constraints.
    /// </summary>    
    public class ConstraintValidator<TMessage> : MemberConstraintSet<TMessage>, IMessageValidator<TMessage> where TMessage : class
    {                
        private readonly IFormatProvider _formatProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintValidator{TMessage}" /> class.
        /// </summary>                
        /// <param name="formatProvider">Optional <see cref="IFormatProvider" /> to use when formatting error messages.</param>
        public ConstraintValidator(IFormatProvider formatProvider = null)           
        {                                  
            _formatProvider = formatProvider;
        }                     

        #region [====== Validate ======]        

        /// <inheritdoc />
        public ErrorInfo Validate(TMessage message, bool haltOnFirstError = false)
        {
            if (message == null)
            {
                return ErrorInfo.Empty;
            }
            var builder = CreateErrorInfoBuilder(_formatProvider);

            WriteErrorMessages(message, builder, haltOnFirstError);

            return builder.BuildErrorInfo();
        }

        /// <summary>
        /// Creates and returns a new <see cref="ErrorInfoBuilder" /> that will be used to collect all error messages during validation.
        /// </summary>
        /// <param name="formatProvider">
        /// The format provider that is used to format all error messages.
        /// </param>
        /// <returns>A new <see cref="ErrorInfoBuilder" />.</returns>
        protected virtual ErrorInfoBuilder CreateErrorInfoBuilder(IFormatProvider formatProvider) =>
            new ErrorInfoBuilder(formatProvider);

        #endregion        
    }
}
