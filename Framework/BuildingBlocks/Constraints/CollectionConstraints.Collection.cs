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
        #region [====== IsNotNullOrEmpty ======]

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
        public static IMemberConstraint<TMessage, ICollection<TValue>> IsNotNullOrEmpty<TMessage, TValue>(this IMemberConstraint<TMessage, ICollection<TValue>> member, string errorMessage = null)
        {            
            return member.Apply(new CollectionIsNotNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage));
        }

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
        public static IMemberConstraint<TMessage, IReadOnlyCollection<TValue>> IsNotNullOrEmpty<TMessage, TValue>(this IMemberConstraint<TMessage, IReadOnlyCollection<TValue>> member, string errorMessage = null)
        {
            return member.Apply(new ReadOnlyCollectionIsNotNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Checks whether or not the specified <paramref name="collection"/> is not <c>null</c> and contains one or more elements.
        /// </summary>
        /// <typeparam name="TValue">Type of the values of the collection.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="collection"/> is not <c>null</c> and contains at least one element;
        /// otherwise <c>false</c>.
        /// </returns>
        public static bool IsNotNullOrEmpty<TValue>(ICollection<TValue> collection)
        {
            return collection != null && collection.Count > 0;
        }

        /// <summary>
        /// Checks whether or not the specified <paramref name="collection"/> is not <c>null</c> and contains one or more elements.
        /// </summary>
        /// <typeparam name="TValue">Type of the values of the collection.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="collection"/> is not <c>null</c> and contains at least one element;
        /// otherwise <c>false</c>.
        /// </returns>
        public static bool IsNotNullOrEmpty<TValue>(IReadOnlyCollection<TValue> collection)
        {
            return collection != null && collection.Count > 0;
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
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, ICollection<TValue>> IsNullOrEmpty<TMessage, TValue>(this IMemberConstraint<TMessage, ICollection<TValue>> member, string errorMessage = null)
        {
            return member.Apply(new CollectionIsNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage));
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
        public static IMemberConstraint<TMessage, IReadOnlyCollection<TValue>> IsNullOrEmpty<TMessage, TValue>(this IMemberConstraint<TMessage, IReadOnlyCollection<TValue>> member, string errorMessage = null)
        {
            return member.Apply(new ReadOnlyCollectionIsNullOrEmptyReadOnlyConstraint<TValue>().WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Checks whether or not the specified <paramref name="collection"/> is <c>null</c> or has no elements.
        /// </summary>
        /// <typeparam name="TValue">Type of the values of the collection.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="collection"/> is <c>null</c> or contains no elements;
        /// otherwise <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<TValue>(ICollection<TValue> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// Checks whether or not the specified <paramref name="collection"/> is <c>null</c> or has no elements.
        /// </summary>
        /// <typeparam name="TValue">Type of the values of the collection.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="collection"/> is <c>null</c> or contains no elements;
        /// otherwise <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<TValue>(IReadOnlyCollection<TValue> collection)
        {
            return collection == null || collection.Count == 0;
        }

        #endregion

        #region [====== Count ======]

        /// <summary>
        /// Counts the number of elements of the specified collection.
        /// </summary>        
        /// <param name="member">A member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<TMessage, int> Count<TMessage, TValue>(this IMemberConstraint<TMessage, Queue<TValue>> member)
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
        public static IMemberConstraint<TMessage, int> Count<TMessage, TValue>(this IMemberConstraint<TMessage, Stack<TValue>> member)
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
        public static IMemberConstraint<TMessage, int> Count<TMessage, TValue>(this IMemberConstraint<TMessage, LinkedList<TValue>> member)
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
        public static IMemberConstraint<TMessage, int> Count<TMessage, TValue>(this IMemberConstraint<TMessage, ICollection<TValue>> member)
        {
            return member.Select(collection => collection.Count);
        }

        /// <summary>
        /// Counts the number of elements of the specified collection.
        /// </summary>        
        /// <param name="member">A member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<TMessage, int> Count<TMessage, TValue>(this IMemberConstraint<TMessage, IReadOnlyCollection<TValue>> member)
        {
            return member.Select(collection => collection.Count);
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
        public static IMemberConstraint<TMessage, TValue> ElementAt<TMessage, TValue>(this IMemberConstraint<TMessage, LinkedList<TValue>> member, int index, string errorMessage = null)
        {
            return member.As<ICollection<TValue>>().ElementAt(index, errorMessage);
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
        public static IMemberConstraint<TMessage, TValue> ElementAt<TMessage, TValue>(this IMemberConstraint<TMessage, Queue<TValue>> member, int index, string errorMessage = null)
        {
            return member.As<ICollection<TValue>>().ElementAt(index, errorMessage);
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
        public static IMemberConstraint<TMessage, TValue> ElementAt<TMessage, TValue>(this IMemberConstraint<TMessage, Stack<TValue>> member, int index, string errorMessage = null)
        {
            return member.As<ICollection<TValue>>().ElementAt(index, errorMessage);
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
        public static IMemberConstraint<TMessage, TValue> ElementAt<TMessage, TValue>(this IMemberConstraint<TMessage, ICollection<TValue>> member, int index, string errorMessage = null)
        {
            return member.Apply(new CollectionElementAtConstraint<TValue>(index).WithErrorMessage(errorMessage), name => NameOfElementAt(name, index));
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
        public static IMemberConstraint<TMessage, TValue> ElementAt<TMessage, TValue>(this IMemberConstraint<TMessage, IReadOnlyCollection<TValue>> member, int index, string errorMessage = null)
        {
            return member.Apply(new ReadOnlyCollectionElementAtConstraint<TValue>(index).WithErrorMessage(errorMessage), name => NameOfElementAt(name, index));
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
        public static bool TryGetElementAt<TValue>(this ICollection<TValue> collection, int index, out TValue element)
        {
            return TryGetElementAt(collection.AsReadOnlyCollection(), index, out element);
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
        public static bool TryGetElementAt<TValue>(this IReadOnlyCollection<TValue> collection, int index, out TValue element)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (index < 0)
            {
                throw NewNegativeIndexException(index);
            }
            if (index < collection.Count)
            {
                return TryGetElementAt((IEnumerable<TValue>) collection, index, out element);
            }
            element = default(TValue);
            return false;
        }

        #endregion

        /// <summary>
        /// Converts the specified <paramref name="collection"/> to an instance of <see cref="IReadOnlyCollection{T}" />.
        /// </summary>
        /// <typeparam name="TValue">Type of the values of the collection.</typeparam>
        /// <param name="collection">The collection to convert.</param>
        /// <returns>An instance of <see cref="IReadOnlyCollection{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection "/> is <c>null</c>.
        /// </exception>
        public static IReadOnlyCollection<TValue> AsReadOnlyCollection<TValue>(this ICollection<TValue> collection)
        {
            return new ReadOnlyCollection<TValue>(collection);
        }
    }

    #region [====== CollectionIsNotNullOrEmptyConstraints ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a collection is <c>null</c> or empty.
    /// </summary>
    public sealed class CollectionIsNotNullOrEmptyConstraint<TValue> : Constraint<ICollection<TValue>>
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
        public override IConstraintWithErrorMessage<ICollection<TValue>> WithName(Identifier name)
        {
            return new CollectionIsNotNullOrEmptyConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<ICollection<TValue>> WithErrorMessage(StringTemplate errorMessage)
        {
            return new CollectionIsNotNullOrEmptyConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<ICollection<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new CollectionIsNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(ICollection<TValue> value)
        {
            return CollectionConstraints.IsNotNullOrEmpty(value);
        }

        #endregion
    }

    /// <summary>
    /// Represents a constraint that checks whether or not a collection is <c>null</c> or empty.
    /// </summary>
    public sealed class ReadOnlyCollectionIsNotNullOrEmptyConstraint<TValue> : Constraint<IReadOnlyCollection<TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyCollectionIsNotNullOrEmptyConstraint{T}" /> class.
        /// </summary>    
        public ReadOnlyCollectionIsNotNullOrEmptyConstraint() { }

        private ReadOnlyCollectionIsNotNullOrEmptyConstraint(ReadOnlyCollectionIsNotNullOrEmptyConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private ReadOnlyCollectionIsNotNullOrEmptyConstraint(ReadOnlyCollectionIsNotNullOrEmptyConstraint<TValue> constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.CollectionConstraints_IsNotNullOrEmpty); }
        }        

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyCollection<TValue>> WithName(Identifier name)
        {
            return new ReadOnlyCollectionIsNotNullOrEmptyConstraint<TValue>(this, name);
        }       

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyCollection<TValue>> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ReadOnlyCollectionIsNotNullOrEmptyConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyCollection<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ReadOnlyCollectionIsNullOrEmptyReadOnlyConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]
     
        /// <inheritdoc />
        public override bool IsSatisfiedBy(IReadOnlyCollection<TValue> value)
        {
            return CollectionConstraints.IsNotNullOrEmpty(value);
        }        

        #endregion        
    }

    #endregion

    #region [====== CollectionIsNullOrEmptyConstraints ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a collection is <c>null</c> or empty.
    /// </summary>
    public sealed class CollectionIsNullOrEmptyConstraint<TValue> : Constraint<ICollection<TValue>>
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
        public override IConstraintWithErrorMessage<ICollection<TValue>> WithName(Identifier name)
        {
            return new CollectionIsNullOrEmptyConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<ICollection<TValue>> WithErrorMessage(StringTemplate errorMessage)
        {
            return new CollectionIsNullOrEmptyConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<ICollection<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new CollectionIsNotNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(ICollection<TValue> value)
        {
            return CollectionConstraints.IsNullOrEmpty(value);
        }

        #endregion
    }

    /// <summary>
    /// Represents a constraint that checks whether or not a collection is <c>null</c> or empty.
    /// </summary>
    public sealed class ReadOnlyCollectionIsNullOrEmptyReadOnlyConstraint<TValue> : Constraint<IReadOnlyCollection<TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyCollectionIsNullOrEmptyReadOnlyConstraint{T}" /> class.
        /// </summary>    
        public ReadOnlyCollectionIsNullOrEmptyReadOnlyConstraint() { }

        private ReadOnlyCollectionIsNullOrEmptyReadOnlyConstraint(ReadOnlyCollectionIsNullOrEmptyReadOnlyConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private ReadOnlyCollectionIsNullOrEmptyReadOnlyConstraint(ReadOnlyCollectionIsNullOrEmptyReadOnlyConstraint<TValue> constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.CollectionConstraints_IsNullOrEmpty); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyCollection<TValue>> WithName(Identifier name)
        {
            return new ReadOnlyCollectionIsNullOrEmptyReadOnlyConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyCollection<TValue>> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ReadOnlyCollectionIsNullOrEmptyReadOnlyConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyCollection<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ReadOnlyCollectionIsNotNullOrEmptyConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IReadOnlyCollection<TValue> value)
        {
            return CollectionConstraints.IsNullOrEmpty(value);
        }

        #endregion                    
    }

    #endregion    

    #region [====== CollectionElementAtConstraints ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a collection contains an element at a certain index.
    /// </summary>
    public sealed class CollectionElementAtConstraint<TValue> : Constraint<ICollection<TValue>, TValue>
    {
        private readonly int _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionElementAtConstraint{T}" /> class.
        /// </summary>    
        public CollectionElementAtConstraint(int index)
        {
            if (index < 0)
            {
                throw CollectionConstraints.NewNegativeIndexException(index);
            }
            _index = index;
        }

        private CollectionElementAtConstraint(CollectionElementAtConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _index = constraint._index;
        }

        private CollectionElementAtConstraint(CollectionElementAtConstraint<TValue> constraint, Identifier name)
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
            get { return StringTemplate.Parse(ErrorMessages.CollectionConstraints_ElementAt); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<ICollection<TValue>, TValue> WithName(Identifier name)
        {
            return new CollectionElementAtConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<ICollection<TValue>, TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new CollectionElementAtConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<ICollection<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<ICollection<TValue>>(this, ErrorMessages.CollectionConstraints_NoElementAt)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(ICollection<TValue> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return _index < value.Count;
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(ICollection<TValue> valueIn, out TValue valueOut)
        {
            return valueIn.TryGetElementAt(_index, out valueOut);
        }

        #endregion
    }

    /// <summary>
    /// Represents a constraint that checks whether or not a collection contains an element at a certain index.
    /// </summary>
    public sealed class ReadOnlyCollectionElementAtConstraint<TValue> : Constraint<IReadOnlyCollection<TValue>, TValue>
    {
        private readonly int _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyCollectionElementAtConstraint{T}" /> class.
        /// </summary>    
        public ReadOnlyCollectionElementAtConstraint(int index)
        {
            if (index < 0)
            {
                throw CollectionConstraints.NewNegativeIndexException(index);
            }
            _index = index;
        }

        private ReadOnlyCollectionElementAtConstraint(ReadOnlyCollectionElementAtConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _index = constraint._index;
        }

        private ReadOnlyCollectionElementAtConstraint(ReadOnlyCollectionElementAtConstraint<TValue> constraint, Identifier name)
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
            get { return StringTemplate.Parse(ErrorMessages.CollectionConstraints_ElementAt); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyCollection<TValue>, TValue> WithName(Identifier name)
        {
            return new ReadOnlyCollectionElementAtConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyCollection<TValue>, TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ReadOnlyCollectionElementAtConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyCollection<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<IReadOnlyCollection<TValue>>(this, ErrorMessages.CollectionConstraints_NoElementAt)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IReadOnlyCollection<TValue> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return _index < value.Count;
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IReadOnlyCollection<TValue> valueIn, out TValue valueOut)
        {
            return valueIn.TryGetElementAt(_index, out valueOut);
        }

        #endregion
    }

    #endregion
}
