using System;
using System.Linq.Expressions;
using Kingo.BuildingBlocks.Constraints;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator{T}" /> that is implemented using constraints.
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
        public DataErrorInfo Validate(TMessage message)
        {
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException("message");
            }
            throw new NotImplementedException();
        }               
    }
}
