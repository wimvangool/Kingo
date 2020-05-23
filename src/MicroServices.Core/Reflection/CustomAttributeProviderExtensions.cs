using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Kingo.Ensure;

namespace Kingo.Reflection
{
    /// <summary>
    /// Contains extension methods for instances of type <see cref="ICustomAttributeProvider"/>.
    /// </summary>
    public static class CustomAttributeProviderExtensions
    {
        /// <summary>
        /// Attempts to retrieve a single attribute of type <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <param name="provider">The provider that may provide the attributes.</param>
        /// <param name="attribute">
        /// When this method returns <c>true</c>, this parameter will refer to the retrieved attribute;
        /// otherwise it will be <c>null</c>.
        /// </param>
        /// <param name="inherit">
        /// <c>true</c> to search the member's inheritance chain to find the attributes; otherwise, <c>false</c>.
        /// This parameter is ignored for assemblies, properties, events, and other provider that don't support inheritance.
        /// </param>
        /// <returns><c>true</c> if the attribute was retrieved; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Multiple attribute defined of the method are assignable to type <typeparamref name="TAttribute"/>.
        /// </exception>
        public static bool TryGetAttributeOfType<TAttribute>(this ICustomAttributeProvider provider, out TAttribute attribute, bool inherit = true) where TAttribute : class
        {
            try
            {
                attribute = provider.GetAttributesOfType<TAttribute>(inherit).SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw NewAmbiguousAttributeMatchException(provider.ToString(), typeof(TAttribute));
            }
            return attribute != null;
        }

        /// <summary>
        /// Retrieves all attributes of type <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <param name="provider">The provider that may provide the attributes.</param>
        /// <param name="inherit">
        /// <c>true</c> to search this member's inheritance chain to find the attributes; otherwise, <c>false</c>.
        /// This parameter is ignored for properties and events.
        /// </param>
        /// <returns>A collection of attributes.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<TAttribute> GetAttributesOfType<TAttribute>(this ICustomAttributeProvider provider, bool inherit = false) where TAttribute : class =>
            IsNotNull(provider, nameof(provider)).GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>();

        private static Exception NewAmbiguousAttributeMatchException(string targetName, Type attributeType)
        {
            var messageFormat = ExceptionMessages.MemberInfoExtensions_AmbiguousAttributeMatch;
            var message = string.Format(messageFormat, attributeType.Name, targetName);
            return new InvalidOperationException(message);
        }
    }
}
