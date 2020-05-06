using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kingo.MicroServices.DataAnnotations
{    
    public sealed class SomeCommand : ValidatableObject
    {        
        [Required]        
        public string PropertyA
        {
            get;
            set;
        }

        [Range(1, 4)]        
        public int PropertyB
        {
            get;
            set;
        }

        public int PropertyC
        {
            get;
            set;
        }
        
        [ChildMember]
        public SomeCommandData PropertyD
        {
            get;
            set;
        }

        [ChildMember(ErrorMessage = "{0} is not valid.")]
        public SomeCommandData PropertyE
        {
            get;
            set;
        }

        [ChildMember(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "ChildMemberIsNotValid")]
        public SomeCommandData PropertyF
        {
            get;
            set;
        }        

        [ChildMember(ErrorMessage = "{0} is not valid.", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "ChildMemberIsNotValid")]
        public SomeCommandData PropertyG
        {
            get;
            set;
        }

        protected override IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (PropertyB >= PropertyC)
            {
                yield return NewValidationError("PropertyC must be greater than PropertyB", nameof(PropertyB), nameof(PropertyC));
            }
        }   
        
        private static ValidationResult NewValidationError(string errorMessage, params string[] memberNames) =>
            new ValidationResult(errorMessage, memberNames);
    }
}
