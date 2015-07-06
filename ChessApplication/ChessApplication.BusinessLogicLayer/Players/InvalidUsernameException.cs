using System;
using System.ComponentModel.Server.Domain;

namespace SummerBreeze.ChessApplication.Players
{
    /// <summary>
    /// This exception is thrown when the specified username is invalid.
    /// </summary>
    [Serializable]
    public sealed class InvalidUsernameException : InvalidValueException<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUsernameException" /> class.
        /// </summary>
        /// <param name="value">The invalid value.</param>
        internal InvalidUsernameException(string value)
            : base(value) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUsernameException" /> class.
        /// </summary>
        /// <param name="value">The invalid value.</param>
        /// <param name="message">Message of the exception.</param>
        internal InvalidUsernameException(string value, string message)
            : base(value, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUsernameException" /> class.
        /// </summary>
        /// <param name="value">The invalid value.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        internal InvalidUsernameException(string value, string message, Exception innerException)
            : base(value, message, innerException) { }        
    }
}