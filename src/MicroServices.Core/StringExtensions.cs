using System;

namespace Kingo
{
    /// <summary>
    /// Contains extensions methods for instances of type <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Removes the specified <paramref name="postfix"/> from <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to remove the postfix from.</param>
        /// <param name="postfix">The postfix to remove.</param>
        /// <param name="comparison">
        /// Indicates which comparison must be used when checking if <paramref name="value"/> ends with <paramref name="postfix"/>.
        /// </param>
        /// <returns>
        /// The value where the postfix has been removed if <paramref name="value"/> ends with the specified <paramref name="postfix"/>;
        /// otherwise it will just return <paramref name="value"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> or <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparison"/> is not a valid value.
        /// </exception>
        public static string RemovePostfix(this string value, string postfix, StringComparison comparison = StringComparison.CurrentCulture)
        {
            if (value.TryRemovePostfix(postfix, out var newValue, comparison))
            {
                return newValue;
            }
            return value;
        }

        /// <summary>
        /// Attempts to remove the specified <paramref name="postfix"/> from the string.
        /// </summary>
        /// <param name="value">The value to remove the postfix from.</param>
        /// <param name="postfix">The postfix to remove.</param>
        /// <param name="newValue">
        /// If <paramref name="value"/> ends with <paramref name="postfix"/>, this parameter will be assigned the
        /// value where this postfix has been removed.
        /// </param>
        /// <param name="comparison">
        /// Indicates which comparison must be used when checking if <paramref name="value"/> ends with <paramref name="postfix"/>.
        /// </param>
        /// <returns><c>true</c> if the postfix was removed; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> or <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparison"/> is not a valid value.
        /// </exception>
        public static bool TryRemovePostfix(this string value, string postfix, out string newValue, StringComparison comparison = StringComparison.CurrentCulture)
        {   
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (postfix == null)
            {
                throw new ArgumentNullException(nameof(postfix));
            }
            if (value.EndsWith(postfix, comparison))
            {
                newValue = value.Remove(value.Length - postfix.Length);
                return true;
            }
            newValue = null;
            return false;
        }
    }
}
