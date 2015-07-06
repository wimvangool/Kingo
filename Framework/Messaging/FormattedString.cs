using System.Globalization;

namespace System
{
    /// <summary>
    /// Represents an error message with optional arguments for formatting.
    /// </summary>
    public sealed class FormattedString
    {        
        private readonly string _format;
        private readonly object[] _arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedString" /> class.
        /// </summary>
        /// <param name="value">The error message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public FormattedString(string value)
        {            
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }            
            _format = value;
            _arguments = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedString" /> class.
        /// </summary>
        /// <param name="format">The error message format string.</param>
        /// <param name="arg0">The argument of the format string.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> is <c>null</c>.
        /// </exception>
        public FormattedString(string format, object arg0)
        {            
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }            
            _format = format;
            _arguments = new [] { arg0 };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedString" /> class.
        /// </summary>
        /// <param name="format">The error message format string.</param>
        /// <param name="arg0">The first argument of the format string.</param>
        /// <param name="arg1">The second argument of the format string.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> is <c>null</c>.
        /// </exception>
        public FormattedString(string format, object arg0, object arg1)
        {            
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }            
            _format = format;
            _arguments = new[] { arg0, arg1 };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedString" /> class.
        /// </summary>
        /// <param name="format">The error message format string.</param>
        /// <param name="arguments">The arguments of the format string.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> is <c>null</c>.
        /// </exception>
        public FormattedString(string format, params object[] arguments)
        {            
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }                        
            _format = format;
            _arguments = arguments;
        }        

        /// <summary>
        /// Returns the formatted error message.
        /// </summary>
        /// <returns>The formatted error message.</returns>
        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns the formatted error message.
        /// </summary>
        /// <param name="provider">A format provider.</param>
        /// <returns>The formatted error message.</returns>
        public string ToString(IFormatProvider provider)
        {
            if (_arguments == null || _arguments.Length == 0)
            {
                return _format;
            }
            return string.Format(provider, _format, _arguments);
        }      

        /// <summary>
        /// Implicitly converts the specified value to a <see cref="FormattedString" /> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An <see cref="FormattedString" />.</returns>
        public static implicit operator FormattedString(string value)
        {
            return value == null ? null : new FormattedString(value);
        }
    }
}
