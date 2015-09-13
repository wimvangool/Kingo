using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    internal sealed class AndConstraint<TMessage, TValue, TResult, TNewResult> : ConstraintWithErrorMessage<TMessage, TValue, TNewResult>
    {
        private readonly IConstraintWithErrorMessage<TMessage, TValue, TResult> _left;
        private readonly IConstraintWithErrorMessage<TMessage, TResult, TNewResult> _right;

        internal AndConstraint(IConstraintWithErrorMessage<TMessage, TValue, TResult> left, IConstraintWithErrorMessage<TMessage, TResult, TNewResult> right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            _left = left;
            _right = right;
        }        

        public override bool IsSatisfiedBy(TValue value, TMessage message, out TNewResult result, out IConstraintWithErrorMessage<TMessage> failedConstraint)
        {
            TResult intermediateResult;

            if (_left.IsSatisfiedBy(value, message, out intermediateResult, out failedConstraint))
            {
                return _right.IsSatisfiedBy(intermediateResult, message, out result, out failedConstraint);
            }
            result = default(TNewResult);
            return false;
        }       
    }
}
