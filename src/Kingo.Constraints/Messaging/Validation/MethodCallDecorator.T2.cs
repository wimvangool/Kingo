using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kingo.Messaging.Validation
{
    internal abstract class MethodCallDecorator<T, TValue>
    {        
        public abstract MethodCallExpression Decorate(MethodCallExpression expression);

        public virtual MethodCallDecorator<T, TValue> Append(MethodCallDecorator<T, TValue> appender)
        {
            if (appender == null)
            {
                return this;
            }
            return new CompositeAppender<T, TValue>(this, appender);
        }

        public override string ToString() => string.Join(".", MethodCalls());

        internal abstract IEnumerable<string> MethodCalls();

        protected static Type GetGenericArgumentType(Type type, int index) => type.GetGenericArguments()[index];

        protected static Expression NoErrorMessage() => Expression.Constant(null, typeof(string));

        public static MethodCallDecorator<T, TValue> operator +(MethodCallDecorator<T, TValue> left, MethodCallDecorator<T, TValue> right)
        {
            if (ReferenceEquals(left, null))
            {
                return right;
            }
            return left.Append(right);
        }
    }
}
