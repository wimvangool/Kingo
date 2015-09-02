using System;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator" /> in which validation-errors are reported
    /// through a <see cref="DataErrorInfoBuilder" />.
    /// </summary>    
    public sealed class ConstraintValidator : IMessageValidator, IMemberConstraintSet
    {        
        private readonly MemberConstraintSet _memberConstraintSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintValidator" /> class.
        /// </summary>        
        public ConstraintValidator()
        {                      
            _memberConstraintSet = new MemberConstraintSet();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _memberConstraintSet.ToString();
        }

        #region [====== VerifyThat ======]

        /// <inheritdoc />
        public IMemberConstraint<TValue> VerifyThat<TValue>(Expression<Func<TValue>> memberExpression)
        {
            return _memberConstraintSet.VerifyThat(memberExpression);
        }

        /// <inheritdoc /> 
        public IMemberConstraint<TValue> VerifyThat<TValue>(Func<TValue> valueFactory, string name)
        {
            return _memberConstraintSet.VerifyThat(valueFactory, name);
        }

        /// <inheritdoc />
        public IMemberConstraint<TValue> VerifyThat<TValue>(TValue value, string name)
        {
            return _memberConstraintSet.VerifyThat(value, name);
        }

        #endregion        

        /// <inheritdoc />
        public DataErrorInfo Validate()
        {
            var errorTreeBuilder = new DataErrorInfoBuilder();

            _memberConstraintSet.AddErrorMessagesTo(errorTreeBuilder);

            return errorTreeBuilder.BuildErrorTree();
        }        
    }
}
