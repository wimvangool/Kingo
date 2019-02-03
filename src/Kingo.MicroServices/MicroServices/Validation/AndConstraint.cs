using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kingo.MicroServices.Validation
{
    internal sealed class AndConstraint : CompositeConstraint
    {
        public AndConstraint(params IConstraint[] constraints) :
            base(constraints) { }

        public AndConstraint(IEnumerable<IConstraint> constraints) :
            base(constraints) { }

        private AndConstraint(IEnumerable<IConstraint> constraints, params IConstraint[] addedConstraints) :
            base(constraints.Concat(addedConstraints)) { }

        #region [====== Logical Operations ======]

        /// <inheritdoc />
        public override IConstraint And(IConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }
            if (constraint == this)
            {
                return constraint;
            }
            return new AndConstraint(Constraints, constraint);
        }

        #endregion

        #region [====== Validation ======]

        public override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            foreach (var constraint in Constraints)
            {
                if (constraint.IsNotValid(value, validationContext, out var result))
                {
                    return result;
                }
            }
            return ValidationResult.Success;
        }

        #endregion
    }
}
