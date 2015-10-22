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
            return member.Apply(new IsTrueConstraint(errorMessage));
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
            return member.Apply(new IsFalseConstraint(errorMessage));
        }        

        #endregion
    }

    #region [====== IsTrueConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is <c>true</c>.
    /// </summary>
    public sealed class IsTrueConstraint : Constraint<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsTrueConstraint" /> class.
        /// </summary>
        /// <param name="errorMessage">The error message for this constraint.</param>
        /// <param name="name">The name of this constraint.</param>
        public IsTrueConstraint(string errorMessage = null, string name = null)
            : this(StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name)) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="IsTrueConstraint" /> class.
        /// </summary>
        /// <param name="errorMessage">The error message for this constraint.</param>
        /// <param name="name">The name of this constraint.</param>
        public IsTrueConstraint(StringTemplate errorMessage, Identifier name)
            : base(SelectBetween(errorMessage, ErrorMessages.BooleanConstraints_IsTrue), name) {}

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> WithName(Identifier name)
        {
            return new IsTrueConstraint(ErrorMessage, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsTrueConstraint(errorMessage, Name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsFalseConstraint(errorMessage, name);
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(bool value)
        {
            return value;
        }
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
        /// <param name="errorMessage">The error message for this constraint.</param>
        /// <param name="name">The name of this constraint.</param>
        public IsFalseConstraint(string errorMessage = null, string name = null)
            : this(StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name)) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="IsFalseConstraint" /> class.
        /// </summary>
        /// <param name="errorMessage">The error message for this constraint.</param>
        /// <param name="name">The name of this constraint.</param>
        public IsFalseConstraint(StringTemplate errorMessage, Identifier name)
            : base(SelectBetween(errorMessage, ErrorMessages.BooleanConstraints_IsFalse), name) {}

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> WithName(Identifier name)
        {
            return new IsFalseConstraint(ErrorMessage, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsFalseConstraint(errorMessage, Name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsTrueConstraint(errorMessage, name);
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(bool value)
        {
            return !value;
        }
    }

    #endregion
}
