using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.Messaging.Domain
{
    internal abstract class DomainEventMember
    {        
        private const BindingFlags _MemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        internal abstract Type MemberType
        {
            get;
        }

        protected abstract Type MemberAttributeType
        {
            get;
        }

        public Expression CreateMemberAccessExpression(Expression message)
        {
            return CreateMemberExpression(new MemberAccessExpressionFactory(message));
        }        

        public Expression CreateMemberAssignmentExpression(Expression message, Expression value)
        {
            return CreateMemberExpression(new MemberAssignmentExpressionFactory(message, value));
        }

        private Expression CreateMemberExpression(MemberExpressionFactory expressionFactory)
        {
            return expressionFactory.CreateMemberExpression(this);
        }

        internal IEnumerable<FieldInfo> FindMemberCandidateFields(Type messageType)
        {
            return
                from field in messageType.GetFields(_MemberFlags)
                where HasAttribute(field, MemberAttributeType)
                select field;
        }        

        internal IEnumerable<PropertyInfo> FindMemberCandidateProperties(Type messageType)
        {
            return
                from property in messageType.GetProperties(_MemberFlags)
                where HasAttribute(property, MemberAttributeType)
                select property;
        }

        private static bool HasAttribute(MemberInfo member, Type attributeType)
        {
            return member.GetCustomAttributes(attributeType).Any();
        }       

        internal abstract Exception NewMemberNotFoundException(Type messageType);

        internal abstract Exception NewMultipleMemberCandidatesFoundException(Type messageType);        
    }
}
