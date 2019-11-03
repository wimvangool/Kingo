namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the context of a <see cref="IQuery{TRequest, TResponse}"/> operation.
    /// </summary>
    public interface IQueryOperationContext : IMicroProcessorOperationContext
    {
    }
}
