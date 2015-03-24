namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// When implemented by a class, represents a consumer of <see cref="ErrorMessage">error messages</see>.
    /// </summary>
    public interface IErrorMessageConsumer
    {
        /// <summary>
        /// Adds an error message to the consumer.
        /// </summary>
        /// <param name="errorMessage">An error message.</param>
        void Add(ErrorMessage errorMessage);
    }
}
