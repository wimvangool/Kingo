using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{T}" />.
    /// </summary>
    public static partial class CollectionConstraints
    {        
        #region [====== TryGetItem ======]

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
        public static bool TryGetItem<TValue>(this IEnumerable<TValue> value, int index, out TValue element)
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

        #endregion
    }

    #region [====== HasItemConstraint ======]

    /// <summary>
    /// Represents a filter that selects an item from a value using a set of indices.
    /// </summary>
    public sealed class HasItemFilter<TValueIn> : Filter<TValueIn, object>
    {
        private readonly IndexList _indexList;        

        /// <summary>
        /// Initializes a new instance of the <see cref="HasItemFilter{T}" /> class.
        /// </summary>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indexList"/> is <c>null</c>.
        /// </exception>        
        public HasItemFilter(IEnumerable<Tuple<Type, object>> indexList)
            : this(new IndexList(indexList)) { }

        internal HasItemFilter(IndexList indices)
        {
            _indexList = indices;
        }

        private HasItemFilter(HasItemFilter<TValueIn> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _indexList = constraint._indexList;            
        }

        private HasItemFilter(HasItemFilter<TValueIn> constraint, Identifier name)
            : base(constraint, name)
        {
            _indexList = constraint._indexList;            
        }

        /// <summary>
        /// Indices of the element to select.
        /// </summary>
        public IReadOnlyList<object> IndexList
        {
            get { return _indexList; }
        }                            

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.CollectionConstraints_HasItem); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<TValueIn, object> WithName(Identifier name)
        {
            return new HasItemFilter<TValueIn>(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<TValueIn, object> WithErrorMessage(StringTemplate errorMessage)
        {
            return new HasItemFilter<TValueIn>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValueIn> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<TValueIn>(this, ErrorMessages.CollectionConstraints_HasNoItem)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValueIn value, out object valueOut)
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException("value");
            }
            try
            {
                valueOut = GetValue(value);
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                valueOut = null;
                return false;
            }
            catch (TargetInvocationException)
            {
                valueOut = null;
                return false;
            }            
        }          
     
        private object GetValue(TValueIn value)
        {
            var array = value as Array;
            if (array != null)
            {
                return array.GetValue(ConvertToArrayIndexValues(_indexList));
            }
            return GetIndexer(value.GetType(), _indexList.Types()).GetValue(value, _indexList.Values().ToArray());
        }

        private static int[] ConvertToArrayIndexValues(IndexList indexList)
        {
            try
            {
                return indexList.Values().Cast<int>().ToArray();
            }
            catch (InvalidCastException exception)
            {
                throw NewInvalidArrayIndexValuesSpecified(indexList, exception);
            }
        }        

        private static PropertyInfo GetIndexer(Type type, IEnumerable<Type> arguments)
        {
            var argumentTypes = arguments.ToArray();
            var defaultMemberAttribute = type.GetCustomAttribute<DefaultMemberAttribute>();
            var indexerName = defaultMemberAttribute == null ? "Item" : defaultMemberAttribute.MemberName;
            var indexer = type.GetProperty(indexerName, argumentTypes);
            if (indexer == null)
            {
                throw NewIndexerNotFoundException(type, argumentTypes);
            }
            return indexer;
        }

        private static Exception NewInvalidArrayIndexValuesSpecified(object indexList, Exception innerException)
        {
            var messageFormat = ExceptionMessages.HasItemFilter_InvalidArrayIndexValues;
            var message = string.Format(messageFormat, indexList);
            return new IndexerInvocationException(message, innerException);
        }

        private static Exception NewIndexerNotFoundException(Type type, IEnumerable<Type> argumentTypes)
        {
            var messageFormat = ExceptionMessages.HasItemFilter_IndexerNotFound;
            var message = string.Format(messageFormat, type, string.Join(", ", argumentTypes.Select(argument => argument.FullName)));
            return new IndexerInvocationException(message, null);
        }

        #endregion                
    }

    #endregion
}
