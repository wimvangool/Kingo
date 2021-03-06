﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// This exception is thrown by a <see cref="IUnitOfWorkResourceManager" /> when a concurrency conflict has occurred
    /// while flushing any changes.
    /// </summary>
    public class ConcurrencyException : MessageHandlerException
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        public ConcurrencyException(string message = null, Exception innerException = null)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <inheritdoc />
        public override BadRequestException AsBadRequestException(string message) =>
            new ConflictException(message, this);        
    }
}
