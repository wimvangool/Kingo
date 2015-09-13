using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a constraint over a certain <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TMessage">Type of a certain message.</typeparam>
    /// <typeparam name="TValue">Type of the value this constraint is for.</typeparam>
    public interface IConstraint<in TMessage, in TValue>
    {                
        /// <summary>
        /// Determines whether or not this constraint is satisfied by the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value of the member to check.</param>
        /// <param name="message">Message the member belongs to.</param>
        /// <returns>
        /// <c>true</c> if this constraint was satisfied by the specified <paramref name="value"/>; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        bool IsSatisfiedBy(TValue value, TMessage message);        
    }
}
