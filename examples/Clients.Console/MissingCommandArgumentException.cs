using System;

namespace Clients.ConsoleApp
{        
    internal class MissingCommandArgumentException : Exception
    {
        internal readonly string CommandArgument;
        
        public MissingCommandArgumentException(string commandArgument)
        {
            CommandArgument = commandArgument;
        }        
    }
}