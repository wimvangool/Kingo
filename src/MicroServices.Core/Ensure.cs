using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Kingo
{
    /// <summary>
    /// Contains a set of generic parameter validation functions.
    /// </summary>
    public static class Ensure
    {
        #region [====== IsNotNull ======]

        /// <summary>
        /// Ensures that the specified <paramref name="value"/> is not <c>null</c>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>The specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value" /> is <c>null</c>.
        /// </exception>
        public static TValue IsNotNull<TValue>(TValue value, string paramName = null) where TValue : class =>
            value ?? throw new ArgumentNullException(paramName ?? nameof(value));

        #endregion
    }
}
