using System;
using System.ComponentModel.DataAnnotations;
using Kingo.Constraints;

namespace Kingo.MicroServices.DataAnnotations
{
    /// <summary>
    /// Represents a validation-attribute that is implemented through a set of <see cref="IConstraint">constraints</see>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public abstract class ConstraintValidationAttribute : ValidationAttribute
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintValidationAttribute" /> class.
        /// </summary>
        /// <param name="errorMessage">If specified, represents the (default) error message of this attribute.</param>
        protected ConstraintValidationAttribute(string errorMessage = null) :
            base(errorMessage) { }

        /// <summary>
        /// When implemented by a class, returns the constraint
        /// that will carry out the validation of (non-null) values.
        /// </summary>
        protected abstract IConstraint Constraint
        {
            get;
        }

        /// <inheritdoc />
        public override bool IsValid(object value) =>
            value == null || Constraint.IsValid(value);
    }
}
