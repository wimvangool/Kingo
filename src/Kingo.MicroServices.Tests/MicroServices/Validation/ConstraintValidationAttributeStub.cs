﻿using System;

namespace Kingo.MicroServices.Validation
{
    internal sealed class ConstraintValidationAttributeStub : ConstraintValidationAttribute
    {
        public ConstraintValidationAttributeStub(IConstraint constraint, bool isRequired = false)
        {
            Constraint = constraint ?? throw new ArgumentNullException(nameof(constraint));
            IsRequired = isRequired;
        }

        protected override IConstraint Constraint
        {
            get;
        }
    }
}
