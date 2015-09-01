using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Irony;

namespace ServiceComponents.ChessApplication
{
    /// <summary>
    /// This exception is thrown when a user command could not be parsed.
    /// </summary>    
    internal class UserCommandParseException : Exception
    {
        internal readonly IEnumerable<string> ErrorMessages;

        internal UserCommandParseException(IEnumerable<string> errorMessages)
        {
            ErrorMessages = errorMessages;
        }
        
        internal UserCommandParseException(IEnumerable<string> errorMessages, string message)
            : base(message)
        {
            ErrorMessages = errorMessages;
        }
        
        internal UserCommandParseException(IEnumerable<string> errorMessages, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorMessages = errorMessages;
        }        
    }
}