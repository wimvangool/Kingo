using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class MemberExceptionFactory : IErrorMessageReader
    {
        private string _errorMessage;

        public void Add(IErrorMessage errorMessage, string memberName)
        {
            Add(errorMessage.ToString(), memberName);
        }

        public void Add(string errorMessage, string memberName)
        {
            _errorMessage = errorMessage;
        }

        internal Exception CreateException()
        {            
            return new InvalidOperationException(_errorMessage);
        }        
    }
}
