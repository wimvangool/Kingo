namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// When implemented by a class, represents a producer of <see cref="ErrorMessage">error messages</see>.
    /// </summary>
    public interface IErrorMessageProducer
    {
        /// <summary>
        /// If <paramref name="consumer"/> is not <c>null</c>, produces a set of <see cref="ErrorMessage">error messages</see>
        /// and adds them to the specified <paramref name="consumer"/>.
        /// </summary>
        /// <param name="consumer">A consumer of <see cref="ErrorMessage">error messages</see>.</param>
        /// <returns>The number of error messages that were produced and added to the <paramref name="consumer"/>.</returns>
        int Accept(IErrorMessageConsumer consumer);
    }
}
