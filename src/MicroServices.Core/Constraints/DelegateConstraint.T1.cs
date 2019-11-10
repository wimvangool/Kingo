using System;

namespace Kingo.Constraints
{
    internal sealed class DelegateConstraint<TValue> : Constraint<TValue>
    {
        private readonly Func<TValue, bool> _constraint;

        public DelegateConstraint(Func<TValue, bool> constraint)
        {
            _constraint = constraint ?? throw new ArgumentNullException(nameof(constraint));
        }

        public override bool IsValid(TValue value) =>
            _constraint.Invoke(value);
    }
}
