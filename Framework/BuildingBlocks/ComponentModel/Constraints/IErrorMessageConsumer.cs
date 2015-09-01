using System;

namespace Kingo.BuildingBlocks.ComponentModel.Constraints
{
    /// <summary>
    /// When implemented by a class, represents a consumer of error messages.
    /// </summary>
    public interface IErrorMessageConsumer
    {                
        /// <summary>
        /// Adds an error message to the consumer.
        /// </summary>
        /// <param name="memberName">Name of the member for which the <paramref name="errorMessage"/> was generated.</param>
        /// <param name="errorMessage">An error message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> or <paramref name="errorMessage" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An error for the specified <paramref name="memberName"/> has already been added.
        /// </exception>
        void Add(string memberName, string errorMessage);
    }
}
