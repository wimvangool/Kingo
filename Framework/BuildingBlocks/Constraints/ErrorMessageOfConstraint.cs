using System;
using System.Collections.Generic;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class ErrorMessageOfConstraint : ErrorMessage
    {
        private readonly IConstraintWithErrorMessage _failedConstraint;
        private readonly object _failedValue;
        private readonly object _errorMessageArgument;

        internal ErrorMessageOfConstraint(IConstraintWithErrorMessage failedConstraint, object failedValue, object errorMessageArgument = null)
        {
            _failedConstraint = failedConstraint;
            _failedValue = failedValue;
            _errorMessageArgument = errorMessageArgument ?? failedConstraint;
        }

        public override IConstraintWithErrorMessage FailedConstraint
        {
            get { return _failedConstraint; }
        }

        public override object FailedValue
        {
            get { return _failedValue; }
        }

        protected override StringTemplate FormatErrorMessage(IEnumerable<KeyValuePair<Identifier, object>> arguments, IFormatProvider formatProvider)
        {            
            return _failedConstraint.ErrorMessage
                .Format(arguments, formatProvider)
                .Format(_failedConstraint.Name, _errorMessageArgument, formatProvider);
        }        
    }
}
