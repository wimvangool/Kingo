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
        /// <param name="memberName">Name of the member for which the <paramref name="errorMessage"/> was generated.</param>
        /// <param name="errorMessage">An error message.</param>
        void Add(string memberName, ErrorMessage errorMessage);
    }
}
