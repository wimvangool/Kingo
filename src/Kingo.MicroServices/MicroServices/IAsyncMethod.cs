using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the <see cref="IMessageHandler{TMessage}.HandleAsync"/>,
    /// <see cref="IQuery{TResponse}.ExecuteAsync"/> or <see cref="IQuery{TRequest, TResponse}.ExecuteAsync"/>
    /// method of a message handler or query.
    /// </summary>
    public interface IAsyncMethod : IMethodAttributeProvider
    {
        /// <summary>
        /// Returns the message handler or query type this method has been implemented on.
        /// </summary>
        MicroProcessorComponent Component
        {
            get;
        }
    }
}
