using System;
using ServiceComponents.Resources;

namespace ServiceComponents.ComponentModel.Client.DataVirtualization
{
	/// <summary>
	/// Represents an item of a <see cref="VirtualCollection{T}" />.
	/// </summary>	
	public struct VirtualCollectionItem<T> : IEquatable<VirtualCollectionItem<T>>, IVirtualCollectionItem
	{        
        private readonly VirtualCollectionItemStatus _status;
        private readonly int _indexPlusOne;
		private readonly T _value;        

        /// <summary>
        /// Initializes a new instance of a <see cref="VirtualCollectionItem{T}" /> structure.
        /// </summary>
        /// <param name="value">The loaded item.</param>
        /// <param name="index">Index of the item in the collection.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than <c>0</c> or equal to <see cref="int.MaxValue" />.
        /// </exception>
		public VirtualCollectionItem(int index, T value)
		{            
			_status = VirtualCollectionItemStatus.Loaded;
            _indexPlusOne = Increment(index);
			_value = value;           
		}

	    internal VirtualCollectionItem(int index, VirtualCollectionItemStatus status)
		{            
            _status = status;
            _indexPlusOne = Increment(index);
            _value = default(T);
		}

        private static int Increment(int index)
        {
            if (index < 0 || index.Equals(int.MaxValue))
            {
                throw NewInvalidPageIndexException(index);
            }
            return index + 1;
        }

	    /// <inheritdoc />
	    public int Index
	    {
	        get { return _indexPlusOne - 1; }            
	    }

	    /// <inheritdoc />
	    public bool IsNotLoaded
	    {
            get { return _status == VirtualCollectionItemStatus.NotLoaded; }
	    }

	    /// <inheritdoc />
	    public bool IsLoaded
	    {
            get { return _status == VirtualCollectionItemStatus.Loaded; }
	    }

	    /// <inheritdoc />
	    public bool FailedToLoad
	    {
            get { return _status == VirtualCollectionItemStatus.FailedToLoad; }
	    }

	    object IVirtualCollectionItem.Value
	    {
            get { return _value; }
	    }

	    /// <inheritdoc />
	    public T Value
	    {
            get { return _value; }
	    }		

		#region [====== Object Identity ======]

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is an instance of <see cref="VirtualCollectionItem{T}" />
        /// and equals the value of this instance; otherwise, <c>false</c>.
        /// </returns>
		public override bool Equals(object obj)
		{
			if (obj is VirtualCollectionItem<T>)
			{
				return Equals((VirtualCollectionItem<T>) obj);
			}
			return false;
		}

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="VirtualCollectionItem{T}" /> value.
        /// </summary>
        /// <param name="other">A <see cref="VirtualCollectionItem{T}" /> value to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> has the same value as this instance; otherwise, <c>false</c>.
        /// </returns>
		public bool Equals(VirtualCollectionItem<T> other)
		{
            return
                _status == other._status &&
                _indexPlusOne == other._indexPlusOne &&
                Equals(_value, other._value);
		}

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
            return GetType().GetHashCode();
		}

        #endregion        

        #region [====== Formatting and Parsing ======]

        /// <summary>Converts this value to its equivalent string-representation.</summary>
        /// <returns>The string-representation of this value.</returns>
        public override string ToString()
        {            
            var loadedValue = _value as object;
			if (loadedValue != null)
			{
                return loadedValue.ToString();
			}
            return @"<null>";            
        }        

        #endregion                		

        #region [====== Factory Methods ======]        

	    private static Exception NewInvalidPageIndexException(int pageIndex)
	    {
            var messageFormat = ExceptionMessages.Object_IndexOutOfRange;
            var message = string.Format(messageFormat, pageIndex);
            return new ArgumentOutOfRangeException("pageIndex", message);
	    }

	    #endregion

        #region [====== Operator Overloads ======]

        /// <summary>Determines whether two specified <see cref="VirtualCollectionItem{T}" />-instances have the same value.</summary>
		/// <param name="left">The first instance to compare.</param>
		/// <param name="right">The second instance to compare</param>
		/// <returns><c>true</c> if both instances have the same value; otherwise <c>false</c>.</returns>
		public static bool operator ==(VirtualCollectionItem<T> left, VirtualCollectionItem<T> right)
		{
			return left.Equals(right);
		}

        /// <summary>Determines whether two specified <see cref="VirtualCollectionItem{T}" />-instances do not have the same value.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare</param>
        /// <returns><c>true</c> if both instances do not have the same value; otherwise <c>false</c>.</returns>
		public static bool operator !=(VirtualCollectionItem<T> left, VirtualCollectionItem<T> right)
		{
			return !left.Equals(right);
		}

		#endregion
	}
}