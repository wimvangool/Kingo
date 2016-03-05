using System;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    internal sealed class DomainEventVersionMember : DomainEventMember
    {
        private readonly Type _versionType;

        public DomainEventVersionMember(Type versionType)
        {
            _versionType = versionType;
        }

        internal override Type MemberType
        {
            get { return _versionType; }
        }

        protected override Type MemberAttributeType
        {
            get { return typeof(VersionAttribute); }
        }

        internal override Exception NewMemberNotFoundException(Type messageType)
        {
            var messageFormat = ExceptionMessages.DomainEvent_VersionMemberNotFound;
            var message = string.Format(messageFormat, messageType);
            return new InvalidOperationException(message);
        }

        internal override Exception NewMultipleMemberCandidatesFoundException(Type messageType)
        {
            var messageFormat = ExceptionMessages.DomainEvent_MultipleVersionMembersFound;
            var message = string.Format(messageFormat, messageType);
            return new InvalidOperationException(message);
        }
    }
}
