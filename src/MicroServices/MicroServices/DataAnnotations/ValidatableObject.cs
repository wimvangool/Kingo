using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kingo.MicroServices.DataAnnotations
{
    /// <summary>
    /// Represents a data contract that can be validated.
    /// </summary>
    [Serializable]    
    public abstract class ValidatableObject : DataContract, IValidatableObject
    {        
        #region [====== Validate ======]

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext) =>
            Validate(validationContext).Concat(ValidateChildMembers(validationContext)).ErrorsOnly();

        /// <summary>
        /// Validates all constraints that are not declared as a <see cref="ValidationAttribute" />.
        /// </summary>
        /// <param name="validationContext">The context of this validation-operation.</param>
        /// <returns>A collection of validation-errors.</returns>
        protected virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
            Enumerable.Empty<ValidationResult>();
        
        /// <summary>
        /// Validates all the constraints of child members. 
        /// </summary>
        /// <param name="validationContext">The context of this validation-operation.</param>
        /// <returns>A collection of validation-errors, if any.</returns>
        protected virtual IEnumerable<ValidationResult> ValidateChildMembers(ValidationContext validationContext) =>
            ChildMember.ValidateChildMembers(validationContext);

        /// <summary>
        /// Creates and returns a new <see cref="ValidationResult" /> with a specific error message.
        /// </summary>
        /// <param name="errorMessage">Error message describing the validation error.</param>
        /// <param name="memberNames">Names of the members that are considered invalid.</param>
        /// <returns>A new <see cref="ValidationResult"/> that represents the validation error.</returns>
        protected static ValidationResult NewValidationError(string errorMessage, params string[] memberNames) =>
            new ValidationResult(errorMessage, memberNames);

        #endregion        
    }
}
