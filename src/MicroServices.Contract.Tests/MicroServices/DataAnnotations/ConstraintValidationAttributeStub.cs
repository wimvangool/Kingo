using Kingo.Constraints;

namespace Kingo.MicroServices.DataAnnotations
{
    internal sealed class ConstraintValidationAttributeStub : ConstraintValidationAttribute
    {
        private ConstraintValidationAttributeStub(IConstraint constraint)
        {
            Constraint = constraint;
        }

        protected override IConstraint Constraint
        {
            get;
        }

        public static ConstraintValidationAttribute IsAlwaysValid<TValue>() =>
            new ConstraintValidationAttributeStub(Constraints.Constraint.IsAlwaysValid<TValue>());

        public static ConstraintValidationAttribute IsNeverValid<TValue>() =>
            new ConstraintValidationAttributeStub(Constraints.Constraint.IsNeverValid<TValue>());
    }
}
