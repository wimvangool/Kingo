using System;
using static Kingo.Ensure;

namespace Kingo
{
    /// <summary>
    /// Contains extensions methods for instances of type <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        #region [====== RemovePrefix ======]

        /// <summary>
        /// Removes the specified <paramref name="prefix"/> from <paramref name="value"/> if <paramref name="value" /> starts with
        /// the specified <paramref name="prefix"/>.
        /// </summary>
        /// <param name="value">The value to remove the prefix from.</param>
        /// <param name="prefix">The prefix to remove.</param>
        /// <param name="comparison">
        /// Indicates which comparison must be used when checking if <paramref name="value"/> ends with <paramref name="prefix"/>.
        /// </param>
        /// <returns>
        /// The value where the prefix has been removed if <paramref name="value"/> starts with the specified <paramref name="prefix"/>;
        /// otherwise it will just return <paramref name="value"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> or <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparison"/> is not a valid value.
        /// </exception>
        public static string RemovePrefix(this string value, string prefix, StringComparison comparison = StringComparison.CurrentCulture)
        {
            if (value.TryRemovePrefix(prefix, out var newValue, comparison))
            {
                return newValue;
            }
            return value;
        }

        /// <summary>
        /// Attempts to remove the specified <paramref name="prefix"/> from the string.
        /// </summary>
        /// <param name="value">The value to remove the prefix from.</param>
        /// <param name="prefix">The prefix to remove.</param>
        /// <param name="newValue">
        /// If <paramref name="value"/> ends with <paramref name="prefix"/>, this parameter will be assigned the
        /// value where this prefix has been removed.
        /// </param>
        /// <param name="comparison">
        /// Indicates which comparison must be used when checking if <paramref name="value"/> ends with <paramref name="prefix"/>.
        /// </param>
        /// <returns><c>true</c> if the prefix was removed; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> or <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparison"/> is not a valid value.
        /// </exception>
        public static bool TryRemovePrefix(this string value, string prefix, out string newValue, StringComparison comparison = StringComparison.CurrentCulture)
        {
            if (IsNotNull(value).StartsWith(IsNotNull(prefix, nameof(prefix)), comparison))
            {
                newValue = value.Remove(0, prefix.Length);
                return true;
            }
            newValue = null;
            return false;
        }

        #endregion

        #region [====== RemovePostfix ======]

        /// <summary>
        /// Removes the specified <paramref name="postfix"/> from <paramref name="value"/> if <paramref name="value" /> ends with
        /// the specified <paramref name="postfix"/>.
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
            if (IsNotNull(value).EndsWith(IsNotNull(postfix, nameof(postfix)), comparison))
            {
                newValue = value.Remove(value.Length - postfix.Length);
                return true;
            }
            newValue = null;
            return false;
        }

        #endregion
    }
}
