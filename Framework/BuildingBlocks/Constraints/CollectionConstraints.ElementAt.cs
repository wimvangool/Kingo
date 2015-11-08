using System;
using System.Collections.Generic;
using System.Globalization;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{T}" />.
    /// </summary>
    public static partial class CollectionConstraints
    {
        #region [====== ElementAt ======]

        /// <summary>
        /// Verifies that the specified collection contains an element at the specified <paramref name="index"/> and returns it.
        /// </summary>       
        /// <param name="member">A member.</param>  
        /// <param name="index">Index of the element to select.</param>      
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
        public static IMemberConstraint<T, TValue> ElementAt<T, TValue>(this IMemberConstraint<T, IEnumerable<TValue>> member, int index, string errorMessage = null)
        {
            return member.Apply(new ElementAtFilter<TValue>(index).WithErrorMessage(errorMessage), name => NameOfElementAt(name, index));
        }

        /// <summary>
        /// Verifies that the specified dictionary contains an element with the specified <paramref name="key"/> and returns it.
        /// </summary>       
        /// <param name="member">A member.</param>  
        /// <param name="key">key of the element to select.</param>      
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
        public static IMemberConstraint<T, TValue> ElementAt<T, TKey, TValue>(this IMemberConstraint<T, IReadOnlyDictionary<TKey, TValue>> member, TKey key, string errorMessage = null)
        {
            return member.Apply(new ElementAtFilter<TKey, TValue>(key).WithErrorMessage(errorMessage), name => NameOfElementAt(name, key));            
        }

        #endregion

        #region [====== TryGetElementAt ======]

        /// <summary>
        /// Attempts to retrieve the <paramref name="element"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the element.</typeparam>
        /// <param name="value">The collection to get the element from.</param>
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
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>        
        public static bool TryGetElementAt<TValue>(this IEnumerable<TValue> value, int index, out TValue element)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (0 <= index)
            {
                using (var enumerator = value.GetEnumerator())
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
            }
            element = default(TValue);
            return false;                
        }          

        private static string NameOfElementAt(string member, object keyOrIndex)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", member, keyOrIndex);
        }

        internal static Exception NewIndexOutOfRangeException(int index)
        {
            var messageFormat = ExceptionMessages.CollectionConstraints_IndexOutOfRange;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }

        #endregion
    }

    #region [====== ElementAtFilter<> ======]

    /// <summary>
    /// Represents a filter that transforms a collection into an element of the collection.
    /// </summary>
    public sealed class ElementAtFilter<TValue> : Filter<IEnumerable<TValue>, TValue>
    {
        /// <summary>
        /// Index of the element.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementAtFilter{T}" /> class.
        /// </summary>    
        /// <param name="index">Index of the element.</param>
        public ElementAtFilter(int index)
        {
            Index = index;
        }

        private ElementAtFilter(ElementAtFilter<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Index = constraint.Index;
        }

        private ElementAtFilter(ElementAtFilter<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            Index = constraint.Index;
        }        

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.CollectionConstraints_ElementAt); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<IEnumerable<TValue>, TValue> WithName(Identifier name)
        {
            return new ElementAtFilter<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<IEnumerable<TValue>, TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ElementAtFilter<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IEnumerable<TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<IEnumerable<TValue>>(this, ErrorMessages.CollectionConstraints_NoElementAt)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IEnumerable<TValue> value, out TValue valueOut)
        {
            return value.TryGetElementAt(Index, out valueOut);
        }

        #endregion
    }

    #endregion

    #region [====== ElementAtFilter<,> ======]

    /// <summary>
    /// Represents a filter that transforms a dictionary into an element of the dictionary.
    /// </summary>
    public sealed class ElementAtFilter<TKey, TValue> : Filter<IReadOnlyDictionary<TKey, TValue>, TValue>
    {
        /// <summary>
        /// Key of the element.
        /// </summary>
        public readonly TKey Key;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementAtFilter{T, S}" /> class.
        /// </summary>    
        /// <param name="key">Key of the element.</param>
        public ElementAtFilter(TKey key)
        {
            Key = key;
        }

        private ElementAtFilter(ElementAtFilter<TKey, TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Key = constraint.Key;
        }

        private ElementAtFilter(ElementAtFilter<TKey, TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            Key = constraint.Key;
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.CollectionConstraints_ElementAt_Dictionary); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<IReadOnlyDictionary<TKey, TValue>, TValue> WithName(Identifier name)
        {
            return new ElementAtFilter<TKey, TValue>(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<IReadOnlyDictionary<TKey, TValue>, TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ElementAtFilter<TKey, TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<IReadOnlyDictionary<TKey, TValue>> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<IReadOnlyDictionary<TKey, TValue>>(this, ErrorMessages.CollectionConstraints_NoElementAt_Dictionary)
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
            return value.ContainsKey(Key);
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(IReadOnlyDictionary<TKey, TValue> value, out TValue valueOut)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return value.TryGetValue(Key, out valueOut);
        }

        #endregion
    }

    #endregion
}
