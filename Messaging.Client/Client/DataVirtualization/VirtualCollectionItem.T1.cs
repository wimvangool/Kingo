using System;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
	/// <summary>
	/// Represents an item of a <see cref="VirtualCollection{T}" />.
	/// </summary>	
	public struct VirtualCollectionItem<T> : IEquatable<VirtualCollectionItem<T>>
	{
        /// <summary>
        /// Returns the item that indicates that the item was not (yet) loaded.
        /// </summary>
        public static readonly VirtualCollectionItem<T>  NotLoadedItem = new VirtualCollectionItem<T>(VirtualCollectionItemStatus.NotLoaded);

		/// <summary>
		/// Returns the item that indicates there was a problem loading the item.
		/// </summary>
		public static readonly VirtualCollectionItem<T> ErrorItem = new VirtualCollectionItem<T>(VirtualCollectionItemStatus.Error);

        private readonly VirtualCollectionItemStatus _status;
		private readonly T _value;        

        /// <summary>
        /// Initializes a new instance of a <see cref="VirtualCollectionItem{T}" /> structure.
        /// </summary>
        /// <param name="value">The loaded item.</param>
		public VirtualCollectionItem(T value)
		{
			_status = VirtualCollectionItemStatus.Loaded;
			_value = value;           
		}

		private VirtualCollectionItem(VirtualCollectionItemStatus status)
		{
            _status = status;
            _value = default(T);
		}

		/// <summary>
		/// Indicates whether or not this item represents an unloaded item.
		/// </summary>
	    public bool IsNotLoaded
	    {
            get { return _status == VirtualCollectionItemStatus.NotLoaded; }
	    }

        /// <summary>
        /// Indicates whether or not this item represents a loaded item.
        /// </summary>
	    public bool IsLoaded
	    {
            get { return _status == VirtualCollectionItemStatus.Loaded; }
	    }

		/// <summary>
		/// Indicates whether or not there was a problem loading this item.
		/// </summary>
	    public bool CouldNotBeLoaded
	    {
            get { return _status == VirtualCollectionItemStatus.Error; }
	    }

		/// <summary>
		/// Returns the value of this item. If <see cref="IsNotLoaded" /> is <c>false</c>, this property will return the default value.
		/// </summary>
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
            return _status == other._status && Equals(_value, other._value);
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