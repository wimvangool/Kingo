using System.Linq.Expressions;

namespace Kingo.Messaging.Validation
{
    internal sealed class MethodCallExpressionFactory<T, TValue>
    {
        private readonly MemberExpressionDecoder<T, TValue> _interpreter;
        private readonly LambdaExpression _leftExpression;
        private readonly MethodCallDecorator<T, TValue> _methodCallDecorator;

        internal MethodCallExpressionFactory(MemberExpressionDecoder<T, TValue> interpreter, LambdaExpression leftExpression, MethodCallDecorator<T, TValue> methodCallDecorator)        
        {
            _interpreter = interpreter;
            _leftExpression = leftExpression;
            _methodCallDecorator = methodCallDecorator;
        }

        internal MethodCallExpressionFactory<T, TValue> Append(MethodCallDecorator<T, TValue> methodCallAppender)
        {
            return new MethodCallExpressionFactory<T, TValue>(_interpreter, _leftExpression, _methodCallDecorator.Append(methodCallAppender));
        }

        internal MethodCallExpression CreateMethodCallExpression()
        {
            var fieldOrPropertyName = GetFieldOrPropertyName(_leftExpression);
            var methodCallExpression = _interpreter.CreateMethodCallExpression(_leftExpression, fieldOrPropertyName);

            return _methodCallDecorator.Decorate(methodCallExpression);
        }

        private static Identifier GetFieldOrPropertyName(LambdaExpression expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                return null;
            }
            return Identifier.Parse(memberExpression.Member.Name);
        }  
    }
}
