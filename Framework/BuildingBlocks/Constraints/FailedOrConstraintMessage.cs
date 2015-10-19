using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class FailedOrConstraintMessage : ErrorMessage
    {
        private readonly IConstraintWithErrorMessage _failedConstraint;
        private readonly IConstraintWithErrorMessage[] _failedChildConstraints;

        internal FailedOrConstraintMessage(IConstraintWithErrorMessage failedConstraint, IConstraintWithErrorMessage[] failedChildConstraints)
        {
            _failedConstraint = failedConstraint;
            _failedChildConstraints = failedChildConstraints;
        }

        public override IConstraintWithErrorMessage FailedConstraint
        {
            get { return _failedConstraint;}
        }

        protected override StringTemplate FormatErrorMessage(IFormatProvider formatProvider)
        {
            var errorMessage = _failedConstraint.ErrorMessage;

            foreach (var constraint in _failedChildConstraints)
            {
                errorMessage = errorMessage.Format(constraint.Name, constraint, formatProvider);
            }
            return errorMessage;
        }
    }
}
