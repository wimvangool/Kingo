using System;
using System.Globalization;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    internal sealed class TrueConstraint<TMessage, TValue> : ConstraintWithErrorMessage<TMessage, TValue, TValue>
    {
        internal override IConstraintWithErrorMessage<TMessage, TValue, TNewResult> And<TNewResult>(IConstraintWithErrorMessage<TMessage, TValue, TNewResult> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return constraint;
        }

        public override bool IsSatisfiedBy(TValue value, TMessage message, out TValue result, out IConstraintWithErrorMessage<TMessage> failedConstraint)
        {
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException("message");
            }
            result = value;
            failedConstraint = null;
            return true;
        }        
    }
}
