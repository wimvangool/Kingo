using System;
using System.Collections.Generic;
using Kingo.Resources;

namespace Kingo.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{T}" />.
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
        public static IMemberConstraintBuilder<T, TValue> IsNotEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, object other, string errorMessage = null)
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
        public static IMemberConstraintBuilder<T, TValue> IsNotEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, TValue other, string errorMessage = null)
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
        public static IMemberConstraintBuilder<T, TValue> IsNotEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, TValue other, IEqualityComparer<TValue> comparer, string errorMessage = null)
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
        public static IMemberConstraintBuilder<T, TValue> IsNotEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, IEquatable<TValue> other, string errorMessage = null)
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
        public static IMemberConstraintBuilder<T, TValue> IsNotEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, object> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException(nameof(otherFactory));
            }
            var errorMessageTemplate = StringTemplate.ParseOrNull(errorMessage);

            return member.Apply(message => new IsNotEqualToConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessageTemplate));
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
        public static IMemberConstraintBuilder<T, TValue> IsNotEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, TValue> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException(nameof(otherFactory));
            }
            var errorMessageTemplate = StringTemplate.ParseOrNull(errorMessage);

            return member.Apply(message => new IsNotEqualToConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessageTemplate));
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
        public static IMemberConstraintBuilder<T, TValue> IsNotEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, TValue> otherFactory, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException(nameof(otherFactory));
            }
            var errorMessageTemplate = StringTemplate.ParseOrNull(errorMessage);

            return member.Apply(message => new IsNotEqualToConstraint<TValue>(otherFactory.Invoke(message), comparer).WithErrorMessage(errorMessageTemplate));
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
        public static IMemberConstraintBuilder<T, TValue> IsNotEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, IEquatable<TValue>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException(nameof(otherFactory));
            }
            var errorMessageTemplate = StringTemplate.ParseOrNull(errorMessage);

            return member.Apply(message => new IsNotEqualToConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessageTemplate));
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
        public static IMemberConstraintBuilder<T, TValue> IsEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, object other, string errorMessage = null)
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
        public static IMemberConstraintBuilder<T, TValue> IsEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, TValue other, string errorMessage = null)
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
        public static IMemberConstraintBuilder<T, TValue> IsEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, TValue other, IEqualityComparer<TValue> comparer, string errorMessage = null)
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
        public static IMemberConstraintBuilder<T, TValue> IsEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, IEquatable<TValue> other, string errorMessage = null)
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
        public static IMemberConstraintBuilder<T, TValue> IsEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, object> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException(nameof(otherFactory));
            }
            var errorMessageTemplate = StringTemplate.ParseOrNull(errorMessage);

            return member.Apply(message => new IsEqualToConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessageTemplate));
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
        public static IMemberConstraintBuilder<T, TValue> IsEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, TValue> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException(nameof(otherFactory));
            }
            var errorMessageTemplate = StringTemplate.ParseOrNull(errorMessage);

            return member.Apply(message => new IsEqualToConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessageTemplate));
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
        public static IMemberConstraintBuilder<T, TValue> IsEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, TValue> otherFactory, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException(nameof(otherFactory));
            }
            var errorMessageTemplate = StringTemplate.ParseOrNull(errorMessage);

            return member.Apply(message => new IsEqualToConstraint<TValue>(otherFactory.Invoke(message), comparer).WithErrorMessage(errorMessageTemplate));
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
        public static IMemberConstraintBuilder<T, TValue> IsEqualTo<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, IEquatable<TValue>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException(nameof(otherFactory));
            }
            var errorMessageTemplate = StringTemplate.ParseOrNull(errorMessage);

            return member.Apply(message => new IsEqualToConstraint<TValue>(otherFactory.Invoke(message)).WithErrorMessage(errorMessageTemplate));
        }

        #endregion
    }

    #region [====== IsNotEqualToConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is equal to another value.
    /// </summary>
    public sealed class IsNotEqualToConstraint<TValue> : Constraint<TValue>
    {
        /// <summary>
        /// The instance that the value is compared to.
        /// </summary>
        public readonly IEquatable<TValue> Other;

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
            Other = other;
        }

        private IsNotEqualToConstraint(IsNotEqualToConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Other = constraint.Other;
        }

        private IsNotEqualToConstraint(IsNotEqualToConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            Other = constraint.Other;
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
            return !Comparer.IsEqualTo(value, Other);
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
        /// <summary>
        /// The instance that the value is compared to.
        /// </summary>
        public readonly IEquatable<TValue> Other;

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
            Other = other;
        }

        private IsEqualToConstraint(IsEqualToConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Other = constraint.Other;
        }

        private IsEqualToConstraint(IsEqualToConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            Other = constraint.Other;
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
            return Comparer.IsEqualTo(value, Other);
        }

        #endregion
    }

    #endregion
}
