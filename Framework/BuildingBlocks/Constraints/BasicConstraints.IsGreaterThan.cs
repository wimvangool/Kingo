using System;
using System.Collections.Generic;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{T}" />.
    /// </summary>
    public static partial class BasicConstraints
    {
        #region [====== IsGreaterThan ======]

        /// <summary>
        /// Verifies that the member is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>    
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraintBuilder<T, TValue> IsGreaterThan<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            return member.Apply(new IsGreaterThanConstraint<TValue>(other, comparer).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>        
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
        public static IMemberConstraintBuilder<T, TValue> IsGreaterThan<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, IComparable<TValue> other, string errorMessage = null)
        {
            return member.Apply(new IsGreaterThanConstraint<TValue>(other).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member is greater than <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>    
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="otherFactory"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraintBuilder<T, TValue> IsGreaterThan<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, TValue> otherFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsGreaterThanConstraint<TValue>(otherFactory.Invoke(message), comparer).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member is greater than <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>        
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
        public static IMemberConstraintBuilder<T, TValue> IsGreaterThan<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, IComparable<TValue>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsGreaterThanConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessage));
        }

        #endregion

        #region [====== IsGreaterThanOrEqualTo ======]

        /// <summary>
        /// Verifies that the member is greater than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>        
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraintBuilder<T, TValue> IsGreaterThanOrEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            return member.Apply(new IsGreaterThanOrEqualToConstraint<TValue>(other, comparer).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>        
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
        public static IMemberConstraintBuilder<T, TValue> IsGreaterThanOrEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, IComparable<TValue> other, string errorMessage = null)
        {
            return member.Apply(new IsGreaterThanOrEqualToConstraint<TValue>(other).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member is greater than or equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>        
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="otherFactory"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraintBuilder<T, TValue> IsGreaterThanOrEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, TValue> otherFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsGreaterThanOrEqualToConstraint<TValue>(otherFactory.Invoke(message), comparer).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member is equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>        
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
        public static IMemberConstraintBuilder<T, TValue> IsGreaterThanOrEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, IComparable<TValue>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsGreaterThanOrEqualToConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessage));        
        }

        #endregion
    }

    #region [====== IsGreaterThanConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is greater than another value.
    /// </summary>
    public sealed class IsGreaterThanConstraint<TValue> : Constraint<TValue>
    {
        /// <summary>
        /// Instance to compare the value to.
        /// </summary>
        public readonly IComparable<TValue> Other;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsGreaterThanConstraint{T}" /> class.
        /// </summary>    
        /// <param name="other">Instance to compare the value to.</param>
        /// <param name="comparer">Optional comparer to use when comparing the two instances.</param>
        public IsGreaterThanConstraint(TValue other, IComparer<TValue> comparer = null)
            : this(new Comparable<TValue>(other, comparer)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsGreaterThanConstraint{T}" /> class.
        /// </summary>    
        /// <param name="other">Instance to compare the value to.</param>
        public IsGreaterThanConstraint(IComparable<TValue> other)
        {
            Other = other;
        }

        private IsGreaterThanConstraint(IsGreaterThanConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Other = constraint.Other;
        }

        private IsGreaterThanConstraint(IsGreaterThanConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            Other = constraint.Other;
        }              

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BasicConstraints_IsGreaterThan); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new IsGreaterThanConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsGreaterThanConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsSmallerThanOrEqualToConstraint<TValue>(Other).WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return Comparer.IsGreaterThan(value, Other);
        }

        #endregion
    }

    #endregion

    #region [====== IsGreaterThanOrEqualToConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is greater than or equal to another value.
    /// </summary>
    public sealed class IsGreaterThanOrEqualToConstraint<TValue> : Constraint<TValue>
    {
        /// <summary>
        /// Instance to compare the value to.
        /// </summary>
        public readonly IComparable<TValue> Other;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsGreaterThanOrEqualToConstraint{T}" /> class.
        /// </summary>    
        /// <param name="other">Instance to compare the value to.</param>
        /// <param name="comparer">Optional comparer to use when comparing the two instances.</param>
        public IsGreaterThanOrEqualToConstraint(TValue other, IComparer<TValue> comparer = null)
            : this(new Comparable<TValue>(other, comparer)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsGreaterThanOrEqualToConstraint{T}" /> class.
        /// </summary>    
        /// <param name="other">Instance to compare the value to.</param>
        public IsGreaterThanOrEqualToConstraint(IComparable<TValue> other)
        {
            Other = other;
        }

        private IsGreaterThanOrEqualToConstraint(IsGreaterThanOrEqualToConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Other = constraint.Other;
        }

        private IsGreaterThanOrEqualToConstraint(IsGreaterThanOrEqualToConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            Other = constraint.Other;
        }               

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BasicConstraints_IsGreaterThanOrEqualTo); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new IsGreaterThanOrEqualToConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsGreaterThanOrEqualToConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsSmallerThanConstraint<TValue>(Other).WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return Comparer.IsGreaterThanOrEqualTo(value, Other);
        }

        #endregion
    }

    #endregion
}
