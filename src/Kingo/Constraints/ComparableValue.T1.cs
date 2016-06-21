using System;
using System.Collections.Generic;

namespace Kingo.Constraints
{
    internal sealed class ComparableValue<TValue> : IEquatable<ComparableValue<TValue>>,
                                                    IEquatable<TValue>,
                                                    IComparable<ComparableValue<TValue>>,
                                                    IComparable<TValue>,
                                                    IComparable
    {
        private readonly TValue _value;
        private readonly IComparer<TValue> _comparer;

        internal ComparableValue(TValue value, IComparer<TValue> comparer)
        {
            _value = value;
            _comparer = Comparer.EnsureComparer(comparer);
        }

        internal TValue Value
        {
            get { return _value; }
        }

        #region [====== Equals & GetHashCode ======]

        public override bool Equals(object obj)
        {            
            return Equals(obj as ComparableValue<TValue>);
        }

        public bool Equals(ComparableValue<TValue> other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return Equals(other._value);
        }

        public bool Equals(TValue other)
        {
            return Equals(_value, other);
        }        

        public override int GetHashCode()
        {
            return ReferenceEquals(_value, null) ? 0 : _value.GetHashCode();
        }

        #endregion

        #region [====== Compare ======]

        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return Comparable.Greater;
            }
            var comparable = obj as ComparableValue<TValue>;
            if (comparable != null)
            {
                return CompareTo(comparable);
            }
            if (obj is TValue)
            {
                return CompareTo((TValue) obj);
            }
            throw Comparable.NewUnexpectedTypeException(GetType(), obj.GetType());
        }

        public int CompareTo(ComparableValue<TValue> other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }
            return CompareTo(other._value);
        }

        public int CompareTo(TValue other)
        {
            return _comparer.Compare(_value, other);
        }

        #endregion

        #region [====== Conversion ======]

        public override string ToString()
        {
            return ReferenceEquals(_value, null) ? StringTemplate.NullValue : _value.ToString();            
        }

        #endregion
    }
}
