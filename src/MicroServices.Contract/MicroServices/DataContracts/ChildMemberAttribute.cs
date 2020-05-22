using System;
using System.ComponentModel.DataAnnotations;

namespace Kingo.MicroServices.DataContracts
{
    /// <summary>
    /// This attribute can be used to decorate fields or properties of a <see cref="ValidatableObject" />
    /// to signal that it represents an item, collection or dictionary that should be validated in
    /// and of itself. Any validation-errors of the child-member will be added to the set of validation-errors
    /// of the parent.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ChildMemberAttribute : Attribute
    {
        #region [====== ValidationErrorFactory ======]

        // The ValidationErrorFactory represents a stub-implementation of the ValidationAttribute class
        // that deems every value as invalid, so that it can be used to obtain a validation-error for
        // the child-member when required.
        private sealed class ValidationErrorFactory : ValidationAttribute
        {            
            public override bool IsValid(object value) =>
                false;
        }

        #endregion

        private readonly ValidationErrorFactory _validationErrorFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildMemberAttribute" /> class.
        /// </summary>
        public ChildMemberAttribute()
        {
            _validationErrorFactory = new ValidationErrorFactory();                          
        }

        /// <summary>
        /// If specified, defines the error-message that is added to the collection of validation-results
        /// if any child-members are invalid. This property cannot be used in combination with the
        /// <see cref="ErrorMessageResourceType"/> and <see cref="ErrorMessageResourceName"/> properties.
        /// </summary>
        public string ErrorMessage
        {
            get => _validationErrorFactory.ErrorMessage;
            set => _validationErrorFactory.ErrorMessage = value;
        }

        /// <summary>
        /// If specified, defines the resource-type where the localized error-message can be retrieved
        /// that is added to the collection of validation-results if any child-members are invalid.
        /// This property should be used in combination with the <see cref="ErrorMessageResourceName"/>-property.
        /// </summary>
        public Type ErrorMessageResourceType
        {
            get => _validationErrorFactory.ErrorMessageResourceType;
            set => _validationErrorFactory.ErrorMessageResourceType = value;
        }

        /// <summary>
        /// If specified, defines the resource-name of the localized error-message inside the resource-type
        /// that is added to the collection of validation-results if any child-members are invalid.
        /// This property should be used in combination with the <see cref="ErrorMessageResourceType"/>-property.
        /// </summary>
        public string ErrorMessageResourceName
        {
            get => _validationErrorFactory.ErrorMessageResourceName;
            set => _validationErrorFactory.ErrorMessageResourceName = value;
        }
        
        internal bool TryGetValidationError(ValidationContext validationContext, out ValidationResult result)
        {
            if (ErrorMessage == null && ErrorMessageResourceType == null)
            {
                result = null;
                return false;
            }
            result = _validationErrorFactory.GetValidationResult(validationContext.ObjectInstance, validationContext);
            return true;
        }
    }
}
