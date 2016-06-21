namespace Kingo
{
    /// <summary>
    /// When implemented by a class, represents a validator for a specific instance.
    /// </summary>
    public interface IValidator<in T> : IValidator where T : class
    {
        /// <summary>
        /// Validates all values of the specified <paramref name="instance"/> and returns all the validation-errors, if any.
        /// </summary>   
        /// <param name="instance">The instance to validate.</param>             
        /// <returns>
        /// A <see cref="ErrorInfo" /> instance that contains all validation errors, if any. If <paramref name="instance"/> is
        /// <c>null</c>, an empty <see cref="ErrorInfo" /> instance is returned.
        /// </returns>         
        ErrorInfo Validate(T instance);        
    }
}
