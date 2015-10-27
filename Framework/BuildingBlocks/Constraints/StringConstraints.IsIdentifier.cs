using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{String}" />.
    /// </summary>
    public static partial class StringConstraints
    {
        #region [====== IsIdentifier ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value can be converted to an <see cref="Identifier" />.
        /// </summary>
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, Identifier> IsIdentifier<TMessage>(this IMemberConstraint<TMessage, string> member, string errorMessage = null)
        {
            return member.Apply(new StringIsIdentifierConstraint().WithErrorMessage(errorMessage));
        }

        #endregion
    }

    #region [====== StringIsIdentifierConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string can be converted to an <see cref="Identifier" />.
    /// </summary>
    public sealed class StringIsIdentifierConstraint : Constraint<string, Identifier>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsIdentifierConstraint" /> class.
        /// </summary>    
        public StringIsIdentifierConstraint() {}

        private StringIsIdentifierConstraint(StringIsIdentifierConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private StringIsIdentifierConstraint(StringIsIdentifierConstraint constraint, Identifier name)
            : base(constraint, name) {}

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsIdentifier); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, Identifier> WithName(Identifier name)
        {
            return new StringIsIdentifierConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, Identifier> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsIdentifierConstraint(this, errorMessage);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string valueIn, out Identifier valueOut)
        {
            return Identifier.TryParse(valueIn, out valueOut);
        }

        #endregion
    }

    #endregion
}
