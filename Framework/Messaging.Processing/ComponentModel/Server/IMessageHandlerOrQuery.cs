using System;
using System.Collections.Generic;

namespace ServiceComponents.ComponentModel.Server
{
    /// <summary>
    /// When implemented by a class, represents a provider of attributes that are declared
    /// on a class or interface method implementation of a <see cref="IMessageHandler{T}" />
    /// or a <see cref="Query{T, S}" />.
    /// </summary>
    public interface IMessageHandlerOrQuery
    {
        /// <summary>
        /// Attempts to retrieve a single attribute of type <typeparamref name="TAttribute"/> defined on the class of this instance.
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
        /// Attempts to retrieve a single attribute of type <typeparamref name="TAttribute"/> defined on the method on this instance.
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
        /// Retrieves all attributes of type <typeparamref name="TAttribute"/> that are declared on the class of this instance.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <returns>A collection of attributes.</returns>
        IEnumerable<TAttribute> GetClassAttributesOfType<TAttribute>() where TAttribute : class;

        /// <summary>
        /// Retrieves all attributes of type <typeparamref name="TAttribute"/> that are declared on the method of this instance.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <returns>A collection of attributes.</returns>
        IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>() where TAttribute : class; 
    }
}
