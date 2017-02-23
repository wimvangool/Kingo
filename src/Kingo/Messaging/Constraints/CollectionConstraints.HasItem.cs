using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kingo.DynamicMethods;
using Kingo.Resources;

namespace Kingo.Messaging.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{T}" />.
    /// </summary>
    public static partial class CollectionConstraints
    {
        #region [====== TryGetIndexer ======]

        private sealed class IndexerSignatureComparer : IEqualityComparer<Tuple<Type, Type[]>>
        {
            public bool Equals(Tuple<Type, Type[]> x, Tuple<Type, Type[]> y)
            {
                if (x.Item1 != y.Item1 || x.Item2.Length != y.Item2.Length)
                {
                    return false;
                }
                for (int index = 0; index < x.Item2.Length; index++)
                {
                    if (x.Item2[index] != y.Item2[index])
                    {
                        return false;
                    }
                }
                return true;
            }

            public int GetHashCode(Tuple<Type, Type[]> obj)
            {
                return GetHashCodeMethod.Invoke(obj.Item1);
            }
        }

        private static readonly ConcurrentDictionary<Tuple<Type, Type[]>, PropertyInfo> _Indexers =
            new ConcurrentDictionary<Tuple<Type, Type[]>, PropertyInfo>(new IndexerSignatureComparer());

        internal static bool TryGetIndexer(Type type, IEnumerable<Type> arguments, out PropertyInfo indexer)
        {
            var signature = new Tuple<Type, Type[]>(type, arguments.ToArray());
            
            return (indexer = _Indexers.GetOrAdd(signature, GetIndexer)) != null;            
        }

        private static PropertyInfo GetIndexer(Tuple<Type, Type[]> signature)
        {
            var instanceType = signature.Item1;
            var argumentTypes = signature.Item2;
            var defaultMemberAttribute = instanceType.GetCustomAttribute<DefaultMemberAttribute>();
            var indexerName = defaultMemberAttribute == null ? "Item" : defaultMemberAttribute.MemberName;

            return instanceType.GetProperty(indexerName, argumentTypes);
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
                throw new ArgumentNullException(nameof(value));
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

        internal static PropertyInfo GetIndexer(Type type, IEnumerable<Type> arguments)
        {
            PropertyInfo indexer;

            if (CollectionConstraints.TryGetIndexer(type, arguments, out indexer))
            {
                return indexer;
            }
            throw NewIndexerNotFoundException(type, arguments);
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
