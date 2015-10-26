using System;
using System.Collections.Generic;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class ErrorMessageOfOrConstraint : ErrorMessage
    {
        private readonly IConstraintWithErrorMessage _failedConstraint;
        private readonly object _failedValue;
        private readonly IConstraintWithErrorMessage[] _failedChildConstraints;

        internal ErrorMessageOfOrConstraint(IConstraintWithErrorMessage failedConstraint, object failedValue, IConstraintWithErrorMessage[] failedChildConstraints)
        {
            _failedConstraint = failedConstraint;
            _failedValue = failedValue;
            _failedChildConstraints = failedChildConstraints;
        }

        public override IConstraintWithErrorMessage FailedConstraint
        {
            get { return _failedConstraint;}
        }

        public override object FailedValue
        {
            get { return _failedValue; }
        }

        protected override StringTemplate FormatErrorMessage(IEnumerable<KeyValuePair<Identifier, object>> arguments, IFormatProvider formatProvider)
        {
            var errorMessage = _failedConstraint.ErrorMessage.Format(arguments, formatProvider);

            foreach (var constraint in _failedChildConstraints)
            {
                errorMessage = errorMessage.Format(constraint.Name, constraint, formatProvider);
            }
            return errorMessage;
        }
    }
}
