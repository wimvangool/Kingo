using System;
using Kingo.Messaging;
using Kingo.Resources;

namespace Kingo.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{T}" />.
    /// </summary>
    public static partial class BasicConstraints
    {
        #region [====== IsNotSameInstanceAs ======]

        /// <summary>
        /// Verifies that the member's value does not refer to the same instance as <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's reference to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>       
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, TValue> IsNotSameInstanceAs<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, object other, string errorMessage = null)
        {
            return member.Apply(new IsNotSameInstanceAsConstraint<TValue>(other).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value does not refer to the same instance as <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's reference to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>       
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, TValue> IsNotSameInstanceAs<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, object> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException(nameof(otherFactory));
            }
            var errorMessageTemplate = StringTemplate.ParseOrNull(errorMessage);

            return member.Apply(message => new IsNotSameInstanceAsConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessageTemplate));
        }

        #endregion

        #region [====== IsSameInstanceAs ======]

        /// <summary>
        /// Verifies that the member's value refers to the same instance as <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's reference to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>A member that has been merged with the specified member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>   
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, TValue> IsSameInstanceAs<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, object other, string errorMessage = null)
        {
            return member.Apply(new IsSameInstanceAsConstraint<TValue>(other).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value refers to the same instance as <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's reference to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>A member that has been merged with the specified member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>   
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, TValue> IsSameInstanceAs<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, object> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException(nameof(otherFactory));
            }
            var errorMessageTemplate = StringTemplate.ParseOrNull(errorMessage);

            return member.Apply(message => new IsSameInstanceAsConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessageTemplate));
        }

        #endregion
    }

    #region [====== IsNotSameInstanceAsConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value refers to the same instance as another value.
    /// </summary>
    public sealed class IsNotSameInstanceAsConstraint<TValue> : Constraint<TValue>
    {
        /// <summary>
        /// The instance to compare the value to.
        /// </summary>
        public readonly object Other;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotSameInstanceAsConstraint{T}" /> class.
        /// </summary>    
        /// <param name="other">The instance to compare the value to.</param>
        public IsNotSameInstanceAsConstraint(object other)
        {
            Other = other;
        }

        private IsNotSameInstanceAsConstraint(IsNotSameInstanceAsConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Other = constraint.Other;
        }

        private IsNotSameInstanceAsConstraint(IsNotSameInstanceAsConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            Other = constraint.Other;
        }               

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BasicConstraints_IsNotSameInstanceAs); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new IsNotSameInstanceAsConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsNotSameInstanceAsConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsSameInstanceAsConstraint<TValue>(Other).WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return !ReferenceEquals(value, Other);
        }

        #endregion
    }

    #endregion

    #region [====== IsSameInstanceAsConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value refers to the same instance as another value.
    /// </summary>
    public sealed class IsSameInstanceAsConstraint<TValue> : Constraint<TValue>
    {
        /// <summary>
        /// The instance to compare the value to.
        /// </summary>
        public readonly object Other;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsSameInstanceAsConstraint{T}" /> class.
        /// </summary>    
        /// <param name="other">The instance to compare the value to.</param>
        public IsSameInstanceAsConstraint(object other)
        {
            Other = other;
        }

        private IsSameInstanceAsConstraint(IsSameInstanceAsConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Other = constraint.Other;
        }

        private IsSameInstanceAsConstraint(IsSameInstanceAsConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            Other = constraint.Other;
        }              

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BasicConstraints_IsSameInstanceAs); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new IsSameInstanceAsConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsSameInstanceAsConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsNotSameInstanceAsConstraint<TValue>(Other).WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return ReferenceEquals(value, Other);
        }

        #endregion
    }

    #endregion
    
}
