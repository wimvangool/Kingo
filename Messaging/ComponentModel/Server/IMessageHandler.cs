using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// When implemented by a class, represents a <see cref="IMessageHandler{TMessage}" /> that is
    /// ready to be invoked with a specific <see cref="Message" />.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// The message that will be passed to the <see cref="IMessageHandler{TMessage}" />.
        /// </summary>
        IMessage Message
        {
            get;
        }

        /// <summary>
        /// Attempts to retrieve a single attribute of type <typeparamref name="TAttribute"/> defined on the class of this handler.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <param name="attribute">
        /// When this method returns <c>true</c>, this parameter will refer to the retrieved attribute;
        /// otherwise it will be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the attribute was retrieved; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Multiple attribute defined of the class are assignable to type <typeparamref name="TAttribute"/>.
        /// </exception>
        bool TryGetClassAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class;

        /// <summary>
        /// Attempts to retrieve a single attribute of type <typeparamref name="TAttribute"/> defined on the method on this handler.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <param name="attribute">
        /// When this method returns <c>true</c>, this parameter will refer to the retrieved attribute;
        /// otherwise it will be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the attribute was retrieved; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Multiple attribute defined of the method are assignable to type <typeparamref name="TAttribute"/>.
        /// </exception>
        bool TryGetMethodAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class;

        /// <summary>
        /// Retrieves all attributes of type <typeparamref name="TAttribute"/> that are declared on the class of this handler.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <returns>A collection of attributes.</returns>
        IEnumerable<TAttribute> GetClassAttributesOfType<TAttribute>() where TAttribute : class;

        /// <summary>
        /// Retrieves all attributes of type <typeparamref name="TAttribute"/> that are declared on the method of this handler.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <returns>A collection of attributes.</returns>
        IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>() where TAttribute : class;               
        
        /// <summary>
        /// Invokes the underlying <see cref="IMessageHandler{TMessage}" /> with the <see cref="Message" />.
        /// </summary>
        /// <returns>A task carrying out the invocation.</returns>
        Task InvokeAsync();
    }
}
