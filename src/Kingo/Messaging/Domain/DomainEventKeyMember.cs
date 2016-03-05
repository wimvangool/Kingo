using System;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    internal sealed class DomainEventKeyMember : DomainEventMember
    {
        private readonly Type _keyType;

        public DomainEventKeyMember(Type keyType)
        {
            _keyType = keyType;
        }

        internal override Type MemberType
        {
            get { return _keyType; }
        }

        protected override Type MemberAttributeType
        {
            get { return typeof(KeyAttribute); }
        }

        internal override Exception NewMemberNotFoundException(Type messageType)
        {
            var messageFormat = ExceptionMessages.DomainEvent_KeyMemberNotFound;
            var message = string.Format(messageFormat, messageType);
            return new InvalidOperationException(message);
        }

        internal override Exception NewMultipleMemberCandidatesFoundException(Type messageType)
        {
            var messageFormat = ExceptionMessages.DomainEvent_MultipleKeyMembersFound;
            var message = string.Format(messageFormat, messageType);
            return new InvalidOperationException(message);
        }
    }
}
