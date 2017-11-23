using System;

namespace Kingo.Messaging.Validation
{
    internal sealed class MemberExceptionFactory : ErrorMessageCollection
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

        internal Exception CreateException() =>
             new InvalidOperationException(_errorMessage);
    }
}
