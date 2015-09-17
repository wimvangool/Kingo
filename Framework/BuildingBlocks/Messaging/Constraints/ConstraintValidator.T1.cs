using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator{T}" /> in which validation-errors are reported
    /// through a <see cref="DataErrorInfoBuilder" />.
    /// </summary>    
    public sealed class ConstraintValidator<TMessage> : IMessageValidator<TMessage>, IMemberConstraintSet<TMessage>
    {        
        private readonly MemberConstraintSet<TMessage> _memberConstraintSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintValidator{T}" /> class.
        /// </summary>        
        public ConstraintValidator()
        {                      
            _memberConstraintSet = new MemberConstraintSet<TMessage>();
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

        /// <inheritdoc />
        public IReadOnlyList<DataErrorInfo> Validate(TMessage message)
        {
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException("message");
            }
            var errorTreeBuilder = new DataErrorInfoBuilder();

            if (_memberConstraintSet.HasErrors(message, errorTreeBuilder))
            {
                return new [] { errorTreeBuilder.BuildErrorTree() };
            }
            return DataErrorInfo.EmptyList;
        }               
    }
}
