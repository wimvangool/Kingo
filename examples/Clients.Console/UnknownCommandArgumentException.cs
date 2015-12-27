using System;

namespace Clients.ConsoleApp
{        
    internal class UnknownCommandArgumentException : Exception
    {
        internal readonly string CommandArgument;
        
        public UnknownCommandArgumentException(string commandArgument)
        {
            CommandArgument = commandArgument;
        }        
    }
}