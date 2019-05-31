namespace Kingo.MicroServices.Contracts
{
    /// <summary>
    /// When implemented by a class, represents a request message that can validate itself.
    /// </summary>    
    public interface IRequestMessage
    {
        /// <summary>
        /// Validates this message and returns all validation errors, if any.
        /// </summary>
        /// <param name="haltOnFirstError">
        /// Indicates whether or not this method should return as soon as the first validation error is found.
        /// </param>
        /// <returns>
        /// A collection of validation errors. If <paramref name="haltOnFirstError"/> is <c>true</c>, the returned
        /// collection contains a minimum of validation errors. If <paramref name="haltOnFirstError" /> is <c>false</c>,
        /// all validation errors are returned.
        /// </returns>
        ErrorInfo Validate(bool haltOnFirstError = false);
    }
}
