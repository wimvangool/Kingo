using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.BuildingBlocks.Constraints.Decoders
{
    internal sealed class AndCallAppender<T, TValue> : MethodCallDecorator<T, TValue>
    {
        private readonly LambdaExpression _rightExpression;

        internal AndCallAppender(LambdaExpression rightExpression)
        {
            _rightExpression = rightExpression;
        }

        internal override IEnumerable<string> MethodCalls()
        {
            yield return "IsNotNull()";
            yield return "And(<right_expression>)";
        }

        public override MethodCallExpression Decorate(MethodCallExpression expression)
        {            
            var valueType = GetGenericArgumentType(expression.Type, 1);
            if (valueType.IsValueType)
            {
                if (IsNullable(valueType))
                {
                    // Append <left_expression>.HasValue<TValue>()
                    expression = AppendHasValue(expression, valueType);                    
                }
            }
            else
            {
                // Append <left_expression>.IsNotNull<TValue>().And(<right_expression>)
                expression = AppendIsNotNull(expression, valueType);
                expression = AppendAnd(expression, _rightExpression);
            }
            return expression;
        }

        private static bool IsNullable(Type valueType)
        {
            return valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        #region [====== HasValue & IsNotNull ======]

        private static MethodCallExpression AppendHasValue(MethodCallExpression methodCallExpression, Type nullableType)
        {
            // IMemberConstraintBuilder<T, TValue>.HasValue<TValue>(string).
            var valueType = GetGenericArgumentType(nullableType, 0);
            var hasValueMethod = GetHasValueOrIsNotNullMethod(typeof(NullableConstraints), valueType, "HasValue");

            return Expression.Call(hasValueMethod, methodCallExpression, NoErrorMessage());
        }

        private static MethodCallExpression AppendIsNotNull(MethodCallExpression methodCallExpression, Type valueType)
        {
            // IMemberConstraintBuilder<T, TValue>.IsNotNull<TValue>(string).
            var isNotNullMethod = GetHasValueOrIsNotNullMethod(typeof(BasicConstraints), valueType, "IsNotNull");

            return Expression.Call(isNotNullMethod, methodCallExpression, NoErrorMessage());
        }

        private static MethodInfo GetHasValueOrIsNotNullMethod(Type classType, Type valueType, string methodName)
        {
            var hasValueOrIsNotNullMethod =
                from method in classType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                where method.IsGenericMethodDefinition && method.Name == methodName
                let parameters = method.GetParameters()
                where parameters.Length == 2
                where parameters[0].ParameterType.IsGenericType
                where parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IMemberConstraintBuilder<,>)
                where parameters[1].ParameterType == typeof(string)
                select method.MakeGenericMethod(typeof(T), valueType);

            return hasValueOrIsNotNullMethod.Single();
        }

        #endregion

        #region [====== And ======]

        private static MethodCallExpression AppendAnd(MethodCallExpression methodCallExpression, Expression rightExpression)
        {
            // IMemberConstraintBuilder<T, TValue>.And<TOther>(Expression<Func<T, TValue, TOther>>).
            var valueType = GetGenericArgumentType(methodCallExpression.Type, 1);
            var otherType = GetGenericArgumentType(rightExpression.Type, 2);
            var andMethod = GetAndMethod(valueType, otherType);

            return Expression.Call(methodCallExpression, andMethod, Expression.Quote(rightExpression));
        }

        private static MethodInfo GetAndMethod(Type valueType, Type otherType)
        {
            // IMemberConstraintBuilder<T, TValue>.And<TOther>(Expression<Func<TValue, TOther>>).
            var builderType = typeof(IMemberConstraintBuilder<,>).MakeGenericType(typeof(T), valueType);

            var andMethod =
                from method in builderType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                where method.IsGenericMethodDefinition && method.Name == "And"
                let parameters = method.GetParameters()
                where parameters.Length == 1
                where parameters[0].ParameterType.IsGenericType
                where parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>)
                select method.MakeGenericMethod(otherType);

            return andMethod.Single();
        }

        #endregion                
    }
}
