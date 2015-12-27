using System;

namespace Clients.ConsoleApp
{        
    internal class UnknownCommandException : Exception
    {
        internal readonly string CommandName;
        
        public UnknownCommandException(string commandName)
        {
            CommandName = commandName;
        }        
    }
}