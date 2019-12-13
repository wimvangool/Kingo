using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents a collection of validation-errors of a single member.
    /// </summary>
    public interface IMemberErrorCollection : IReadOnlyCollection<string>
    {       
        /// <summary>
        /// Returns the name of the member.
        /// </summary>
        string MemberName
        {
            get;
        }

        /// <summary>
        /// Asserts that any error message on the associated member satisfies the specified
        /// <paramref name="errorMessagePredicate"/>.
        /// </summary>
        /// <param name="errorMessagePredicate">
        /// Predicate that is invoked for every error-message of the associated member until
        /// the predicate is satisfied.
        /// </param>
        /// <param name="message">Optional error-message to show when the assertion fails.</param>
        /// <param name="args">Optional arguments for the specified <paramref name="message"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessagePredicate"/> is <c>null</c>.
        /// </exception>
        void HasError(Func<string, bool> errorMessagePredicate, string message = null, params object[] args);
    }
}