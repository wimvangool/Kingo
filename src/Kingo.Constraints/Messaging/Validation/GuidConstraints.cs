using System;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{T}" />.
    /// </summary>
    public static class GuidConstraints
    {
        #region [====== IsNotEmpty ======]

        /// <summary>
        /// Verifies that the value is not an empty <see cref="Guid" />.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraintBuilder<T, Guid> IsNotEmpty<T>(this IMemberConstraintBuilder<T, Guid> member, string errorMessage = null) =>
             member.Apply(new GuidIsNotEmptyConstraint().WithErrorMessage(errorMessage));

        #endregion

        #region [====== IsEmpty ======]

        /// <summary>
        /// Verifies that the value is an empty <see cref="Guid" />.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraintBuilder<T, Guid> IsEmpty<T>(this IMemberConstraintBuilder<T, Guid> member, string errorMessage = null) =>
             member.Apply(new GuidIsEmptyConstraint().WithErrorMessage(errorMessage));

        #endregion
    }

    #region [====== GuidIsNotEmptyConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a <see cref="Guid" /> is an empty guid..
    /// </summary>
    public sealed class GuidIsNotEmptyConstraint : Constraint<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuidIsNotEmptyConstraint" /> class.
        /// </summary>    
        public GuidIsNotEmptyConstraint() {}

        private GuidIsNotEmptyConstraint(GuidIsNotEmptyConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private GuidIsNotEmptyConstraint(GuidIsNotEmptyConstraint constraint, Identifier name)
            : base(constraint, name) {}

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified =>
             StringTemplate.Parse(ErrorMessages.GuidConstraints_IsNotEmpty);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<Guid> WithName(Identifier name) =>
             new GuidIsNotEmptyConstraint(this, name);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<Guid> WithErrorMessage(StringTemplate errorMessage) =>
             new GuidIsNotEmptyConstraint(this, errorMessage);

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<Guid> Invert(StringTemplate errorMessage, Identifier name = null) =>
             new GuidIsEmptyConstraint().WithErrorMessage(errorMessage).WithName(name);

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(Guid value) =>
             !Guid.Empty.Equals(value);

        #endregion
    }

    #endregion

    #region [====== GuidIsEmptyConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a <see cref="Guid" /> is an empty guid..
    /// </summary>
    public sealed class GuidIsEmptyConstraint : Constraint<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuidIsEmptyConstraint" /> class.
        /// </summary>    
        public GuidIsEmptyConstraint() { }

        private GuidIsEmptyConstraint(GuidIsEmptyConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private GuidIsEmptyConstraint(GuidIsEmptyConstraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified =>
             StringTemplate.Parse(ErrorMessages.GuidConstraints_IsEmpty);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<Guid> WithName(Identifier name) =>
             new GuidIsEmptyConstraint(this, name);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<Guid> WithErrorMessage(StringTemplate errorMessage) =>
             new GuidIsEmptyConstraint(this, errorMessage);

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<Guid> Invert(StringTemplate errorMessage, Identifier name = null) =>
             new GuidIsNotEmptyConstraint().WithErrorMessage(errorMessage).WithName(name);

        #endregion

        #region [====== IsSatisfiedBy & IsSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(Guid value) =>
             Guid.Empty.Equals(value);

        #endregion
    }

    #endregion
}
