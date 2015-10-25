using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static class GuidConstraints
    {
        #region [====== IsNotEmpty ======]

        /// <summary>
        /// Verifies that the value is not an empty <see cref="Guid" />.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<TMessage, Guid> IsNotEmpty<TMessage>(this IMemberConstraint<TMessage, Guid> member, string errorMessage = null)
        {
            return member.Apply(new IsNotEmptyGuidConstraint().WithErrorMessage(errorMessage));
        }              

        #endregion

        #region [====== IsEmpty ======]

        /// <summary>
        /// Verifies that the value is an empty <see cref="Guid" />.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<TMessage, Guid> IsEmpty<TMessage>(this IMemberConstraint<TMessage, Guid> member, string errorMessage = null)
        {
            return member.Apply(new IsEmptyGuidConstraint().WithErrorMessage(errorMessage));
        }              

        #endregion
    }

    #region [====== IsNotEmptyGuidConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a <see cref="Guid" /> is an empty guid..
    /// </summary>
    public sealed class IsNotEmptyGuidConstraint : Constraint<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotEmptyGuidConstraint" /> class.
        /// </summary>    
        public IsNotEmptyGuidConstraint() {}

        private IsNotEmptyGuidConstraint(IsNotEmptyGuidConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private IsNotEmptyGuidConstraint(IsNotEmptyGuidConstraint constraint, Identifier name)
            : base(constraint, name) {}

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.GuidConstraints_IsNotEmpty); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<Guid> WithName(Identifier name)
        {
            return new IsNotEmptyGuidConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<Guid> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsNotEmptyGuidConstraint(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<Guid> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(Guid value)
        {
            return !Guid.Empty.Equals(value);
        }

        #endregion
    }

    #endregion

    #region [====== IsEmptyGuidConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a <see cref="Guid" /> is an empty guid..
    /// </summary>
    public sealed class IsEmptyGuidConstraint : Constraint<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsEmptyGuidConstraint" /> class.
        /// </summary>    
        public IsEmptyGuidConstraint() { }

        private IsEmptyGuidConstraint(IsEmptyGuidConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private IsEmptyGuidConstraint(IsEmptyGuidConstraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.GuidConstraints_IsEmpty); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<Guid> WithName(Identifier name)
        {
            return new IsEmptyGuidConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<Guid> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsEmptyGuidConstraint(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<Guid> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsNotEmptyGuidConstraint().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(Guid value)
        {
            return Guid.Empty.Equals(value);
        }

        #endregion
    }

    #endregion
}
