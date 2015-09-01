using System;
using System.Collections.Generic;

namespace Kingo.ChessApplication
{
    internal sealed class UserCommandArgumentStack
    {
        private readonly Dictionary<string, string> _remainingArguments;

        internal UserCommandArgumentStack(IEnumerable<KeyValuePair<string, string>> arguments)
        {
            var remainingArguments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var argument in arguments)
            {
                remainingArguments.Add(argument.Key, argument.Value);
            }
            _remainingArguments = remainingArguments;
        }        

        internal bool PopBoolean(string name)
        {
            throw new NotImplementedException();
        }

        internal short PopInt16(string name)
        {
            throw new NotImplementedException();
        }

        internal int PopInt32(string name)
        {
            throw new NotImplementedException();
        }

        internal long PopInt64(string name)
        {
            throw new NotImplementedException();
        }

        internal string PopString(string name)
        {
            string value;

            if (_remainingArguments.TryGetValue(name, out value))
            {
                _remainingArguments.Remove(name);

                return value;
            }
            throw new UserCommandParseException(new [] { name }, "Missing required argument:");
        }   
     
        internal void ThrowIfNotEmpty()
        {
            if (_remainingArguments.Count == 0)
            {
                return;
            }
            throw new UserCommandParseException(_remainingArguments.Keys, "Invalid arguments specified:");
        }
    }
}
