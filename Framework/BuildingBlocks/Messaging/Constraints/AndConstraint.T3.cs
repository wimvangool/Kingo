using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    internal sealed class AndConstraint<TValue, TResult, TNewResult> : ConstraintWithErrorMessage<TValue, TNewResult>
    {
        private readonly IConstraintWithErrorMessage<TValue, TResult> _left;
        private readonly IConstraintWithErrorMessage<TResult, TNewResult> _right;

        internal AndConstraint(IConstraintWithErrorMessage<TValue, TResult> left, IConstraintWithErrorMessage<TResult, TNewResult> right)
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

        public override bool IsSatisfiedBy(TValue value, out TNewResult result, out IConstraintWithErrorMessage failedConstraint)
        {
            TResult intermediateResult;

            if (_left.IsSatisfiedBy(value, out intermediateResult, out failedConstraint))
            {
                return _right.IsSatisfiedBy(intermediateResult, out result, out failedConstraint);
            }
            result = default(TNewResult);
            return false;
        }

        public override string ToString(string memberName)
        {
            return string.Format("{0} && {1}", _left.ToString(memberName), _right.ToString(memberName));
        }
    }
}
