using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ServiceComponents.ChessApplication
{
    /// <summary>
    /// This exception is thrown when an a UserCommand could not be executed.
    /// </summary>   
    internal class UserCommandExecutionException : Exception
    {
        internal UserCommandExecutionException() { }
        
        internal UserCommandExecutionException(string message)
            : base(message) { }
        
        internal UserCommandExecutionException(string message, Exception innerException)
            : base(message, innerException) { }          
    }
}