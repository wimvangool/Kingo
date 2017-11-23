using System;
using System.Collections;
using System.Linq;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{T}" />.
    /// </summary>
    public static partial class CollectionConstraints
    {
        #region [====== IsNotNullOrEmpty ======]

        /// <summary>
        /// Verifies that the specified collection has at least one element.
        /// </summary>       
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, TValue> IsNotNullOrEmpty<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, string errorMessage = null)   
            where TValue : IEnumerable => member.Apply(new CollectionIsNotNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage));

        internal static bool IsNotNullOrEmpty(IEnumerable value) =>
             value != null && value.Cast<object>().Any();

        #endregion

        #region [====== IsNullOrEmpty ======]

        /// <summary>
        /// Verifies that the specified collection is either <c>null</c> or empty.
        /// </summary>        
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, TValue> IsNullOrEmpty<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, string errorMessage = null)            
            where TValue : IEnumerable => member.Apply(new CollectionIsNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage));

        #endregion        
    }    

    #region [====== CollectionIsNotNullOrEmptyConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a collection is <c>null</c> or empty.
    /// </summary>
    public sealed class CollectionIsNotNullOrEmptyConstraint<TValue> : Constraint<TValue>        
        where TValue : IEnumerable
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
        protected override StringTemplate ErrorMessageIfNotSpecified =>
             StringTemplate.Parse(ErrorMessages.CollectionConstraints_IsNotNullOrEmpty);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name) =>
             new CollectionIsNotNullOrEmptyConstraint<TValue>(this, name);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage) =>
             new CollectionIsNotNullOrEmptyConstraint<TValue>(this, errorMessage);

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null) =>
             new CollectionIsNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value) =>
             CollectionConstraints.IsNotNullOrEmpty(value);

        #endregion        
    }

    #endregion

    #region [====== CollectionIsNullOrEmptyConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a collection is <c>null</c> or empty.
    /// </summary>
    public sealed class CollectionIsNullOrEmptyConstraint<TValue> : Constraint<TValue>      
        where TValue : IEnumerable
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
        protected override StringTemplate ErrorMessageIfNotSpecified =>
             StringTemplate.Parse(ErrorMessages.CollectionConstraints_IsNullOrEmpty);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name) =>
             new CollectionIsNullOrEmptyConstraint<TValue>(this, name);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage) =>
             new CollectionIsNullOrEmptyConstraint<TValue>(this, errorMessage);

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null) =>
             new CollectionIsNotNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);

        #endregion

        #region [====== IsSatisfiedBy & IsSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value) =>
             !CollectionConstraints.IsNotNullOrEmpty(value);

        #endregion
    }
   
    #endregion
}
