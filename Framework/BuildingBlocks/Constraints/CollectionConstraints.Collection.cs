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
            return member.Apply(new IsNotNullOrEmptyCollectionConstraint<TValue>().WithErrorMessage(errorMessage));
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
            return member.Apply(new IsNotNullOrEmptyReadOnlyCollectionConstraint<TValue>().WithErrorMessage(errorMessage));
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
            return member.Apply(new IsNullOrEmptyCollectionConstraint<TValue>().WithErrorMessage(errorMessage));
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
            return member.Apply(new IsNullOrEmptyReadOnlyCollectionConstraint<TValue>().WithErrorMessage(errorMessage));
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
            return member.Apply(new ElementAtCollectionConstraint<TValue>(index).WithErrorMessage(errorMessage), name => NameOfElementAt(name, index));
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
            return member.Apply(new ElementAtReadOnlyCollectionConstraint<TValue>(index).WithErrorMessage(errorMessage), name => NameOfElementAt(name, index));
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

    #region [====== IsNotNullOrEmptyCollectionConstraints ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a collection is <c>null</c> or empty.
    /// </summary>
    public sealed class IsNotNullOrEmptyCollectionConstraint<TValue> : Constraint<ICollection<TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotNullOrEmptyCollectionConstraint{T}" /> class.
        /// </summary>    
        public IsNotNullOrEmptyCollectionConstraint() { }

        private IsNotNullOrEmptyCollectionConstraint(IsNotNullOrEmptyCollectionConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private IsNotNullOrEmptyCollectionConstraint(IsNotNullOrEmptyCollectionConstraint<TValue> constraint, Identifier name)
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
            return new IsNotNullOrEmptyCollectionConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<ICollection<TValue>> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsNotNullOrEmptyCollectionConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<ICollection<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsNullOrEmptyCollectionConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);
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
    public sealed class IsNotNullOrEmptyReadOnlyCollectionConstraint<TValue> : Constraint<IReadOnlyCollection<TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotNullOrEmptyReadOnlyCollectionConstraint{T}" /> class.
        /// </summary>    
        public IsNotNullOrEmptyReadOnlyCollectionConstraint() { }

        private IsNotNullOrEmptyReadOnlyCollectionConstraint(IsNotNullOrEmptyReadOnlyCollectionConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private IsNotNullOrEmptyReadOnlyCollectionConstraint(IsNotNullOrEmptyReadOnlyCollectionConstraint<TValue> constraint, Identifier name)
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
            return new IsNotNullOrEmptyReadOnlyCollectionConstraint<TValue>(this, name);
        }       

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyCollection<TValue>> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsNotNullOrEmptyReadOnlyCollectionConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyCollection<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsNullOrEmptyReadOnlyCollectionConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);
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

    #region [====== IsNullOrEmptyCollectionConstraints ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a collection is <c>null</c> or empty.
    /// </summary>
    public sealed class IsNullOrEmptyCollectionConstraint<TValue> : Constraint<ICollection<TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsNullOrEmptyCollectionConstraint{T}" /> class.
        /// </summary>    
        public IsNullOrEmptyCollectionConstraint() { }

        private IsNullOrEmptyCollectionConstraint(IsNullOrEmptyCollectionConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private IsNullOrEmptyCollectionConstraint(IsNullOrEmptyCollectionConstraint<TValue> constraint, Identifier name)
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
            return new IsNullOrEmptyCollectionConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<ICollection<TValue>> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsNullOrEmptyCollectionConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<ICollection<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsNotNullOrEmptyCollectionConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);
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
    public sealed class IsNullOrEmptyReadOnlyCollectionConstraint<TValue> : Constraint<IReadOnlyCollection<TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsNullOrEmptyReadOnlyCollectionConstraint{T}" /> class.
        /// </summary>    
        public IsNullOrEmptyReadOnlyCollectionConstraint() { }

        private IsNullOrEmptyReadOnlyCollectionConstraint(IsNullOrEmptyReadOnlyCollectionConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private IsNullOrEmptyReadOnlyCollectionConstraint(IsNullOrEmptyReadOnlyCollectionConstraint<TValue> constraint, Identifier name)
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
            return new IsNullOrEmptyReadOnlyCollectionConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyCollection<TValue>> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsNullOrEmptyReadOnlyCollectionConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyCollection<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsNotNullOrEmptyReadOnlyCollectionConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);
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

    #region [====== ElementAtCollectionConstraints ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a collection contains an element at a certain index.
    /// </summary>
    public sealed class ElementAtCollectionConstraint<TValue> : Constraint<ICollection<TValue>, TValue>
    {
        private readonly int _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementAtCollectionConstraint{T}" /> class.
        /// </summary>    
        public ElementAtCollectionConstraint(int index)
        {
            if (index < 0)
            {
                throw CollectionConstraints.NewNegativeIndexException(index);
            }
            _index = index;
        }

        private ElementAtCollectionConstraint(ElementAtCollectionConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _index = constraint._index;
        }

        private ElementAtCollectionConstraint(ElementAtCollectionConstraint<TValue> constraint, Identifier name)
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
            return new ElementAtCollectionConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<ICollection<TValue>, TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ElementAtCollectionConstraint<TValue>(this, errorMessage);
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
    public sealed class ElementAtReadOnlyCollectionConstraint<TValue> : Constraint<IReadOnlyCollection<TValue>, TValue>
    {
        private readonly int _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementAtReadOnlyCollectionConstraint{T}" /> class.
        /// </summary>    
        public ElementAtReadOnlyCollectionConstraint(int index)
        {
            if (index < 0)
            {
                throw CollectionConstraints.NewNegativeIndexException(index);
            }
            _index = index;
        }

        private ElementAtReadOnlyCollectionConstraint(ElementAtReadOnlyCollectionConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _index = constraint._index;
        }

        private ElementAtReadOnlyCollectionConstraint(ElementAtReadOnlyCollectionConstraint<TValue> constraint, Identifier name)
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
            return new ElementAtReadOnlyCollectionConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyCollection<TValue>, TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ElementAtReadOnlyCollectionConstraint<TValue>(this, errorMessage);
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
