using System;
using System.Collections.Generic;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class FailedConstraintMessage : ErrorMessage
    {
        private readonly IConstraintWithErrorMessage _failedConstraint;
        private readonly object _errorMessageArgument;

        internal FailedConstraintMessage(IConstraintWithErrorMessage failedConstraint, object errorMessageArgument = null)
        {
            _failedConstraint = failedConstraint;
            _errorMessageArgument = errorMessageArgument ?? failedConstraint;
        }

        public override IConstraintWithErrorMessage FailedConstraint
        {
            get { return _failedConstraint; }
        }        

        protected override StringTemplate FormatErrorMessage(IEnumerable<KeyValuePair<Identifier, object>> arguments, IFormatProvider formatProvider)
        {            
            return _failedConstraint.ErrorMessage
                .Format(arguments, formatProvider)
                .Format(_failedConstraint.Name, _errorMessageArgument, formatProvider);
        }        
    }
}
