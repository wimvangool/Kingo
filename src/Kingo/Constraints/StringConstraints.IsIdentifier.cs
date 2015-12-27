using System;
using Kingo.Messaging;
using Kingo.Resources;

namespace Kingo.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{String}" />.
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
        public static IMemberConstraintBuilder<T, Identifier> IsIdentifier<T>(this IMemberConstraintBuilder<T, string> member, string errorMessage = null)
        {
            return member.Apply(new StringIsIdentifierFilter().WithErrorMessage(errorMessage));
        }

        #endregion
    }

    #region [====== StringIsIdentifierFilter ======]

    /// <summary>
    /// Represents a filter that transforms a string into an <see cref="Identifier" />.
    /// </summary>
    public sealed class StringIsIdentifierFilter : Filter<string, Identifier>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsIdentifierFilter" /> class.
        /// </summary>    
        public StringIsIdentifierFilter() {}

        private StringIsIdentifierFilter(StringIsIdentifierFilter constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private StringIsIdentifierFilter(StringIsIdentifierFilter constraint, Identifier name)
            : base(constraint, name) {}

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsIdentifier); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, Identifier> WithName(Identifier name)
        {
            return new StringIsIdentifierFilter(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, Identifier> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsIdentifierFilter(this, errorMessage);
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
