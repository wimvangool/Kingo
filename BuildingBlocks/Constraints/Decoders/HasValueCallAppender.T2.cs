using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.BuildingBlocks.Constraints.Decoders
{
    internal sealed class HasValueCallAppender<T, TValue> : MethodCallDecorator<T, TValue>
    {        
        internal override IEnumerable<string> MethodCalls()
        {
            yield return "HasValue()";
        }

        public override MethodCallExpression Decorate(MethodCallExpression expression)
        {
            return AppendHasValue(expression); 
        }

        private static MethodCallExpression AppendHasValue(MethodCallExpression methodCallExpression)
        {
            // IMemberConstraintBuilder<T, TValue>.HasValue<TValue>(string).
            var nullableType = GetGenericArgumentType(methodCallExpression.Type, 1);
            var valueType = GetGenericArgumentType(nullableType, 0);
            var hasValueMethod = GetHasValueMethod(valueType);

            return Expression.Call(hasValueMethod, methodCallExpression, NoErrorMessage());
        }

        private static MethodInfo GetHasValueMethod(Type valueType)
        {
            var isNotNullMethod =
                from method in typeof(NullableConstraints).GetMethods(BindingFlags.Public | BindingFlags.Static)
                where method.IsGenericMethodDefinition && method.Name == "HasValue"
                let parameters = method.GetParameters()
                where parameters.Length == 2
                where parameters[0].ParameterType.IsGenericType
                where parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IMemberConstraintBuilder<,>)
                where parameters[1].ParameterType == typeof(string)
                select method.MakeGenericMethod(typeof(T), valueType);

            return isNotNullMethod.Single();
        }
    }
}
