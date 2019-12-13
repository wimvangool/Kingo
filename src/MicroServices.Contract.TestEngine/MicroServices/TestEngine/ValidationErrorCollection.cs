using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class ValidationErrorCollection : IValidationErrorCollection
    {                
        private readonly Dictionary<string, MemberErrorCollection> _memberErrorCollections;

        public ValidationErrorCollection(IEnumerable<ValidationResult> validationErrors)
        {            
            _memberErrorCollections = CreateMemberErrorCollections(validationErrors);
        }

        private Dictionary<string, MemberErrorCollection> CreateMemberErrorCollections(IEnumerable<ValidationResult> validationErrors)
        {
            var memberErrorCollections = new Dictionary<string, MemberErrorCollection>();

            foreach (var validationError in validationErrors)
            {
                foreach (var memberName in validationError.MemberNames)
                {
                    GetOrAddMemberErrorCollection(memberErrorCollections, memberName).Add(validationError.ErrorMessage);
                }
            }
            return memberErrorCollections;
        }

        private static MemberErrorCollection GetOrAddMemberErrorCollection(IDictionary<string, MemberErrorCollection> collections, string memberName)
        {
            if (collections.TryGetValue(memberName, out var collection))
            {
                return collection;
            }
            return collections[memberName] = new MemberErrorCollection(memberName);
        }

        public IMemberErrorCollection this[string memberName]
        {
            get
            {
                if (memberName == null)
                {
                    throw new ArgumentNullException(nameof(memberName));
                }
                if (_memberErrorCollections.TryGetValue(memberName, out var memberErrorCollection))
                {
                    return memberErrorCollection;
                }
                throw NewMemberErrorsNotFoundException(memberName);
            }
        }                

        private static Exception NewMemberErrorsNotFoundException(string memberName)
        {
            var messageFormat = ExceptionMessages.Request_MemberErrorsNotFound;
            var message = string.Format(messageFormat, memberName);
            return new TestFailedException(message);
        }
    }
}
