using System;
using System.Collections.Generic;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static partial class CollectionConstraints
    {
        #region [====== Count ======]        

        /// <summary>
        /// Counts the number of elements of the specified collection.
        /// </summary>        
        /// <param name="member">A member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<TMessage, int> Count<TMessage, TValue>(this IMemberConstraint<TMessage, List<TValue>> member)
        {
            return member.As<ICollection<TValue>>().Count();
        }

        /// <summary>
        /// Counts the number of elements of the specified collection.
        /// </summary>        
        /// <param name="member">A member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<TMessage, int> Count<TMessage, TValue>(this IMemberConstraint<TMessage, IList<TValue>> member)
        {
            return member.As<ICollection<TValue>>().Count();
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
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> ElementAt<TMessage, TValue>(this IMemberConstraint<TMessage, List<TValue>> member, int index, string errorMessage = null)
        {
            return member.As<IList<TValue>>().ElementAt(index, errorMessage);
        }

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
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> ElementAt<TMessage, TValue>(this IMemberConstraint<TMessage, IList<TValue>> member, int index, string errorMessage = null)
        {
            return member.Apply(new ListElementAtConstraint<TValue>(index).WithErrorMessage(errorMessage), name => NameOfElementAt(name, index));
        }

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
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> ElementAt<TMessage, TValue>(this IMemberConstraint<TMessage, IReadOnlyList<TValue>> member, int index, string errorMessage = null)
        {
            return member.Apply(new ReadOnlyListElementAtReadOnlyConstraint<TValue>(index).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Attempts to retrieve the <paramref name="element"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the element.</typeparam>
        /// <param name="list">The list to get the element from.</param>
        /// <param name="index">The index of the element.</param>
        /// <param name="element">
        /// If this method returns <c>true</c>, this parameter will refer to the element at the specified <paramref name="index"/>;
        /// otherwise, it will be set to the default value of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the list contains an element at the specified <paramref name="index"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        public static bool TryGetElementAt<TValue>(this IList<TValue> list, int index, out TValue element)
        {
            return TryGetElementAt(list.AsReadOnlyList(), index, out element);
        }

        /// <summary>
        /// Attempts to retrieve the <paramref name="element"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the element.</typeparam>
        /// <param name="list">The list to get the element from.</param>
        /// <param name="index">The index of the element.</param>
        /// <param name="element">
        /// If this method returns <c>true</c>, this parameter will refer to the element at the specified <paramref name="index"/>;
        /// otherwise, it will be set to the default value of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the list contains an element at the specified <paramref name="index"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        public static bool TryGetElementAt<TValue>(this IReadOnlyList<TValue> list, int index, out TValue element)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            if (index < 0)
            {
                throw NewNegativeIndexException(index);
            }
            if (index < list.Count)
            {
                element = list[index];
                return true;
            }
            element = default(TValue);
            return false;
        }

        #endregion

        /// <summary>
        /// Converts the specified <paramref name="list"/> to an instance of <see cref="IReadOnlyList{T}" />.
        /// </summary>
        /// <typeparam name="TValue">Type of the values of the collection.</typeparam>
        /// <param name="list">The collection to convert.</param>
        /// <returns>An instance of <see cref="IReadOnlyList{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is <c>null</c>.
        /// </exception>
        public static IReadOnlyList<TValue> AsReadOnlyList<TValue>(this IList<TValue> list)
        {
            return new ReadOnlyList<TValue>(list);
        }
    }

    #region [====== ListElementAtConstraints ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a list contains an element at a certain index.
    /// </summary>
    public sealed class ListElementAtConstraint<TValue> : Constraint<IList<TValue>, TValue>
    {
        private readonly int _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListElementAtConstraint{T}" /> class.
        /// </summary>    
        public ListElementAtConstraint(int index)
        {
            if (index < 0)
            {
                throw CollectionConstraints.NewNegativeIndexException(index);
            }
            _index = index;
        }

        private ListElementAtConstraint(ListElementAtConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _index = constraint._index;
        }

        private ListElementAtConstraint(ListElementAtConstraint<TValue> constraint, Identifier name)
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
            get { return StringTemplate.Parse(ErrorMessages.ListConstraints_ElementAt); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IList<TValue>, TValue> WithName(Identifier name)
        {
            return new ListElementAtConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IList<TValue>, TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ListElementAtConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IList<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<IList<TValue>>(this, ErrorMessages.CollectionConstraints_NoElementAt)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IList<TValue> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return _index < value.Count;
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IList<TValue> valueIn, out TValue valueOut)
        {
            return valueIn.TryGetElementAt(_index, out valueOut);
        }

        #endregion
    }

    /// <summary>
    /// Represents a constraint that checks whether or not a list contains an element at a certain index.
    /// </summary>
    public sealed class ReadOnlyListElementAtReadOnlyConstraint<TValue> : Constraint<IReadOnlyList<TValue>, TValue>
    {
        private readonly int _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyListElementAtReadOnlyConstraint{T}" /> class.
        /// </summary>    
        public ReadOnlyListElementAtReadOnlyConstraint(int index)
        {
            if (index < 0)
            {
                throw CollectionConstraints.NewNegativeIndexException(index);
            }
            _index = index;
        }

        private ReadOnlyListElementAtReadOnlyConstraint(ReadOnlyListElementAtReadOnlyConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _index = constraint._index;
        }

        private ReadOnlyListElementAtReadOnlyConstraint(ReadOnlyListElementAtReadOnlyConstraint<TValue> constraint, Identifier name)
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
            get { return StringTemplate.Parse(ErrorMessages.ListConstraints_ElementAt); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyList<TValue>, TValue> WithName(Identifier name)
        {
            return new ReadOnlyListElementAtReadOnlyConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyList<TValue>, TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ReadOnlyListElementAtReadOnlyConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyList<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<IReadOnlyList<TValue>>(this, ErrorMessages.CollectionConstraints_NoElementAt)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IReadOnlyList<TValue> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return _index < value.Count;
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IReadOnlyList<TValue> valueIn, out TValue valueOut)
        {
            return valueIn.TryGetElementAt(_index, out valueOut);
        }

        #endregion
    }

    #endregion
}
