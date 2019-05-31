using System;

namespace Kingo.MicroServices.Contracts
{
    /// <summary>
    /// When implemented by a class, represents a reader or consumer of error messages.
    /// </summary>
    public interface IErrorMessageCollection
    {        
        #region [====== Add ======]

        /// <summary>
        /// Adds the specified <paramref name="errorMessageBuilder"/> to this reader.
        /// </summary>        
        /// <param name="errorMessageBuilder">An error message.</param>     
        /// <param name="memberName">
        /// Name of the member for which the <paramref name="errorMessageBuilder"/> was generated. If <c>null</c> or an empty string is specified,
        /// the <paramref name="errorMessageBuilder"/> is associated with the entire message or instance that was validated.
        /// </param>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageBuilder"/> is <c>null</c>.
        /// </exception>              
        void Add(IErrorMessageBuilder errorMessageBuilder, string memberName);

        /// <summary>
        /// Adds the specified <paramref name="errorMessageBuilder"/> to this reader.
        /// </summary>        
        /// <param name="errorMessageBuilder">An error message.</param>     
        /// <param name="memberName">
        /// Name of the member for which the <paramref name="errorMessageBuilder"/> was generated. If <c>null</c> or an empty string is specified,
        /// the <paramref name="errorMessageBuilder"/> is associated with the entire message or instance that was validated.
        /// </param>        
        /// <param name="inheritanceLevel">
        /// The error level of the <paramref name="errorMessageBuilder"/> in relation to the specified <paramref name="memberName"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageBuilder"/> is <c>null</c>.
        /// </exception>  
        void Add(IErrorMessageBuilder errorMessageBuilder, string memberName, ErrorInheritanceLevel inheritanceLevel);

        /// <summary>
        /// Adds the specified <paramref name="errorMessage"/> to this reader.
        /// </summary>        
        /// <param name="errorMessage">An error message.</param>        
        /// <param name="memberName">
        /// Name of the member for which the <paramref name="errorMessage"/> was generated. If <c>null</c> or an empty string is specified,
        /// <paramref name="errorMessage"/> is associated with the entire message or instance that was validated.
        /// </param>             
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>  
        void Add(string errorMessage, string memberName);

        /// <summary>
        /// Adds the specified <paramref name="errorMessage"/> to this reader.
        /// </summary>        
        /// <param name="errorMessage">An error message.</param>        
        /// <param name="memberName">
        /// Name of the member for which the <paramref name="errorMessage"/> was generated. If <c>null</c> or an empty string is specified,
        /// <paramref name="errorMessage"/> is associated with the entire message or instance that was validated.
        /// </param>     
        /// <param name="inheritanceLevel">
        /// The error level of the <paramref name="errorMessage"/> in relation to the specified <paramref name="memberName"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>  
        void Add(string errorMessage, string memberName, ErrorInheritanceLevel inheritanceLevel);        

        #endregion
    }
}
