using System;
using System.Linq.Expressions;
using Kingo.BuildingBlocks.Constraints;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator{T}" /> that is implemented using constraints.
    /// </summary>    
    public class ConstraintValidator<TMessage> : IMessageValidator<TMessage>, IMemberConstraintSet<TMessage>
    {        
        private readonly MemberConstraintSet<TMessage> _memberConstraintSet;
        private readonly IFormatProvider _formatProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintValidator{T}" /> class.
        /// </summary>        
        /// <param name="formatProvider">Optional <see cref="IFormatProvider" /> to use when formatting error messages.</param>
        public ConstraintValidator(IFormatProvider formatProvider = null)
        {                      
            _memberConstraintSet = new MemberConstraintSet<TMessage>();
            _formatProvider = formatProvider;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _memberConstraintSet.ToString();
        }

        #region [====== VerifyThat ======]

        /// <inheritdoc />
        public IMemberConstraint<TMessage, TValue> VerifyThat<TValue>(Expression<Func<TMessage, TValue>> memberExpression)
        {
            return _memberConstraintSet.VerifyThat(memberExpression);
        }

        /// <inheritdoc /> 
        public IMemberConstraint<TMessage, TValue> VerifyThat<TValue>(Func<TMessage, TValue> valueFactory, string name)
        {
            return _memberConstraintSet.VerifyThat(valueFactory, name);
        }        

        #endregion        

        #region [====== Validate ======]

        /// <inheritdoc />
        public MessageErrorInfo Validate(TMessage message)
        {
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException("message");
            }
            var builder = CreateDataErrorInfoBuilder(_formatProvider);

            _memberConstraintSet.WriteErrorMessages(message, builder);

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
