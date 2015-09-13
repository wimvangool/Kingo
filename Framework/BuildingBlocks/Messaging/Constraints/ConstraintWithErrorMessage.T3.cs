using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Serves as a base class for all constraints that are associated with a specific error message.
    /// </summary>
    /// <typeparam name="TMessage">Type of a certain message.</typeparam>
    /// <typeparam name="TValue">Type of the value to check.</typeparam>
    /// <typeparam name="TResult">Type of the result the value is converted to.</typeparam>
    public abstract class ConstraintWithErrorMessage<TMessage, TValue, TResult> : Constraint<TMessage, TValue>, IConstraintWithErrorMessage<TMessage, TValue, TResult>
    {                     
        /// <inheritdoc />
        IConstraintWithErrorMessage<TMessage, TValue, TNewResult> IConstraintWithErrorMessage<TMessage, TValue, TResult>.And<TNewResult>(IConstraintWithErrorMessage<TMessage, TResult, TNewResult> constraint)
        {
            return And(constraint);
        }

        internal virtual IConstraintWithErrorMessage<TMessage, TValue, TNewResult> And<TNewResult>(IConstraintWithErrorMessage<TMessage, TResult, TNewResult> constraint)
        {
            return new AndConstraint<TMessage, TValue, TResult, TNewResult>(this, constraint);
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value, TMessage message)
        {            
            TResult result;
            IConstraintWithErrorMessage<TMessage> failedConstraint;

            return IsSatisfiedBy(value, message, out result, out failedConstraint);
        }        

        /// <inheritdoc />
        public abstract bool IsSatisfiedBy(TValue value, TMessage message, out TResult result, out IConstraintWithErrorMessage<TMessage> failedConstraint);
    }
}
