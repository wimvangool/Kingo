using System;

namespace Syztem.ComponentModel.Server
{
	/// <summary>
    /// Contains all configuration settings for a dependency.
	/// </summary>
	[Serializable]
	internal struct DependencyConfiguration : IEquatable<DependencyConfiguration>, IDependencyConfiguration
	{
        /// <summary>
        /// The default configuration that is applied for dependencies.
        /// </summary>
        public static readonly DependencyConfiguration Default = new DependencyConfiguration(InstanceLifetime.PerUnitOfWork);

        private readonly InstanceLifetime _lifetime;

        /// <summary>
        /// Initializes a new instance of a <see cref="DependencyConfiguration" /> structure.
        /// </summary>
        /// <param name="lifetime">The lifetime of the dependency.</param>
		public DependencyConfiguration(InstanceLifetime lifetime)
		{
            _lifetime = lifetime;
		}

	    /// <inheritdoc />
	    public InstanceLifetime Lifetime
	    {
            get { return _lifetime; }
	    }

		#region [====== Object Identity ======]

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is an instance of <see cref="DependencyConfiguration" />
        /// and equals the value of this instance; otherwise, <c>false</c>.
        /// </returns>
		public override bool Equals(object obj)
		{
			if (obj is DependencyConfiguration)
			{
				return Equals((DependencyConfiguration) obj);
			}
			return false;
		}

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="DependencyConfiguration" /> value.
        /// </summary>
        /// <param name="other">A <see cref="DependencyConfiguration" /> value to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> has the same value as this instance; otherwise, <c>false</c>.
        /// </returns>
		public bool Equals(DependencyConfiguration other)
		{
			return _lifetime.Equals(other._lifetime);
		}

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
            return HashCode.Of(_lifetime);
		}

        #endregion        

        #region [====== Formatting and Parsing ======]

        /// <summary>Converts this value to its equivalent string-representation.</summary>
        /// <returns>The string-representation of this value.</returns>
        public override string ToString()
        {
            return _lifetime.ToString();
        }
        
        #endregion                		

		#region [====== Operator Overloads ======]

        /// <summary>Determines whether two specified <see cref="DependencyConfiguration" />-instances have the same value.</summary>
		/// <param name="left">The first instance to compare.</param>
		/// <param name="right">The second instance to compare</param>
		/// <returns><c>true</c> if both instances have the same value; otherwise <c>false</c>.</returns>
		public static bool operator ==(DependencyConfiguration left, DependencyConfiguration right)
		{
			return left.Equals(right);
		}

        /// <summary>Determines whether two specified <see cref="DependencyConfiguration" />-instances do not have the same value.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare</param>
        /// <returns><c>true</c> if both instances do not have the same value; otherwise <c>false</c>.</returns>
		public static bool operator !=(DependencyConfiguration left, DependencyConfiguration right)
		{
			return !left.Equals(right);
		}

		#endregion
	}
}