using System;
using ServiceComponents.ComponentModel.Server.Domain;

namespace ServiceComponents.ChessApplication.Players
{
    /// <summary>
    /// This exception is thrown when the specified password is invalid.
    /// </summary>
    [Serializable]
    public sealed class InvalidPasswordException : InvalidValueException<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPasswordException" /> class.
        /// </summary>
        /// <param name="value">The invalid value.</param>
        internal InvalidPasswordException(string value)
            : base(value) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPasswordException" /> class.
        /// </summary>
        /// <param name="value">The invalid value.</param>
        /// <param name="message">Message of the exception.</param>
        internal InvalidPasswordException(string value, string message)
            : base(value, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPasswordException" /> class.
        /// </summary>
        /// <param name="value">The invalid value.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        internal InvalidPasswordException(string value, string message, Exception innerException)
            : base(value, message, innerException) {}        
    }
}