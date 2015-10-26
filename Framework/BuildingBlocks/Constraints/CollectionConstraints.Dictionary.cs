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
        #region [====== ElementAt ======]

        /// <summary>
        /// Verifies that the specified collection contains a value with the specified <paramref name="key"/>.
        /// </summary>        
        /// <param name="member">A member.</param>
        /// <param name="key">The key of the element.</param>   
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
        public static IMemberConstraint<TMessage, TValue> ElementAt<TMessage, TKey, TValue>(this IMemberConstraint<TMessage, IDictionary<TKey, TValue>> member, TKey key, string errorMessage = null)
        {
            return member.Apply(new ElementAtDictionaryConstraint<TKey, TValue>(key).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the specified collection contains a value with the specified <paramref name="key"/>.
        /// </summary>        
        /// <param name="member">A member.</param>
        /// <param name="key">The key of the element.</param>   
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
        public static IMemberConstraint<TMessage, TValue> ElementAt<TMessage, TKey, TValue>(this IMemberConstraint<TMessage, IReadOnlyDictionary<TKey, TValue>> member, TKey key, string errorMessage = null)
        {
            return member.Apply(new ElementAtReadOnlyDictionaryConstraint<TKey, TValue>(key).WithErrorMessage(errorMessage));
        }

        #endregion
    }

    #region [====== ElementAtDictionaryConstraints ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a dictionary contains a value with a certain key.
    /// </summary>
    public sealed class ElementAtDictionaryConstraint<TKey, TValue> : Constraint<IDictionary<TKey, TValue>, TValue>
    {
        private readonly TKey _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementAtDictionaryConstraint{T, S}" /> class.
        /// </summary>    
        public ElementAtDictionaryConstraint(TKey key)
        {
            _key = key;
        }

        private ElementAtDictionaryConstraint(ElementAtDictionaryConstraint<TKey, TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _key = constraint._key;
        }

        private ElementAtDictionaryConstraint(ElementAtDictionaryConstraint<TKey, TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            _key = constraint._key;
        }

        /// <summary>
        /// Key of the element.
        /// </summary>
        public TKey Key
        {
            get { return _key; }
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.CollectionConstraints_ElementAt_Dictionary); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IDictionary<TKey, TValue>, TValue> WithName(Identifier name)
        {
            return new ElementAtDictionaryConstraint<TKey, TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IDictionary<TKey, TValue>, TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ElementAtDictionaryConstraint<TKey, TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IDictionary<TKey, TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<IDictionary<TKey, TValue>>(this, ErrorMessages.CollectionConstraints_NoElementAt)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IDictionary<TKey, TValue> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return value.ContainsKey(_key);
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IDictionary<TKey, TValue> valueIn, out TValue valueOut)
        {
            if (valueIn == null)
            {
                throw new ArgumentNullException("valueIn");
            }
            return valueIn.TryGetValue(_key, out valueOut);
        }

        #endregion
    }

    /// <summary>
    /// Represents a constraint that checks whether or not a dictionary contains a value with a certain key.
    /// </summary>
    public sealed class ElementAtReadOnlyDictionaryConstraint<TKey, TValue> : Constraint<IReadOnlyDictionary<TKey, TValue>, TValue>
    {
        private readonly TKey _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementAtReadOnlyDictionaryConstraint{T, S}" /> class.
        /// </summary>    
        public ElementAtReadOnlyDictionaryConstraint(TKey key)
        {
            _key = key;
        }

        private ElementAtReadOnlyDictionaryConstraint(ElementAtReadOnlyDictionaryConstraint<TKey, TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _key = constraint._key;
        }

        private ElementAtReadOnlyDictionaryConstraint(ElementAtReadOnlyDictionaryConstraint<TKey, TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            _key = constraint._key;
        }

        /// <summary>
        /// Key of the element.
        /// </summary>
        public TKey Key
        {
            get { return _key; }
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.CollectionConstraints_ElementAt_Dictionary); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyDictionary<TKey, TValue>, TValue> WithName(Identifier name)
        {
            return new ElementAtReadOnlyDictionaryConstraint<TKey, TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyDictionary<TKey, TValue>, TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ElementAtReadOnlyDictionaryConstraint<TKey, TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyDictionary<TKey, TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<IReadOnlyDictionary<TKey, TValue>>(this, ErrorMessages.CollectionConstraints_NoElementAt)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IReadOnlyDictionary<TKey, TValue> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return value.ContainsKey(_key);
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IReadOnlyDictionary<TKey, TValue> valueIn, out TValue valueOut)
        {
            if (valueIn == null)
            {
                throw new ArgumentNullException("valueIn");
            }
            return valueIn.TryGetValue(_key, out valueOut);
        }

        #endregion
    }

    #endregion
}
