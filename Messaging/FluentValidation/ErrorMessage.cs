using System.Globalization;

namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// Represents an error message with optional arguments for formatting.
    /// </summary>
    public sealed class ErrorMessage
    {        
        private readonly string _errorMessageFormat;
        private readonly object[] _errorMessageArguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage" /> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public ErrorMessage(string errorMessage)
        {            
            if (errorMessage == null)
            {
                throw new ArgumentNullException("errorMessage");
            }            
            _errorMessageFormat = errorMessage;
            _errorMessageArguments = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage" /> class.
        /// </summary>
        /// <param name="errorMessageFormat">The error message format string.</param>
        /// <param name="arg0">The argument of the format string.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public ErrorMessage(string errorMessageFormat, object arg0)
        {            
            if (errorMessageFormat == null)
            {
                throw new ArgumentNullException("errorMessageFormat");
            }            
            _errorMessageFormat = errorMessageFormat;
            _errorMessageArguments = new [] { arg0 };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage" /> class.
        /// </summary>
        /// <param name="errorMessageFormat">The error message format string.</param>
        /// <param name="arg0">The first argument of the format string.</param>
        /// <param name="arg1">The second argument of the format string.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public ErrorMessage(string errorMessageFormat, object arg0, object arg1)
        {            
            if (errorMessageFormat == null)
            {
                throw new ArgumentNullException("errorMessageFormat");
            }            
            _errorMessageFormat = errorMessageFormat;
            _errorMessageArguments = new[] { arg0, arg1 };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage" /> class.
        /// </summary>
        /// <param name="errorMessageFormat">The error message format string.</param>
        /// <param name="arguments">The arguments of the format string.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public ErrorMessage(string errorMessageFormat, params object[] arguments)
        {            
            if (errorMessageFormat == null)
            {
                throw new ArgumentNullException("errorMessageFormat");
            }                        
            _errorMessageFormat = errorMessageFormat;
            _errorMessageArguments = arguments;
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
            if (_errorMessageArguments == null || _errorMessageArguments.Length == 0)
            {
                return _errorMessageFormat;
            }
            return string.Format(provider, _errorMessageFormat, _errorMessageArguments);
        }      
    }
}
