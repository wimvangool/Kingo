using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.MicroServices.DataContracts
{
    /// <summary>
    /// This exception is thrown when an attempt to update a <see cref="IDataContract"/> to its latest
    /// version fails for some reason.
    /// </summary>
    [Serializable]
    public sealed class DataContractUpdateFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContractUpdateFailedException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DataContractUpdateFailedException(string message) :
            base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContractUpdateFailedException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The cause of this exception.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public DataContractUpdateFailedException(string message, Exception innerException) :
            base(message, innerException) { }

        private DataContractUpdateFailedException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }
    }
}
