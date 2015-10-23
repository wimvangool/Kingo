using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static class BooleanConstraints
    {
        #region [====== IsTrue ======]

        /// <summary>
        /// Verifies that the member's value is <c>true</c>.
        /// </summary>        
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, bool> IsTrue<TMessage>(this IMemberConstraint<TMessage, bool> member, string errorMessage = null)
        {
            return member.Apply(new IsTrueConstraint().WithErrorMessage(errorMessage));
        }        

        #endregion

        #region [====== IsFalse ======]

        /// <summary>
        /// Verifies that the member's value is <c>false</c>.
        /// </summary>        
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, bool> IsFalse<TMessage>(this IMemberConstraint<TMessage, bool> member, string errorMessage = null)
        {
            return member.Apply(new IsFalseConstraint().WithErrorMessage(errorMessage));
        }        

        #endregion
    }

    #region [====== IsTrueConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is <c>false</c>.
    /// </summary>
    public sealed class IsTrueConstraint : Constraint<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsTrueConstraint" /> class.
        /// </summary>        
        public IsTrueConstraint() { }

        private IsTrueConstraint(IsTrueConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private IsTrueConstraint(IsTrueConstraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BooleanConstraints_IsTrue); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> WithName(Identifier name)
        {
            return new IsTrueConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsTrueConstraint(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsFalseConstraint().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(bool value)
        {
            return value;
        }

        #endregion
    }

    #endregion

    #region [====== IsFalseConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is <c>false</c>.
    /// </summary>
    public sealed class IsFalseConstraint : Constraint<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsFalseConstraint" /> class.
        /// </summary>        
        public IsFalseConstraint() { }            

        private IsFalseConstraint(IsFalseConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private IsFalseConstraint(IsFalseConstraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BooleanConstraints_IsFalse); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> WithName(Identifier name)
        {
            return new IsFalseConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsFalseConstraint(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsTrueConstraint().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(bool value)
        {
            return !value;
        }

        #endregion
    }

    #endregion
}
