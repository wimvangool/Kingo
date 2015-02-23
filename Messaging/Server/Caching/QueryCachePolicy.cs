namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// Represents a wrapper of a message containing the parameters of a query, in addition to all execution options that specify
    /// how a query should be executed and whether or not it's results may be cached.
    /// </summary>
    [Serializable]
    public struct QueryCachePolicy : IEquatable<QueryCachePolicy>
    {        
        private readonly QueryExecutionOptions _options;
        private readonly TimeSpan? _absoluteExpiration;
        private readonly TimeSpan? _slidingExpiration;        

        /// <summary>
        /// Initializes a new instance of a <see cref="QueryCachePolicy" /> structure.
        /// </summary>        
        /// <param name="options">Specify how the query should be executed.</param>
        /// <param name="absoluteExpiration">Optional timeout value that causes the cached result to expire after a fixed amount of time.</param>
        /// <param name="slidingExpiration">Optional timeout value that causes the cached result to expire after a certain amount of unused time.</param>
        public QueryCachePolicy(QueryExecutionOptions options, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
        {           
            _options = options;
            _absoluteExpiration = absoluteExpiration;
            _slidingExpiration = slidingExpiration;
        }        

        /// <summary>
        /// Indicates whether or not it is allowed to consult the cache for
        /// any previously stored result of the query.
        /// </summary>
        public bool AllowCacheRead
        {
            get { return IsSet(QueryExecutionOptions.AllowCacheRead); }
        }

        /// <summary>
        /// Indicates whether or not it is allowed to store any retrieved result
        /// into the cache after the query has been executed.
        /// </summary>
        public bool AllowCacheWrite
        {
            get { return IsSet(QueryExecutionOptions.AllowCacheWrite); }
        }

        /// <summary>
        /// Optional timeout value that causes the cached result to expire after a fixed amount of time.
        /// </summary>
        public TimeSpan? AbsoluteExpiration
        {
            get { return _absoluteExpiration; }
        }

        /// <summary>
        /// Optional timeout value that causes the cached result to expire after a certain amount of unused time.
        /// </summary>
        public TimeSpan? SlidingExpiration
        {
            get { return _slidingExpiration; }
        }

        private bool IsSet(QueryExecutionOptions option)
        {
            return (_options & option) == option;
        }

        #region [====== Object Identity ======]

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is an instance of <see cref="QueryCachePolicy" />
        /// and equals the value of this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is QueryCachePolicy)
            {
                return Equals((QueryCachePolicy) obj);
            }
            return false;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="QueryCachePolicy" /> value.
        /// </summary>
        /// <param name="other">A <see cref="QueryCachePolicy" /> value to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> has the same value as this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(QueryCachePolicy other)
        {
            return
                _options == other._options &&
                _absoluteExpiration == other._absoluteExpiration &&
                _slidingExpiration == other._slidingExpiration;                
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return _options.GetHashCode() ^ _absoluteExpiration.GetHashCode() ^ _slidingExpiration.GetHashCode();
        }

        #endregion

        #region [====== Operator Overloads ======]

        /// <summary>Determines whether two specified <see cref="QueryCachePolicy" />-instances have the same value.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare</param>
        /// <returns><c>true</c> if both instances have the same value; otherwise <c>false</c>.</returns>
        public static bool operator ==(QueryCachePolicy left, QueryCachePolicy right)
        {
            return left.Equals(right);
        }

        /// <summary>Determines whether two specified <see cref="QueryCachePolicy" />-instances do not have the same value.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare</param>
        /// <returns><c>true</c> if both instances do not have the same value; otherwise <c>false</c>.</returns>
        public static bool operator !=(QueryCachePolicy left, QueryCachePolicy right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}