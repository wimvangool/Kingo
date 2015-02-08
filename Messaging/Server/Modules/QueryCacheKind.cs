namespace System.ComponentModel.Server.Modules
{
    /// <summary>
    /// Represents a certain kind of cache that is used to store the results of <see cref="IQuery{T, S}">Queries</see>.
    /// </summary>
    public enum QueryCacheKind
    {        
        /// <summary>
        /// Indicates that the result should be stored in global application cache.
        /// </summary>
        Application,

        /// <summary>
        /// Indicates that the result should be stored in session/user specific cache.
        /// </summary>
        Session
    }
}
