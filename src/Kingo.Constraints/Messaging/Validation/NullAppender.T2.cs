using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kingo.Messaging.Validation
{
    internal sealed class NullAppender<T, TValue> : MethodCallDecorator<T, TValue>
    {
        internal override IEnumerable<string> MethodCalls() =>
             Enumerable.Empty<string>();

        public override MethodCallExpression Decorate(MethodCallExpression expression) =>
             expression;

        public override MethodCallDecorator<T, TValue> Append(MethodCallDecorator<T, TValue> appender)
        {
            if (appender == null)
            {
                return this;
            }
            return appender;
        }        
    }
}
