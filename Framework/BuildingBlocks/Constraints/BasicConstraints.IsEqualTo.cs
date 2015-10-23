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
        #region [====== IsNotEqualTo ======]

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="other"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, object other, string errorMessage = null)
        {
            return member.Apply(new IsNotEqualToConstraint<TValue>(other).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="other"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue other, string errorMessage = null)
        {
            return member.Apply(new IsNotEqualToConstraint<TValue>(other).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="other"/>.
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
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>     
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue other, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            return member.Apply(new IsNotEqualToConstraint<TValue>(other, comparer).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="other"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, IEquatable<TValue> other, string errorMessage = null)
        {
            return member.Apply(new IsNotEqualToConstraint<TValue>(other).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="otherFactory"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, object> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsNotEqualToConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="otherFactory"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsNotEqualToConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="otherFactory"/>.
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
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>     
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> otherFactory, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsNotEqualToConstraint<TValue>(otherFactory.Invoke(message), comparer).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="otherFactory"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, IEquatable<TValue>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsNotEqualToConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessage));
        }

        #endregion

        #region [====== IsEqualTo ======]

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="other"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, object other, string errorMessage = null)
        {
            return member.Apply(new IsEqualToConstraint<TValue>(other).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="other"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue other, string errorMessage = null)
        {
            return member.Apply(new IsEqualToConstraint<TValue>(other).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="other"/>.
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
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue other, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            return member.Apply(new IsEqualToConstraint<TValue>(other, comparer).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="other"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, IEquatable<TValue> other, string errorMessage = null)
        {
            return member.Apply(new IsEqualToConstraint<TValue>(other).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="otherFactory"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, object> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsEqualToConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="otherFactory"/>.
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
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsEqualToConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="otherFactory"/>.
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
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> otherFactory, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsEqualToConstraint<TValue>(otherFactory.Invoke(message), comparer).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>        
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
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, IEquatable<TValue>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            return member.Apply(message => new IsEqualToConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessage));
        }

        #endregion
    }

    #region [====== IsNotEqualToConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is equal to another value.
    /// </summary>
    public sealed class IsNotEqualToConstraint<TValue> : Constraint<TValue>
    {
        private readonly IEquatable<TValue> _other;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotEqualToConstraint{T}" /> class.
        /// </summary>
        /// <param name="other">Instance to compare the value to.</param>
        public IsNotEqualToConstraint(object other)
            : this(new EquatableObject<TValue>(other)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotEqualToConstraint{T}" /> class.
        /// </summary>
        /// <param name="other">Instance to compare the value to.</param>
        /// <param name="comparer">Optional comparer to use when comparing the two instances.</param>
        public IsNotEqualToConstraint(TValue other, IEqualityComparer<TValue> comparer = null)
            : this(new EquatableValue<TValue>(other, comparer)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotEqualToConstraint{T}" /> class.
        /// </summary>
        /// <param name="other">Instance to compare the value to.</param>
        public IsNotEqualToConstraint(IEquatable<TValue> other)            
        {
            _other = other;
        }

        private IsNotEqualToConstraint(IsNotEqualToConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _other = constraint._other;
        }

        private IsNotEqualToConstraint(IsNotEqualToConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            _other = constraint._other;
        }

        /// <summary>
        /// The instance that the value is compared to.
        /// </summary>
        public IEquatable<TValue> Other
        {
            get { return _other; }
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BasicConstraints_IsNotEqualTo); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new IsNotEqualToConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsNotEqualToConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsEqualToConstraint<TValue>(Other).WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return !Comparer.IsEqualTo(value, _other);
        }

        #endregion
    }

    #endregion

    #region [====== IsEqualToConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is equal to another value.
    /// </summary>
    public sealed class IsEqualToConstraint<TValue> : Constraint<TValue>
    {
        private readonly IEquatable<TValue> _other;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsEqualToConstraint{T}" /> class.
        /// </summary>
        /// <param name="other">Instance to compare the value to.</param>
        public IsEqualToConstraint(object other)
            : this(new EquatableObject<TValue>(other)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsEqualToConstraint{T}" /> class.
        /// </summary>
        /// <param name="other">Instance to compare the value to.</param>
        /// <param name="comparer">Optional comparer to use when comparing the two instances.</param>
        public IsEqualToConstraint(TValue other, IEqualityComparer<TValue> comparer = null)
            : this(new EquatableValue<TValue>(other, comparer)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsEqualToConstraint{T}" /> class.
        /// </summary>
        /// <param name="other">Instance to compare the value to.</param>
        public IsEqualToConstraint(IEquatable<TValue> other)            
        {
            _other = other;
        }

        private IsEqualToConstraint(IsEqualToConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _other = constraint._other;
        }

        private IsEqualToConstraint(IsEqualToConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            _other = constraint._other;
        }

        /// <summary>
        /// The instance that the value is compared to.
        /// </summary>
        public IEquatable<TValue> Other
        {
            get { return _other; }
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BasicConstraints_IsEqualTo); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new IsEqualToConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsEqualToConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsNotEqualToConstraint<TValue>(Other).WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return Comparer.IsEqualTo(value, _other);
        }

        #endregion
    }

    #endregion
}
