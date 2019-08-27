using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, a method that handles a specific message and is provided with
    /// a context for processing this message.
    /// </summary>
    public interface IAsyncMethod : IMethodAttributeProvider
    {
        /// <summary>
        /// Returns the message handler or query type this method has been implemented on.
        /// </summary>
        ITypeAttributeProvider Component
        {
            get;
        }

        /// <summary>
        /// Returns the parameter that represents the message to be handled.
        /// </summary>
        IParameterAttributeProvider MessageParameter
        {
            get;
        }

        /// <summary>
        /// Returns the parameter that represents the context that is supplied to the method.
        /// </summary>
        IParameterAttributeProvider ContextParameter
        {
            get;
        }
    }
}
