using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.Messaging.Domain
{
    internal sealed class MemberAccessExpressionFactory : MemberExpressionFactory
    {
        private readonly Expression _message;

        public MemberAccessExpressionFactory(Expression message)
        {
            _message = message;
        }

        protected override Type MessageType
        {
            get { return _message.Type; }
        }

        protected override Expression CreateFieldExpression(FieldInfo field)
        {
            return Expression.Field(_message, field);
        }

        protected override Expression CreatePropertyExpression(PropertyInfo property)
        {
            return Expression.Property(_message, property);
        }
    }
}
