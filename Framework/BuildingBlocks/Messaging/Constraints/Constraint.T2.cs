using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Serves as a base class for all <see cref="IConstraint{TMessage, TValue}" /> implementations.
    /// </summary>
    /// <typeparam name="TMessage">Type of a certain message.</typeparam>
    /// <typeparam name="TValue">Type of the value this constraint is for.</typeparam>    
    public abstract class Constraint<TMessage, TValue> : IConstraint<TMessage, TValue>
    {                                                    
        /// <inheritdoc />
        public abstract bool IsSatisfiedBy(TValue value, TMessage message);                              
    }
}
