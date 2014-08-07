using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Messaging.Resources;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    /// <summary>
    /// Represents a range of indices that mark which items of a <see cref="VirtualCollection{T}" /> have failed to load.
    /// </summary>
    [Serializable]
    internal struct IndexRange : IEquatable<IndexRange>
    {
        // We store the count as [count - 1], since the default constructor will
        // initialize it as 0, which is not a valid value for count.
        private readonly int _index;
        private readonly int _countMinusOne;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexRange" /> class.
        /// </summary>
        /// <param name="index">The index that is stored in this range.</param>
        public IndexRange(int index)
            : this(index, 1) { }

        /// <summary>
        /// Initializes a new instance of a <see cref="IndexRange" /> structure.
        /// </summary>
        /// <param name="index">The first index that is stored in this range.</param>
        /// <param name="count">The amount of indices that are stored in this range.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative or <paramref name="count"/> is less than or equal to zero.
        /// </exception>
        public IndexRange(int index, int count)
        {
            if (index < 0)
            {
                throw NewIndexOutOfRangeException(index);
            }
            if (count <= 0)
            {
                throw NewCountOutOfRangException(count);
            }
            _index = index;
            _countMinusOne = count - 1;
        }

        public int MinValue
        {
            get { return _index; }
        }

        public int MaxValue
        {
            get { return _index + _countMinusOne; }
        }

        public int Count
        {
            get { return _countMinusOne + 1; }
        }

        public bool Contains(int index)
        {
            return MinValue <= index && index <= MaxValue;
        }

        public bool IsLeftAdjacentTo(int index)
        {
            return index == MinValue - 1;
        }

        public bool IsRightAdjacentTo(int index)
        {
            return index == MaxValue + 1;
        }

        public bool IsRightAdjacentTo(IndexRange range)
        {
            return IsRightAdjacentTo(range.MinValue);
        }

        public IndexRange AddToLeft()
        {
            return new IndexRange(_index - 1, _countMinusOne + 1);
        }

        public IndexRange AddToRight()
        {
            return new IndexRange(_index, _countMinusOne + 1);
        }

        public IndexRange AddToRight(int count)
        {
            if (count <= 0)
            {
                throw NewCountOutOfRangException(count);
            }
            return new IndexRange(_index, _countMinusOne + count);
        }

        public IndexRange RemoveFromLeft()
        {
            return new IndexRange(_index + 1, _countMinusOne - 1);
        }

        public IndexRange RemoveFromRight()
        {
            return new IndexRange(_index, _countMinusOne - 1);
        }

        #region [====== Object Identity ======]

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is an instance of <see cref="IndexRange" />
        /// and equals the value of this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is IndexRange)
            {
                return Equals((IndexRange) obj);
            }
            return false;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="IndexRange" /> value.
        /// </summary>
        /// <param name="other">A <see cref="IndexRange" /> value to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> has the same value as this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(IndexRange other)
        {
            return _index.Equals(other._index) && _countMinusOne.Equals(other._countMinusOne);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return _index ^ _countMinusOne;
        }

        #endregion

        #region [====== Formatting and Parsing ======]

        /// <summary>Converts this value to its equivalent string-representation.</summary>
        /// <returns>The string-representation of this value.</returns>
        public override string ToString()
        {
            if (_countMinusOne == 0)
            {
                return string.Format("[{0}]", _index);
            }
            return string.Format("[{0}, {1}]", _index, _index + _countMinusOne);
        }        

        #endregion        

        #region [====== Exception Factory Methods ======]

        private static Exception NewIndexOutOfRangeException(int index)
        {
            var messageFormat = ExceptionMessages.Object_IndexOutOfRange;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }

        private static Exception NewCountOutOfRangException(int count)
        {
            var messageFormat = ExceptionMessages.ErrorItemRange_InvalidCount;
            var message = string.Format(messageFormat, count);
            return new ArgumentOutOfRangeException("count", message);
        }

        #endregion

        #region [====== Operator Overloads ======]

        /// <summary>Determines whether two specified <see cref="IndexRange" />-instances have the same value.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare</param>
        /// <returns><c>true</c> if both instances have the same value; otherwise <c>false</c>.</returns>
        public static bool operator ==(IndexRange left, IndexRange right)
        {
            return left.Equals(right);
        }

        /// <summary>Determines whether two specified <see cref="IndexRange" />-instances do not have the same value.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare</param>
        /// <returns><c>true</c> if both instances do not have the same value; otherwise <c>false</c>.</returns>
        public static bool operator !=(IndexRange left, IndexRange right)
        {
            return !left.Equals(right);
        }

        #endregion        
    
        internal bool TryAdd(int index, out IndexRange extendedRange)
        {
            throw new NotImplementedException();
        }
    }
}