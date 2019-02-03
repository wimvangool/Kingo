using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a <see cref="IMessageHandler{T}" />, <see cref="IQuery{T}" /> or <see cref="IQuery{T, S} "/>.
    /// </summary>    
    /// <typeparam name="TResult">Type of the resulting operation.</typeparam>   
    public interface IMessageHandlerOrQuery<TResult> : IAttributeProvider<Type>
    {        
        /// <summary>
        /// Invokes the message handler or query and returns the result.
        /// </summary>        
        /// <returns>The result of the operation.</returns>
        MicroProcessorContext Context
        {
            get;
        }

        /// <summary>
        /// Returns the method of this message handler or query that is to be invoked.
        /// </summary>
        MessageHandlerOrQueryMethod<TResult> Method
        {
            get;
        }
    }
}
