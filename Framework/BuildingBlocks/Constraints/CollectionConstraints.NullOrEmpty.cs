using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{T}" />.
    /// </summary>
    public static partial class CollectionConstraints
    {
        #region [====== IsNotNullOrEmpty ======]

        /// <summary>
        /// Verifies that the specified collection has at least one element.
        /// </summary>       
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraint{T}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<T, IEnumerable<TValue>> IsNotNullOrEmpty<T, TValue>(this IMemberConstraint<T, IEnumerable<TValue>> member, string errorMessage = null)                
        {
            return member.Apply(new CollectionIsNotNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage));                        
        }

        /// <summary>
        /// Determines whether or not the specified <paramref name="value"/> is not <c>null</c> and contains at least one element.
        /// </summary>
        /// <typeparam name="TValue">Type of the items of the collection.</typeparam>
        /// <param name="value">A collection.</param>
        /// <returns><c>true</c> if the value is not <c>null</c> or contains at least one element; otherwise <c>false</c>.</returns>
        public static bool IsNotNullOrEmpty<TValue>(IEnumerable<TValue> value)
        {
            return !IsNullOrEmpty(value);
        }

        #endregion

        #region [====== IsNullOrEmpty ======]

        /// <summary>
        /// Verifies that the specified collection is either <c>null</c> or empty.
        /// </summary>        
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraint{T}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<T, IEnumerable<TValue>> IsNullOrEmpty<T, TValue>(this IMemberConstraint<T, IEnumerable<TValue>> member, string errorMessage = null)            
        {
            return member.Apply(new CollectionIsNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage));                        
        }   
     
        /// <summary>
        /// Determines whether or not the specified <paramref name="value"/> is <c>null</c> or is an empty collection.
        /// </summary>
        /// <typeparam name="TValue">Type of the items of the collection.</typeparam>
        /// <param name="value">A collection.</param>
        /// <returns><c>true</c> if the value is either <c>null</c> or contains no elements; otherwise <c>false</c>.</returns>
        public static bool IsNullOrEmpty<TValue>(IEnumerable<TValue> value)
        {
            if (value == null)
            {
                return true;
            }
            var readonlyCollection = value as IReadOnlyCollection<TValue>;
            if (readonlyCollection != null)
            {
                return readonlyCollection.Count == 0;
            }
            var collection = value as ICollection<TValue>;
            if (collection != null)
            {
                return collection.Count == 0;
            }
            return !value.Any();
        }

        #endregion        
    }    

    #region [====== CollectionIsNotNullOrEmptyConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a collection is <c>null</c> or empty.
    /// </summary>
    public sealed class CollectionIsNotNullOrEmptyConstraint<TValue> : Constraint<IEnumerable<TValue>>        
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionIsNotNullOrEmptyConstraint{T}" /> class.
        /// </summary>    
        public CollectionIsNotNullOrEmptyConstraint() { }

        private CollectionIsNotNullOrEmptyConstraint(CollectionIsNotNullOrEmptyConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private CollectionIsNotNullOrEmptyConstraint(CollectionIsNotNullOrEmptyConstraint<TValue> constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.CollectionConstraints_IsNotNullOrEmpty); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>> WithName(Identifier name)
        {
            return new CollectionIsNotNullOrEmptyConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>> WithErrorMessage(StringTemplate errorMessage)
        {
            return new CollectionIsNotNullOrEmptyConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new CollectionIsNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IEnumerable<TValue> value)
        {
            return CollectionConstraints.IsNotNullOrEmpty(value);
        }

        #endregion        
    }

    #endregion

    #region [====== CollectionIsNullOrEmptyConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a collection is <c>null</c> or empty.
    /// </summary>
    public sealed class CollectionIsNullOrEmptyConstraint<TValue> : Constraint<IEnumerable<TValue>>        
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionIsNullOrEmptyConstraint{T}" /> class.
        /// </summary>    
        public CollectionIsNullOrEmptyConstraint() { }

        private CollectionIsNullOrEmptyConstraint(CollectionIsNullOrEmptyConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private CollectionIsNullOrEmptyConstraint(CollectionIsNullOrEmptyConstraint<TValue> constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.CollectionConstraints_IsNullOrEmpty); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>> WithName(Identifier name)
        {
            return new CollectionIsNullOrEmptyConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>> WithErrorMessage(StringTemplate errorMessage)
        {
            return new CollectionIsNullOrEmptyConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new CollectionIsNotNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IEnumerable<TValue> value)
        {
            return CollectionConstraints.IsNullOrEmpty(value);
        }

        #endregion
    }
   
    #endregion
}
