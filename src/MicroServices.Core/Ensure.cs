using System;

namespace Kingo
{
    /// <summary>
    /// Contains a set of generic parameter validation functions.
    /// </summary>
    public static class Ensure
    {
        #region [====== IsEqualTo ======]

        /// <summary>
        /// Ensures the specified <paramref name="value"/> is equal to the specified <paramref name="comparand"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="comparand">The value to which <paramref name="value"/> is compared.</param>
        /// <param name="paramName">Name of the value-parameter.</param>
        /// <returns>The specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not equal to the specified <paramref name="comparand"/>.
        /// </exception>
        public static TValue IsEqualTo<TValue>(TValue value, TValue comparand, string paramName = null) where TValue : struct, IEquatable<TValue>
        {
            if (value.Equals(comparand))
            {
                throw NewValueIsNotEqualToComparandException(value, comparand, paramName ?? nameof(value));
            }
            return value;
        }

        private static Exception NewValueIsNotEqualToComparandException(object value, object comparand, string paramName)
        {
            var messageFormat = ExceptionMessages.Ensure_ValueIsNotEqualTo;
            var message = string.Format(messageFormat, value, comparand);
            return new ArgumentOutOfRangeException(paramName, message);
        }

        #endregion

        #region [====== IsNotEqualTo ======]

        /// <summary>
        /// Ensures the specified <paramref name="value"/> is not equal equal to the specified <paramref name="comparand"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="comparand">The value to which <paramref name="value"/> is compared.</param>
        /// <param name="paramName">Name of the value-parameter.</param>
        /// <returns>The specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is equal to the specified <paramref name="comparand"/>.
        /// </exception>
        public static TValue IsNotEqualTo<TValue>(TValue value, TValue comparand, string paramName = null) where TValue : struct, IComparable<TValue>
        {
            if (value.Equals(comparand))
            {
                return value;
            }
            throw NewValueNotGreaterThanOrEqualToComparandException(value, comparand, paramName);
        }

        private static Exception NewValueIsEqualToComparandException(object value, object comparand, string paramName)
        {
            var messageFormat = ExceptionMessages.Ensure_ValueIsEqualTo;
            var message = string.Format(messageFormat, value, comparand);
            return new ArgumentOutOfRangeException(paramName, message);
        }

        #endregion

        #region [====== IsGreaterThan ======]

        /// <summary>
        /// Ensures the specified <paramref name="value"/> is greater than the specified <paramref name="comparand"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="comparand">The value to which <paramref name="value"/> is compared.</param>
        /// <param name="paramName">Name of the value-parameter.</param>
        /// <returns>The specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not greater than the specified <paramref name="comparand"/>.
        /// </exception>
        public static TValue IsGreaterThan<TValue>(TValue value, TValue comparand, string paramName = null) where TValue : struct, IComparable<TValue>
        {
            if (value.CompareTo(comparand) <= 0)
            {
                throw NewValueIsNotGreaterThanComparandException(value, comparand, paramName ?? nameof(value));
            }
            return value;
        }

        private static Exception NewValueIsNotGreaterThanComparandException(object value, object comparand, string paramName)
        {
            var messageFormat = ExceptionMessages.Ensure_ValueIsNotGreaterThan;
            var message = string.Format(messageFormat, value, comparand);
            return new ArgumentOutOfRangeException(paramName, message);
        }

        #endregion

        #region [====== IsGreaterThanOrEqualTo ======]

        /// <summary>
        /// Ensures the specified <paramref name="value"/> is greater than or equal to the specified <paramref name="comparand"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="comparand">The value to which <paramref name="value"/> is compared.</param>
        /// <param name="paramName">Name of the value-parameter.</param>
        /// <returns>The specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not greater than or equal to the specified <paramref name="comparand"/>.
        /// </exception>
        public static TValue IsGreaterThanOrEqualTo<TValue>(TValue value, TValue comparand, string paramName = null) where TValue : struct, IComparable<TValue>
        {
            if (value.CompareTo(comparand) < 0)
            {
                throw NewValueNotGreaterThanOrEqualToComparandException(value, comparand, paramName);
            }
            return value;
        }

        private static Exception NewValueNotGreaterThanOrEqualToComparandException(object value, object comparand, string paramName)
        {
            var messageFormat = ExceptionMessages.Ensure_ValueIsNotGreaterThanOrEqualTo;
            var message = string.Format(messageFormat, value, comparand);
            return new ArgumentOutOfRangeException(paramName, message);
        }

        #endregion

        #region [====== IsNotNull ======]

        /// <summary>
        /// Ensures that the specified <paramref name="value"/> is not <c>null</c>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">Name of the value-parameter.</param>
        /// <returns>The specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value" /> is <c>null</c>.
        /// </exception>
        public static TValue IsNotNull<TValue>(TValue value, string paramName = null) where TValue : class =>
            value ?? throw new ArgumentNullException(paramName ?? nameof(value));

        #endregion
    }
}
