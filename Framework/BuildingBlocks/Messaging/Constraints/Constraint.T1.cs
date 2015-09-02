using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Serves as a base class for all <see cref="IConstraint{T}" /> implementations.
    /// </summary>
    /// <typeparam name="TValue">Type of the value this constraint is for.</typeparam>    
    public abstract class Constraint<TValue> : IConstraint<TValue>
    {                                            
        /// <inheritdoc />
        public bool IsSatisfiedBy(Func<TValue> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            return IsSatisfiedBy(valueFactory.Invoke());
        }

        /// <inheritdoc />
        public abstract bool IsSatisfiedBy(TValue value);

        /// <inheritdoc />
        public override string ToString()
        {
            return ToString("x");
        }

        /// <inheritdoc />
        public abstract string ToString(string memberName);               
    }
}
