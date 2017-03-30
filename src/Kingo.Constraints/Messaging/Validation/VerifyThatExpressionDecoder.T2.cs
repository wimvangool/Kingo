using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.Messaging.Validation
{
    internal sealed class VerifyThatExpressionDecoder<T, TValue> : MemberExpressionDecoder<T, TValue>
    {
        private readonly Guid _key;
        private readonly IMemberConstraintSet<T> _constraintSet;
        private readonly Expression<Func<T, TValue>> _fieldOrProperty;

        internal VerifyThatExpressionDecoder(IMemberConstraintSet<T> constraintSet, Expression<Func<T, TValue>> fieldOrProperty)
        {
            if (fieldOrProperty == null)
            {
                throw new ArgumentNullException(nameof(fieldOrProperty));
            }
            _key = Guid.NewGuid();
            _constraintSet = constraintSet;
            _fieldOrProperty = fieldOrProperty;
        }

        public override Guid Key
        {
            get { return _key; }
        }

        protected internal override LambdaExpression FieldOrPropertyExpression
        {
            get { return _fieldOrProperty; }
        }

        protected internal override ParameterExpression PrimaryParameter
        {
            get { return InstanceParameter; }
        }

        protected override LambdaExpression CreateLeftExpression(Expression expressionBody, ParameterExpression primaryParameter)
        {
            return Expression.Lambda(expressionBody, primaryParameter);
        }

        #region [====== VerifyThat ======]

        protected internal override MethodCallExpression CreateMethodCallExpression(LambdaExpression leftExpression, Identifier fieldOrPropertyName)
        {
            // IMemberConstraintSet<T>.VerifyThat<TValue>(Func<T, TValue>, Identifier).
            var verifyThatMethod = GetVerifyThatMethod(_constraintSet.GetType(), leftExpression.ReturnType);            

            return Expression.Call(
                Expression.Constant(_constraintSet),
                verifyThatMethod,
                leftExpression,
                Expression.Constant(fieldOrPropertyName, typeof(Identifier))
            ); 
        }                               

        private static MethodInfo GetVerifyThatMethod(Type memberConstraintSetType, Type valueType)
        {
            // IMemberConstraintSet<T>.VerifyThat<TValue>(Func<T, TValue>, Identifier).
            var verifyThatMethod =
                from method in memberConstraintSetType.GetMethods(BindingFlags.Public | BindingFlags.Instance)                
                where method.IsGenericMethodDefinition && method.Name == "VerifyThat"
                let parameters = method.GetParameters()
                where parameters.Length == 2
                where parameters[0].ParameterType.IsGenericType
                where parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>)
                where parameters[1].ParameterType == typeof(Identifier)
                select method.MakeGenericMethod(valueType);

            return verifyThatMethod.Single();
        }

        #endregion          
    }
}
