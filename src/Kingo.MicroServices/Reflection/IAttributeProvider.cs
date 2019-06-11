using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kingo.Reflection
{
    /// <summary>
    /// When implemented by a class, serves as a provider of attributes declared on class, method, parameter, field, or any other member
    /// that can be decorated with attributes.
    /// </summary>
    public interface IAttributeProvider
    {        
        /// <summary>
        /// Attempts to retrieve a single attribute of type <typeparamref name="TAttribute"/> defined on the associated <see cref="Target" />.
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
        bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class;

        /// <summary>
        /// Retrieves all attributes of type <typeparamref name="TAttribute"/> that are declared on the associated <see cref="Target"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <returns>A collection of attributes.</returns>
        IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() where TAttribute : class;
    }
}
