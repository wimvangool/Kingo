using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kingo.MicroServices.DataAnnotations
{
    /// <summary>
    /// Serves as a base-class for all request-messages such as commands or query-requests.
    /// </summary>
    [Serializable]
    public abstract class RequestMessage : Message, IValidatableObject
    {
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext) =>
            Validate(validationContext).Concat(ChildMember.ValidateChildMembers(validationContext)).ErrorsOnly();

        /// <summary>
        /// Validates all constraints that are not declared as a <see cref="ValidationAttribute" />.
        /// </summary>
        /// <param name="validationContext">The context of this validation-operation.</param>
        /// <returns>A collection of validation-errors.</returns>
        protected virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
            Enumerable.Empty<ValidationResult>();
    }
}
