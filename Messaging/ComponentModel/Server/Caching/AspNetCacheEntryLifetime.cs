using System;
using System.ComponentModel;

namespace DefaultNamespace
{
	/// <summary>
	/// Represents ...
	/// </summary>
	[Serializable]
	internal struct AspNetCacheEntryLifetime : IEquatable<AspNetCacheEntryLifetime>
	{
        private readonly DateTimeOffset _expirationTime;
        private readonly TimeSpan _lifetime;
        
		private AspNetCacheEntryLifetime(DateTimeOffset expirationTime, TimeSpan lifetime)
		{
            _expirationTime = expirationTime;
            _lifetime = lifetime;
		}

	    internal bool HasExpired()
	    {
            return _expirationTime < Clock.Current.UtcDateAndTime();
	    }

		#region [====== Object Identity ======]
        
		public override bool Equals(object obj)
		{
			if (obj is AspNetCacheEntryLifetime)
			{
				return Equals((AspNetCacheEntryLifetime) obj);
			}
			return false;
		}
        
		public bool Equals(AspNetCacheEntryLifetime other)
		{
            return _expirationTime.Equals(other._expirationTime) && _lifetime.Equals(other._lifetime);
		}
        
		public override int GetHashCode()
		{
            return _expirationTime.GetHashCode() ^ _lifetime.GetHashCode();
		}

        #endregion        

        #region [====== Factory Methods ======]

		internal AspNetCacheEntryLifetime ExtendLifetime()
		{
		    return new AspNetCacheEntryLifetime(Clock.Current.UtcDateAndTime().Add(_lifetime), _lifetime);
		}

		internal static AspNetCacheEntryLifetime? Start(TimeSpan? lifetime)
		{
            if (lifetime.HasValue)
            {
                return Start(lifetime.Value);
            }
            return null;
		}

		internal static AspNetCacheEntryLifetime Start(TimeSpan lifetime)
		{
		    return new AspNetCacheEntryLifetime(Clock.Current.UtcDateAndTime().Add(lifetime), lifetime);
		}

        #endregion

        #region [====== Operator Overloads ======]

        public static bool operator ==(AspNetCacheEntryLifetime left, AspNetCacheEntryLifetime right)
		{
			return left.Equals(right);
		}
        
		public static bool operator !=(AspNetCacheEntryLifetime left, AspNetCacheEntryLifetime right)
		{
			return !left.Equals(right);
		}

		#endregion
	}
}