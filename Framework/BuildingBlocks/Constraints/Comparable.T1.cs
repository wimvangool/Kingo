using System;
using System.Collections.Generic;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class Comparable<TValue> : IEquatable<TValue>, IEquatable<Comparable<TValue>>, IComparable<TValue>, IComparable<Comparable<TValue>>
    {
        private readonly TValue _value;
        private readonly IComparer<TValue> _comparer;

        internal Comparable(TValue value, IComparer<TValue> comparer)
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
            return Equals(obj as Comparable<TValue>);
        }

        public bool Equals(Comparable<TValue> other)
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

        public int CompareTo(Comparable<TValue> other)
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

        public override string ToString()
        {
            if (ReferenceEquals(_value, null))
            {
                return string.Empty;
            }
            return _value.ToString();
        }        
    }
}
