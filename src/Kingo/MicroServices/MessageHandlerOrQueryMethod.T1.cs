using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the method of a message handler or query that 
    /// </summary>
    /// <typeparam name="TResult">Type of the result of this method.</typeparam>
    public abstract class MessageHandlerOrQueryMethod<TResult> : IAttributeProvider<MethodInfo>
    {
        MethodInfo IAttributeProvider<MethodInfo>.Target =>
            Info;

        /// <summary>
        /// Returns the <see cref="MethodInfo"/> of the method of this message handler or query that is invoked
        /// when calling <see cref="InvokeAsync"/>.
        /// </summary>
        public abstract MethodInfo Info
        {
            get;
        }

        /// <inheritdoc />
        public abstract bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class;

        /// <inheritdoc />
        public abstract IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() where TAttribute : class;

        /// <summary>
        /// Invokes the (method of) the associated message handler or query.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public abstract Task<InvokeAsyncResult<TResult>> InvokeAsync();
    }
}
