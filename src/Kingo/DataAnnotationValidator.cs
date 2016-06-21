using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kingo
{
    /// <summary>
    /// Represents a <see cref="IValidator{T}" /> that validates an instance through
    /// all <see cref="ValidationAttribute">ValidationAttributes</see> that have been declared on the
    /// members of an instance.
    /// </summary>    
    public class DataAnnotationValidator : IValidator
    {
        private readonly IFormatProvider _formatProvider;
        private readonly Func<object, ValidationContext> _validationContextFactory;        

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAnnotationValidator" /> class.
        /// </summary>  
        /// <param name="formatProvider">
        /// Optional <see cref="IFormatProvider" /> to use when formatting error messages.
        /// </param>              
        /// <param name="validationContextFactory">
        /// The method used to create a <see cref="ValidationContext" /> for a specific instance. Specify <c>null</c>
        /// to use the default factory.
        /// </param>             
        public DataAnnotationValidator(IFormatProvider formatProvider = null, Func<object, ValidationContext> validationContextFactory = null)
        {
            _formatProvider = formatProvider;     
            _validationContextFactory = validationContextFactory ?? (instance => new ValidationContext(instance, null, null));
        }

        /// <summary>
        /// The <see cref="IFormatProvider" /> to use when formatting error messages.
        /// </summary>
        public IFormatProvider FormatProvider
        {
            get { return _formatProvider; }
        }

        #region [====== Validate ======]

        /// <inheritdoc />
        public ErrorInfo Validate(object instance)
        {
            if (instance == null)
            {
                return ErrorInfo.Empty;
            }
            var errorInfo = Validate(_validationContextFactory.Invoke(instance));
            if (errorInfo.HasErrors)
            {
                return ErrorInfo.Empty;
            }
            return errorInfo;
        }

        private ErrorInfo Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(validationContext.ObjectInstance, validationContext, validationResults, true);
            if (isValid)
            {
                return ErrorInfo.Empty;
            }
            return BuildErrorInfo(CreateErrorInfoBuilder(FormatProvider), validationResults);
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

        private static ErrorInfo BuildErrorInfo(ErrorInfoBuilder errorInfoBuilder, IEnumerable<ValidationResult> validationResults)
        {            
            foreach (var result in validationResults)
            foreach (var member in result.MemberNames)    
            {
                errorInfoBuilder.Add(result.ErrorMessage, member);
            }
            return errorInfoBuilder.BuildErrorInfo();
        }        

        #endregion

        #region [====== MergeWith ======]

        /// <inheritdoc />
        public IValidator MergeWith(IValidator validator, bool haltOnFirstError = false)
        {
            return CompositeValidator.Merge(this, validator, haltOnFirstError);
        }

        #endregion
    }
}
