using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static partial class CollectionConstraints
    {
        #region [====== IsNullOrEmpty & IsNotNullOrEmpty ======]

        /// <summary>
        /// Verifies that the specified collection has at least one element.
        /// </summary>       
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, IEnumerable<TValue>> IsNotNullOrEmpty<TMessage, TValue>(this IMemberConstraint<TMessage, IEnumerable<TValue>> member, string errorMessage = null)
        {
            return member.Apply(new EnumerableIsNotNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the specified collection is either <c>null</c> or empty.
        /// </summary>        
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, IEnumerable<TValue>> IsNullOrEmpty<TMessage, TValue>(this IMemberConstraint<TMessage, IEnumerable<TValue>> member, string errorMessage = null)
        {
            return member.Apply(new EnumerableIsNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage));
        }

        #endregion

        #region [====== ElementAt ======]

        /// <summary>
        /// Verifies that the specified collection contains a value at the specified <paramref name="index"/>.
        /// </summary>        
        /// <param name="member">A member.</param>
        /// <param name="index">The index to verify.</param>   
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> ElementAt<TMessage, TValue>(this IMemberConstraint<TMessage, IEnumerable<TValue>> member, int index, string errorMessage = null)
        {
            return member.Apply(new EnumerableElementAtConstraint<TValue>(index).WithErrorMessage(errorMessage), name => NameOfElementAt(name, index));
        }

        /// <summary>
        /// Attempts to retrieve the <paramref name="element"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the element.</typeparam>
        /// <param name="collection">The collection to get the element from.</param>
        /// <param name="index">The index of the element.</param>
        /// <param name="element">
        /// If this method returns <c>true</c>, this parameter will refer to the element at the specified <paramref name="index"/>;
        /// otherwise, it will be set to the default value of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the collection contains an element at the specified <paramref name="index"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        public static bool TryGetElementAt<TValue>(this IEnumerable<TValue> collection, int index, out TValue element)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (index < 0)
            {
                throw NewNegativeIndexException(index);
            }
            using (var enumerator = collection.GetEnumerator())
            {
                var indexMinusOne = index - 1;
                var currentIndex = -1;

                while (currentIndex < indexMinusOne && enumerator.MoveNext())
                {
                    currentIndex++;
                }
                if (enumerator.MoveNext())
                {
                    element = enumerator.Current;
                    return true;
                }
            }
            element = default(TValue);
            return false;
        }

        #endregion                
    }

    #region [====== EnumerableIsNotNullOrEmptyConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a collection is <c>nul</c> or empty.
    /// </summary>
    public sealed class EnumerableIsNotNullOrEmptyConstraint<TValue> : Constraint<IEnumerable<TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableIsNotNullOrEmptyConstraint{T}" /> class.
        /// </summary>    
        public EnumerableIsNotNullOrEmptyConstraint() {}

        private EnumerableIsNotNullOrEmptyConstraint(EnumerableIsNotNullOrEmptyConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private EnumerableIsNotNullOrEmptyConstraint(EnumerableIsNotNullOrEmptyConstraint<TValue> constraint, Identifier name)
            : base(constraint, name) {}

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.CollectionConstraints_IsNotNullOrEmpty); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>> WithName(Identifier name)
        {
            return new EnumerableIsNotNullOrEmptyConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>> WithErrorMessage(StringTemplate errorMessage)
        {
            return new EnumerableIsNotNullOrEmptyConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new EnumerableIsNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IEnumerable<TValue> value)
        {
            return value != null && value.Any();
        }

        #endregion
    }

    #endregion

    #region [====== EnumerableIsNullOrEmptyConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a collection is <c>nul</c> or empty.
    /// </summary>
    public sealed class EnumerableIsNullOrEmptyConstraint<TValue> : Constraint<IEnumerable<TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableIsNullOrEmptyConstraint{T}" /> class.
        /// </summary>    
        public EnumerableIsNullOrEmptyConstraint() { }

        private EnumerableIsNullOrEmptyConstraint(EnumerableIsNullOrEmptyConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private EnumerableIsNullOrEmptyConstraint(EnumerableIsNullOrEmptyConstraint<TValue> constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.CollectionConstraints_IsNullOrEmpty_Enumerable); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>> WithName(Identifier name)
        {
            return new EnumerableIsNullOrEmptyConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>> WithErrorMessage(StringTemplate errorMessage)
        {
            return new EnumerableIsNullOrEmptyConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new EnumerableIsNotNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IEnumerable<TValue> value)
        {
            return value == null || !value.Any();
        }

        #endregion
    }

    #endregion

    #region [====== EnumerableElementAtConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a collection contains an element at a certain index.
    /// </summary>
    public sealed class EnumerableElementAtConstraint<TValue> : Constraint<IEnumerable<TValue>, TValue>
    {
        private readonly int _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableElementAtConstraint{T}" /> class.
        /// </summary>    
        public EnumerableElementAtConstraint(int index)
        {
            if (index < 0)
            {
                throw CollectionConstraints.NewNegativeIndexException(index);
            }
            _index = index;
        }

        private EnumerableElementAtConstraint(EnumerableElementAtConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _index = constraint._index;
        }

        private EnumerableElementAtConstraint(EnumerableElementAtConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            _index = constraint._index;
        }

        /// <summary>
        /// Index of the element.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.CollectionConstraints_ElementAt_Enumerable); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>, TValue> WithName(Identifier name)
        {
            return new EnumerableElementAtConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>, TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new EnumerableElementAtConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<IEnumerable<TValue>>(this, ErrorMessages.CollectionConstraints_NoElementAt_Enumerable)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IEnumerable<TValue> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return value.Skip(_index).Any();
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IEnumerable<TValue> valueIn, out TValue valueOut)
        {            
            return valueIn.TryGetElementAt(_index, out valueOut);
        }

        #endregion
    }

    #endregion
}
