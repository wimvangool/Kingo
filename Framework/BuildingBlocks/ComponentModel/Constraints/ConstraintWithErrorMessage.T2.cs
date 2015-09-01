using System;

namespace Kingo.BuildingBlocks.ComponentModel.Constraints
{
    /// <summary>
    /// Serves as a base class for all constraints that are associated with a specific error message.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to check.</typeparam>
    /// <typeparam name="TResult">Type of the result the value is converted to.</typeparam>
    public abstract class ConstraintWithErrorMessage<TValue, TResult> : Constraint<TValue>, IConstraintWithErrorMessage<TValue, TResult>
    {                     
        /// <inheritdoc />
        IConstraintWithErrorMessage<TValue, TNewResult> IConstraintWithErrorMessage<TValue, TResult>.And<TNewResult>(IConstraintWithErrorMessage<TResult, TNewResult> constraint)
        {
            return And(constraint);
        }

        internal virtual IConstraintWithErrorMessage<TValue, TNewResult> And<TNewResult>(IConstraintWithErrorMessage<TResult, TNewResult> constraint)
        {
            return new AndConstraint<TValue, TResult, TNewResult>(this, constraint);
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            TResult result;
            IConstraintWithErrorMessage failedConstraint;

            return IsSatisfiedBy(value, out result, out failedConstraint);
        }

        /// <inheritdoc />
        public bool IsSatisfiedBy(Func<TValue> valueFactory, out TResult result, out IConstraintWithErrorMessage failedConstraint)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            return IsSatisfiedBy(valueFactory.Invoke(), out result, out failedConstraint);
        }

        /// <inheritdoc />
        public abstract bool IsSatisfiedBy(TValue value, out TResult result, out IConstraintWithErrorMessage failedConstraint);
    }
}
