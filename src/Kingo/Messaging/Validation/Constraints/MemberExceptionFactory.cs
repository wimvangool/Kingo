using System;

namespace Kingo.Messaging.Validation.Constraints
{
    internal sealed class MemberExceptionFactory : ErrorMessageReader
    {
        private ErrorInheritanceLevel _inheritanceLevel = ErrorInheritanceLevel.MaxInherited;
        private string _errorMessage;        

        public override void Add(string errorMessage, string memberName, ErrorInheritanceLevel inheritanceLevel)
        {
            if (errorMessage == null)
            {
                throw new ArgumentNullException(nameof(errorMessage));    
            }
            if (inheritanceLevel < _inheritanceLevel)
            {
                _inheritanceLevel = inheritanceLevel;
                _errorMessage = errorMessage;
            }
        }

        internal Exception CreateException()
        {            
            return new InvalidOperationException(_errorMessage);
        }        
    }
}
