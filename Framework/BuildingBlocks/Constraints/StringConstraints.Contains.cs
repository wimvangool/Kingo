using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{String}" />.
    /// </summary>
    public static partial class StringConstraints
    {
        #region [====== DoesNotContain ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not contain the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
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
        public static IMemberConstraint<TMessage, string> DoesNotContain<TMessage>(this IMemberConstraint<TMessage, string> member, char value, string errorMessage = null)
        {
            return member.DoesNotContain(value.ToString(), errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not contain the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> DoesNotContain<TMessage>(this IMemberConstraint<TMessage, string> member, string value, string errorMessage = null)
        {
            return member.Apply(new StringContainsConstraint(value).Invert(errorMessage));
        }

        #endregion

        #region [====== Contains ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value contains the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
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
        public static IMemberConstraint<TMessage, string> Contains<TMessage>(this IMemberConstraint<TMessage, string> member, char value, string errorMessage = null)
        {
            return member.Contains(value.ToString(), errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value contains the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> Contains<TMessage>(this IMemberConstraint<TMessage, string> member, string value, string errorMessage = null)
        {
            return member.Apply(new StringContainsConstraint(value).WithErrorMessage(errorMessage));
        }

        #endregion
    }

    #region [====== StringContainsConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string contains a certain value.
    /// </summary>
    public sealed class StringContainsConstraint : Constraint<string>
    {
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringContainsConstraint" /> class.
        /// </summary>    
        /// <param name="value">The value to check for.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public StringContainsConstraint(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            _value = value;
        }

        private StringContainsConstraint(StringContainsConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _value = constraint._value;
        }

        private StringContainsConstraint(StringContainsConstraint constraint, Identifier name)
            : base(constraint, name)
        {
            _value = constraint._value;
        }

        /// <summary>
        /// The value to check for.
        /// </summary>
        public string Value
        {
            get { return _value; }
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_Contains); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithName(Identifier name)
        {
            return new StringContainsConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringContainsConstraint(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<string>(this, ErrorMessages.StringConstraints_DoesNotContain)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return value.Contains(_value);
        }

        #endregion
    }

    #endregion
}
