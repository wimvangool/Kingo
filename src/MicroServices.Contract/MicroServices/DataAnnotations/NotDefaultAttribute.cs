using System.ComponentModel.DataAnnotations;
using Kingo.Constraints;

namespace Kingo.MicroServices.DataAnnotations
{
    /// <summary>
    /// Represents a <see cref="ValidationAttribute"/> that verifies that a value is not equal
    /// to its default value.
    /// </summary>
    public sealed class NotDefaultAttribute : ConstraintValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotDefaultAttribute" /> class.
        /// </summary>
        public NotDefaultAttribute() :
            base(ErrorMessages.NotDefaultAttribute_ValueHasDefaultValue)
        {
            Constraint = new NotDefaultConstraint();
        }

        /// <inheritdoc />
        protected override IConstraint Constraint
        {
            get;
        }
    }
}
