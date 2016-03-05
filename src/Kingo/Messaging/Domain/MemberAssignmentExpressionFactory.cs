using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.Messaging.Domain
{
    internal sealed class MemberAssignmentExpressionFactory : MemberExpressionFactory
    {
        private readonly Expression _message;
        private readonly Expression _value;

        public MemberAssignmentExpressionFactory(Expression message, Expression value)
        {
            _message = message;
            _value = value;
        }

        protected override Type MessageType
        {
            get { return _message.Type; }
        }

        protected override Expression CreateFieldExpression(FieldInfo field)
        {
            return Expression.Assign(Expression.Field(_message, field), _value);
        }

        protected override Expression CreatePropertyExpression(PropertyInfo property)
        {
            return Expression.Assign(Expression.Property(_message, property), _value);
        }
    }
}
