using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Messaging.Resources;

namespace System.ComponentModel.Messaging.Validation
{
    /// <summary>
    /// When applied to a property, indicates that this property must be set to a non-null or non-zero value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class RequiredAttribute : RequestMessageValidationAttribute
    {
        private readonly Dictionary<Type, Func<object, ValidationContext, ValidationResult>> _validationMethods;    
	
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredAttribute" /> class.
        /// </summary>
	    public RequiredAttribute()
	    {
            _validationMethods = new Dictionary<Type, Func<object, ValidationContext, ValidationResult>>()
		    {
			    { typeof(string), (value, context) => IsValidString((string) value, context, StringConstraint) },
			    { typeof(byte), (value, context) => IsValidInteger((byte) value, context) },
			    { typeof(short), (value, context) => IsValidInteger((short) value, context) },
			    { typeof(int), (value, context) => IsValidInteger((int) value, context) },
			    { typeof(long), (value, context) => IsValidInteger((long) value, context) },
			    { typeof(float), (value, context) => IsValidDouble((double) value, context) },
			    { typeof(double), (value, context) => IsValidDouble((long) value, context) },
			    { typeof(decimal), (value, context) => IsValidDecimal((decimal) value, context) },
			    { typeof(Guid), (value, context) => IsValidGuid((Guid) value, context) },
		    };
	    }

        /// <summary>
        /// Indicates which string-values are interpreted as not set.
        /// </summary>
	    public RequiredStringConstraint StringConstraint
	    {
		    get;
		    set;
	    }

        /// <inheritdoc />
	    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
	    {
		    if (value == null)
		    {
			    return NotValid(validationContext);
		    }
		    ValidationResult result;

		    if (TryValidateKnownType(value, validationContext, out result))
		    {
			    return result;
		    }
		    return Valid;
	    }	
	
	    private bool TryValidateKnownType(object value, ValidationContext validationContext, out ValidationResult result)
	    {
		    var valueType = value.GetType();
		
		    if (IsNullable(valueType))
		    {
			    result = Valid;
			    return true;
		    }
		    Func<object, ValidationContext, ValidationResult> validationMethod;
		
		    if (_validationMethods.TryGetValue(valueType, out validationMethod))
		    {
			    result = validationMethod.Invoke(value, validationContext);
			    return true;
		    }
            result = null;
            return false;
	    }
	
        private static bool IsNullable(Type type)
	    {
		    return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
	    }		
	
	    private ValidationResult IsValidString(string value, ValidationContext validationContext, RequiredStringConstraint constraint)
	    {
		    switch (constraint)
		    {
			    case RequiredStringConstraint.NotNull:
				    return Valid;
			
			    case RequiredStringConstraint.NotNullOrEmpty:
				    return value.Length == 0 ? NotValid(validationContext) : Valid;
				
			    case RequiredStringConstraint.NotNullOrWhiteSpace:
				    return string.IsNullOrWhiteSpace(value) ? NotValid(validationContext) : Valid;
				
			    default:
				    throw NewInvalidStringConstraintException(constraint);
		    }
	    }

        private ValidationResult IsValidInteger(long value, ValidationContext validationContext)
	    {            
            return value == 0L ? NotValid(validationContext) : Valid;		    
	    }

        private ValidationResult IsValidDouble(double value, ValidationContext validationContext)
	    {
            return (double.IsNaN(value) || double.IsInfinity(value) || value == 0.0) ? NotValid(validationContext) : Valid;
	    }

        private ValidationResult IsValidDecimal(decimal value, ValidationContext validationContext)
	    {            
            return value == 0.0M ? NotValid(validationContext) : Valid;		    
	    }

        private ValidationResult IsValidGuid(Guid value, ValidationContext validationContext)
	    {
            return value == Guid.Empty ? NotValid(validationContext) : Valid;		    
	    }
	  
        private ValidationResult NotValid(ValidationContext validationContext)
        {
            return NotValid(validationContext, ValidationMessages.Required_MissingRequiredValue);
        }

        private static Exception NewInvalidStringConstraintException(RequiredStringConstraint constraint)
        {
            var messageFormat = ExceptionMessages.RequiredAttribute_InvalidStringConstraint;
            var message = string.Format(messageFormat, constraint);
            return new InvalidOperationException(message);
        }
    }
}
