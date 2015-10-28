using System;
using Kingo.BuildingBlocks.Constraints;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator{T}" /> that is implemented using constraints.
    /// </summary>    
    public class ConstraintValidator<TMessage> : MemberConstraintSet<TMessage>, IMessageValidator<TMessage>
    {                
        private readonly IFormatProvider _formatProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintValidator{T}" /> class.
        /// </summary>        
        /// <param name="formatProvider">Optional <see cref="IFormatProvider" /> to use when formatting error messages.</param>
        public ConstraintValidator(IFormatProvider formatProvider = null)
        {                                  
            _formatProvider = formatProvider;
        }                     

        #region [====== Validate ======]

        /// <inheritdoc />
        public MessageErrorInfo Validate(TMessage message)
        {
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException("message");
            }
            var builder = CreateDataErrorInfoBuilder(_formatProvider);

            WriteErrorMessages(message, builder);

            return builder.BuildDataErrorInfo();
        }     
        
        /// <summary>
        /// Creates and returns a new <see cref="MessageErrorInfoBuilder" /> that will be used to collect all error messages during validation.
        /// </summary>
        /// <param name="formatProvider">
        /// The format provider that is used to format all error messages.
        /// </param>
        /// <returns>A new <see cref="MessageErrorInfoBuilder" />.</returns>
        protected virtual MessageErrorInfoBuilder CreateDataErrorInfoBuilder(IFormatProvider formatProvider)
        {
            return new MessageErrorInfoBuilder(formatProvider);
        }

        #endregion
    }
}
