using System.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a method that handles a specific message and is provided with
    /// a context for processing this message.
    /// </summary>
    public interface IAsyncMethod
    {
        /// <summary>
        /// Returns the message handler or query type this method has been implemented on.
        /// </summary>
        MicroProcessorComponent Component
        {
            get;
        }

        /// <summary>
        /// Returns the <see cref="System.Reflection.MethodInfo"/> of the method.
        /// </summary>
        MethodInfo MethodInfo
        {
            get;
        }

        /// <summary>
        /// Returns the <see cref="ParameterInfo" /> that represents the message to be handled.
        /// </summary>
        ParameterInfo MessageParameterInfo
        {
            get;
        }

        /// <summary>
        /// Returns the <see cref="ParameterInfo" /> that represents the context that is supplied to the method.
        /// </summary>
        ParameterInfo ContextParameterInfo
        {
            get;
        }
    }
}
