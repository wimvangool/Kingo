namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// When implemented by a class, represents a reader or consumer of error messages.
    /// </summary>
    public interface IErrorMessageReader
    {        
        #region [====== Add ======]

        /// <summary>
        /// Adds the specified <paramref name="errorMessage"/> to this reader.
        /// </summary>        
        /// <param name="errorMessage">An error message.</param>     
        /// <param name="memberName">
        /// Name of the member for which the <paramref name="errorMessage"/> was generated. If <c>null</c> or an empty string is specified,
        /// the <paramref name="errorMessage"/> is associated with the entire message or instance that was validated.
        /// </param>        
        void Add(IErrorMessage errorMessage, string memberName);

        /// <summary>
        /// Adds the specified <paramref name="errorMessage"/> to this reader.
        /// </summary>        
        /// <param name="errorMessage">An error message.</param>        
        /// <param name="memberName">
        /// Name of the member for which the <paramref name="errorMessage"/> was generated. If <c>null</c> or an empty string is specified,
        /// <paramref name="errorMessage"/> is associated with the entire message or instance that was validated.
        /// </param>     
        void Add(string errorMessage, string memberName);

        #endregion
    }
}
