using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// When implemented by a class, represents a reader or consumer of error messages.
    /// </summary>
    public interface IErrorMessageReader
    {
        #region [====== Put ======]

        /// <summary>
        /// Sets the error message for the entire object.
        /// </summary>
        /// <param name="errorMessage">An error message.</param>
        void Put(IErrorMessage errorMessage);

        /// <summary>
        /// Sets the error message for the entire object.
        /// </summary>
        /// <param name="errorMessage">An error message.</param>
        void Put(string errorMessage);

        #endregion

        #region [====== Add ======]

        /// <summary>
        /// Adds an error message to the reader.
        /// </summary>
        /// <param name="memberName">Name of the member for which the <paramref name="errorMessage"/> was generated.</param>
        /// <param name="errorMessage">An error message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> is <c>null</c>.
        /// </exception>        
        void Add(string memberName, IErrorMessage errorMessage);

        /// <summary>
        /// Adds an error message to the reader.
        /// </summary>
        /// <param name="memberName">Name of the member for which the <paramref name="errorMessage"/> was generated.</param>
        /// <param name="errorMessage">An error message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> is <c>null</c>.
        /// </exception>        
        void Add(string memberName, string errorMessage);

        #endregion
    }
}
