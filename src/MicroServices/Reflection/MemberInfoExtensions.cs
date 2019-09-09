using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kingo.Reflection
{
    /// <summary>
    /// Contains extension methods for instances of type <see cref="MemberInfo" />.
    /// </summary>
    public static class MemberInfoExtensions
    {
        #region [====== MemberInfo ======]

        /// <summary>
        /// Attempts to retrieve a single attribute of type <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <param name="member">The member to obtain the attributes for.</param>
        /// <param name="attribute">
        /// When this method returns <c>true</c>, this parameter will refer to the retrieved attribute;
        /// otherwise it will be <c>null</c>.
        /// </param>
        /// <param name="inherit">
        /// <c>true</c> to search this member's inheritance chain to find the attributes; otherwise, <c>false</c>.
        /// This parameter is ignored for properties and events.
        /// </param>
        /// <returns><c>true</c> if the attribute was retrieved; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Multiple attribute defined of the method are assignable to type <typeparamref name="TAttribute"/>.
        /// </exception>
        public static bool TryGetAttributeOfType<TAttribute>(this MemberInfo member, out TAttribute attribute, bool inherit = true) where TAttribute : class
        {
            try
            {
                attribute = member.GetAttributesOfType<TAttribute>(inherit).SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw NewAmbiguousAttributeMatchException(member.Name, typeof(TAttribute));
            }
            return attribute != null;
        }

        /// <summary>
        /// Retrieves all attributes of type <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <param name="member">The member to obtain the attributes for.</param>
        /// <param name="inherit">
        /// <c>true</c> to search this member's inheritance chain to find the attributes; otherwise, <c>false</c>.
        /// This parameter is ignored for properties and events.
        /// </param>
        /// <returns>A collection of attributes.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<TAttribute> GetAttributesOfType<TAttribute>(this MemberInfo member, bool inherit = false) where TAttribute : class
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }
            return member.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>();
        }

        #endregion

        #region [====== ParameterInfo ======]

        /// <summary>
        /// Attempts to retrieve a single attribute of type <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <param name="parameter">The member to obtain the attributes for.</param>
        /// <param name="attribute">
        /// When this method returns <c>true</c>, this parameter will refer to the retrieved attribute;
        /// otherwise it will be <c>null</c>.
        /// </param>
        /// <param name="inherit">
        /// <c>true</c> to search this member's inheritance chain to find the attributes; otherwise, <c>false</c>.
        /// This parameter is ignored for properties and events.
        /// </param>
        /// <returns><c>true</c> if the attribute was retrieved; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parameter"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Multiple attribute defined of the method are assignable to type <typeparamref name="TAttribute"/>.
        /// </exception>
        public static bool TryGetAttributeOfType<TAttribute>(this ParameterInfo parameter, out TAttribute attribute, bool inherit = true) where TAttribute : class
        {
            try
            {
                attribute = parameter.GetAttributesOfType<TAttribute>(inherit).SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw NewAmbiguousAttributeMatchException(parameter.Name, typeof(TAttribute));
            }
            return attribute != null;
        }

        /// <summary>
        /// Retrieves all attributes of type <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <param name="parameter">The member to obtain the attributes for.</param>
        /// <param name="inherit">
        /// <c>true</c> to search this member's inheritance chain to find the attributes; otherwise, <c>false</c>.
        /// This parameter is ignored for properties and events.
        /// </param>
        /// <returns>A collection of attributes.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parameter"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<TAttribute> GetAttributesOfType<TAttribute>(this ParameterInfo parameter, bool inherit = false) where TAttribute : class
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
            return parameter.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>();
        }

        #endregion

        private static Exception NewAmbiguousAttributeMatchException(string targetName, Type attributeType)
        {
            var messageFormat = ExceptionMessages.MemberInfoExtensions_AmbiguousAttributeMatch;
            var message = string.Format(messageFormat, attributeType.Name, targetName);
            return new InvalidOperationException(message);
        }
    }
}
