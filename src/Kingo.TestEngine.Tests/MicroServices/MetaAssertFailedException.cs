using System;

namespace Kingo.MicroServices
{
    [Serializable]
    public sealed class MetaAssertFailedException : Exception
    {                        
        public MetaAssertFailedException(string message, Exception innerException)
            : base(message, innerException) { }        
    }
}
