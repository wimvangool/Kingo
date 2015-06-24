﻿using System;
using System.ComponentModel.Server.Domain;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SummerBreeze.ChessApplication.Players
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

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPasswordException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private InvalidPasswordException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}