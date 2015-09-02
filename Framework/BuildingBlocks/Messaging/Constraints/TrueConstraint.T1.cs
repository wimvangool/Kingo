using System;
using System.Globalization;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    internal sealed class TrueConstraint<TValue> : ConstraintWithErrorMessage<TValue, TValue>
    {
        internal override IConstraintWithErrorMessage<TValue, TNewResult> And<TNewResult>(IConstraintWithErrorMessage<TValue, TNewResult> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return constraint;
        }

        public override bool IsSatisfiedBy(TValue value, out TValue result, out IConstraintWithErrorMessage failedConstraint)
        {
            result = value;
            failedConstraint = null;
            return true;
        }

        public override string ToString(string memberName)
        {
            return true.ToString(CultureInfo.CurrentCulture).ToLower(CultureInfo.CurrentCulture);
        }
    }
}
