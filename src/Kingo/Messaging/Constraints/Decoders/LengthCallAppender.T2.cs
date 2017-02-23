using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.Messaging.Constraints.Decoders
{
    internal sealed class LengthCallAppender<T, TValue> : MethodCallDecorator<T, TValue>
    {
        internal override IEnumerable<string> MethodCalls()
        {
            yield return "Length()";
        }

        public override MethodCallExpression Decorate(MethodCallExpression expression)
        {
            // IMemberConstraintBuilder<T, TValue>.Length().
            var valueType = GetGenericArgumentType(expression.Type, 1).GetElementType();
            var lengthMethod = GetLengthMethod(valueType);

            return Expression.Call(lengthMethod, expression);
        }

        private static MethodInfo GetLengthMethod(Type valueType)
        {
            var lengthMethod =
                from method in typeof(CollectionConstraints).GetMethods(BindingFlags.Public | BindingFlags.Static)
                where method.IsGenericMethodDefinition && method.Name == "Length"
                let parameters = method.GetParameters()
                where parameters.Length == 1
                where parameters[0].ParameterType.IsGenericType
                where parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IMemberConstraintBuilder<,>)
                select method.MakeGenericMethod(typeof(T), valueType);

            return lengthMethod.Single();
        }
    }
}
