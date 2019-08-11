using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kingo.MicroServices.Contracts
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

        [ChildMember(ErrorMessageResourceType = typeof(SomeCommandErrorMessages), ErrorMessageResourceName = "ChildMemberIsNotValid")]
        public SomeCommandData PropertyF
        {
            get;
            set;
        }        

        [ChildMember(ErrorMessage = "{0} is not valid.", ErrorMessageResourceType = typeof(SomeCommandErrorMessages), ErrorMessageResourceName = "ChildMemberIsNotValid")]
        public SomeCommandData PropertyG
        {
            get;
            set;
        }

        protected override IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (PropertyB >= PropertyC)
            {
                yield return NewValidationError(Guid.NewGuid().ToString(), nameof(PropertyB), nameof(PropertyC));
            }
        }        
    }
}
