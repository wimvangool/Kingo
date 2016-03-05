using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    internal abstract class MemberExpressionFactory
    {        
        protected abstract Type MessageType
        {
            get;
        }

        public Expression CreateMemberExpression(DomainEventMember member)
        {
            var fields = member.FindMemberCandidateFields(MessageType).ToArray();
            var properties = member.FindMemberCandidateProperties(MessageType).ToArray();

            var memberCount = fields.Length + properties.Length;
            if (memberCount == 0)
            {
                throw member.NewMemberNotFoundException(MessageType);
            }
            if (memberCount > 1)
            {
                throw member.NewMultipleMemberCandidatesFoundException(MessageType);
            }
            if (fields.Length == 1)
            {
                var field = fields[0];
                if (field.FieldType == member.MemberType)
                {
                    return CreateFieldExpression(fields[0]);
                }
                throw NewIncompatibleMemberTypeException(field.Name, field.FieldType, member.MemberType);
            }
            var property = properties[0];
            if (property.PropertyType == member.MemberType)
            {
                return CreatePropertyExpression(properties[0]);
            }
            throw NewIncompatibleMemberTypeException(property.Name, property.PropertyType, member.MemberType);
        }        

        protected abstract Expression CreateFieldExpression(FieldInfo field);

        protected abstract Expression CreatePropertyExpression(PropertyInfo property);

        private static Exception NewIncompatibleMemberTypeException(string name, Type actualType, Type expectedType)
        {
            var messageFormat = ExceptionMessages.DomainEvent_IncompatibleMemberType;
            var message = string.Format(messageFormat, name, actualType, expectedType);
            return new InvalidOperationException(message);
        }
    }
}
