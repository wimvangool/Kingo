using System.Linq.Expressions;

namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// Represent a nullable member that can be validated and produces an <see cref="ErrorMessage" /> if this validation fails.
    /// </summary>
    /// <typeparam name="TValue">Type of the member's value.</typeparam>
    public class NullableMember<TValue> : Member<TValue?> where TValue : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullableMember{TValue}" /> class.
        /// </summary>
        /// <param name="memberSet">The set this member belongs to.</param>       
        /// <param name="memberExpression">An expression that returns an instance of <typeparamref name="TValue"/>.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberSet"/> or <paramref name="memberExpression"/> is <c>null</c>.
        /// </exception>
        public NullableMember(MemberSet memberSet, Expression<Func<TValue?>> memberExpression)
            : base(memberSet, memberExpression) { }

        #region [====== IsNotNull ======]

        /// <summary>
        /// Verifies whether or not this member is not <c>null</c>.
        /// </summary>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>A member containing the value of this member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public new Member<TValue> IsNotNull(string errorMessage)
        {
            return ValueOfNullable(new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies whether or not this member is not <c>null</c>.
        /// </summary>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>A member containing the value of this member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public new Member<TValue> IsNotNull(string errorMessageFormat, object arg0)
        {
            return ValueOfNullable(new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies whether or not this member is not <c>null</c>.
        /// </summary>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>     
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>  
        /// <returns>A member containing the value of this member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public new Member<TValue> IsNotNull(string errorMessageFormat, object arg0, object arg1)
        {
            return ValueOfNullable(new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies whether or not this member is not <c>null</c>.
        /// </summary>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>A member containing the value of this member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public new Member<TValue> IsNotNull(string errorMessageFormat, params object[] arguments)
        {
            return ValueOfNullable(new ErrorMessage(errorMessageFormat, arguments));
        }
        
        private Member<TValue> ValueOfNullable(ErrorMessage errorMessage)
        {
            var constraint = IsNotNullConstraint(errorMessage);            
            var member = new Member<TValue>(MemberSet, Name, GetValue, constraint);

            MemberSet.Replace(this, member);

            return member;
        }

        private Constraint IsNotNullConstraint(ErrorMessage errorMessage)
        {
            return Constraint.And(this, value => value.HasValue, errorMessage, MemberSet.Consumer);
        }

        private TValue GetValue()
        {
            return Value.Value;
        }

        #endregion
    }
}
