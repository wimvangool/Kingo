using System;

namespace Kingo.Messaging
{
    [Serializable]
    public sealed class MetaAssertFailedException : Exception
    {                        
        public MetaAssertFailedException(string message, Exception innerException)
            : base(message, innerException) { }        
    }
}
