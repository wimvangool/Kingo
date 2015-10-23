using System;
using System.Collections.Generic;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static partial class BasicConstraints
    {
        #region [====== IsSmallerThan ======]

        /// <summary>
        /// Verifies that the member's value is smaller than <paramref name="other"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsSmallerThan<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            return member.Apply(new IsSmallerThanConstraint<TValue>(other, comparer).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is smaller than <paramref name="other"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsSmallerThan<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, IComparable<TValue> other, string errorMessage = null)
        {
            return member.Apply(new IsSmallerThanConstraint<TValue>(other).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is smaller than <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>  
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>            
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="otherFactory"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsSmallerThan<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> otherFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsSmallerThanConstraint<TValue>(otherFactory.Invoke(message), comparer).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is smaller than <paramref name="otherFactory"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsSmallerThan<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, IComparable<TValue>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsSmallerThanConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessage));
        }

        #endregion

        #region [====== IsSmallerThanOrEqualTo ======]

        /// <summary>
        /// Verifies that the member's value is smaller than or equal to <paramref name="other"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsSmallerThanOrEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            return member.Apply(new IsSmallerThanOrEqualToConstraint<TValue>(other, comparer).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is smaller than or equal to <paramref name="other"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsSmallerThanOrEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, IComparable<TValue> other, string errorMessage = null)
        {
            return member.Apply(new IsSmallerThanOrEqualToConstraint<TValue>(other).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is smaller than or equal to <paramref name="otherFactory"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsSmallerThanOrEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> otherFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsSmallerThanOrEqualToConstraint<TValue>(otherFactory.Invoke(message), comparer).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is smaller than or equal to <paramref name="otherFactory"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsSmallerThanOrEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, IComparable<TValue>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsSmallerThanOrEqualToConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessage));
        }

        #endregion
    }

    #region [====== IsSmallerThanConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is smaller than another value.
    /// </summary>
    public sealed class IsSmallerThanConstraint<TValue> : Constraint<TValue>
    {
        private readonly IComparable<TValue> _other;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsSmallerThanConstraint{T}" /> class.
        /// </summary>    
        /// <param name="other">Instance to compare the value to.</param>
        /// <param name="comparer">Optional comparer to use when comparing the two instances.</param>
        public IsSmallerThanConstraint(TValue other, IComparer<TValue> comparer = null)
            : this(new Comparable<TValue>(other, comparer)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsSmallerThanConstraint{T}" /> class.
        /// </summary>    
        /// <param name="other">Instance to compare the value to.</param>        
        public IsSmallerThanConstraint(IComparable<TValue> other)
        {
            _other = other;
        }

        private IsSmallerThanConstraint(IsSmallerThanConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _other = constraint._other;
        }

        private IsSmallerThanConstraint(IsSmallerThanConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            _other = constraint._other;
        }

        /// <summary>
        /// Instance to compare the value to.
        /// </summary>
        public IComparable<TValue> Other
        {
            get { return _other; }
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BasicConstraints_IsSmallerThan); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new IsSmallerThanConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsSmallerThanConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsGreaterThanOrEqualToConstraint<TValue>(_other).WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return Comparer.IsSmallerThan(value, _other);
        }

        #endregion
    }

    #endregion

    #region [====== IsSmallerThanOrEqualToConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is smaller than or equal to another value.
    /// </summary>
    public sealed class IsSmallerThanOrEqualToConstraint<TValue> : Constraint<TValue>
    {
        private readonly IComparable<TValue> _other;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsSmallerThanOrEqualToConstraint{T}" /> class.
        /// </summary>    
        /// <param name="other">Instance to compare the value to.</param>
        /// <param name="comparer">Optional comparer to use when comparing the two instances.</param>
        public IsSmallerThanOrEqualToConstraint(TValue other, IComparer<TValue> comparer = null)
            : this(new Comparable<TValue>(other, comparer)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsSmallerThanOrEqualToConstraint{T}" /> class.
        /// </summary>    
        /// <param name="other">Instance to compare the value to.</param>        
        public IsSmallerThanOrEqualToConstraint(IComparable<TValue> other)
        {
            _other = other;
        }

        private IsSmallerThanOrEqualToConstraint(IsSmallerThanOrEqualToConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _other = constraint._other;
        }

        private IsSmallerThanOrEqualToConstraint(IsSmallerThanOrEqualToConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            _other = constraint._other;
        }

        /// <summary>
        /// Instance to compare the value to.
        /// </summary>
        public IComparable<TValue> Other
        {
            get { return _other; }
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BasicConstraints_IsSmallerThanOrEqualTo); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new IsSmallerThanOrEqualToConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsSmallerThanOrEqualToConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsGreaterThanConstraint<TValue>(_other).WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return Comparer.IsSmallerThanOrEqualTo(value, _other);
        }

        #endregion
    }

    #endregion
}
