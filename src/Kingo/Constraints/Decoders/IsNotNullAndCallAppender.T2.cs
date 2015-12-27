using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.Constraints.Decoders
{
    internal sealed class IsNotNullAndCallAppender<T, TValue> : MethodCallDecorator<T, TValue>
    {
        private readonly LambdaExpression _rightExpression;

        internal IsNotNullAndCallAppender(LambdaExpression rightExpression)
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
            // Append <left_expression>.IsNotNull<TValue>().And(<right_expression>)
            expression = AppendIsNotNull(expression);
            expression = AppendAnd(expression, _rightExpression);
            return expression;
        }        

        #region [====== IsNotNull ======]

        private static MethodCallExpression AppendIsNotNull(MethodCallExpression expression)
        {
            // IMemberConstraintBuilder<T, TValue>.IsNotNull<TValue>(string).
            var valueType = GetGenericArgumentType(expression.Type, 1);
            var isNotNullMethod = GetIsNotNullMethod(valueType);

            return Expression.Call(isNotNullMethod, expression, NoErrorMessage());
        }

        private static MethodInfo GetIsNotNullMethod(Type valueType)
        {
            var isNotNullMethod =
                from method in typeof(BasicConstraints).GetMethods(BindingFlags.Public | BindingFlags.Static)
                where method.IsGenericMethodDefinition && method.Name == "IsNotNull"
                let parameters = method.GetParameters()
                where parameters.Length == 2
                where parameters[0].ParameterType.IsGenericType
                where parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IMemberConstraintBuilder<,>)
                where parameters[1].ParameterType == typeof(string)
                select method.MakeGenericMethod(typeof(T), valueType);

            return isNotNullMethod.Single();
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
