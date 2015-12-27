using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.Constraints.Decoders
{
    internal sealed class AndExpressionDecoder<T, TValueOut, TOther> : MemberExpressionDecoder<T, TOther>
    {
        private readonly IMemberConstraintBuilder<T, TValueOut> _builder;
        private readonly Expression<Func<T, TValueOut, TOther>> _fieldOrProperty;

        internal AndExpressionDecoder(IMemberConstraintBuilder<T, TValueOut> builder, Expression<Func<T, TValueOut, TOther>> fieldOrProperty)
        {
            if (fieldOrProperty == null)
            {
                throw new ArgumentNullException("fieldOrProperty");
            }
            _builder = builder;
            _fieldOrProperty = fieldOrProperty;
        }

        public override Guid Key
        {
            get { return _builder.Key; }
        }

        protected internal override LambdaExpression FieldOrPropertyExpression
        {
            get { return _fieldOrProperty; }
        }

        protected internal override ParameterExpression PrimaryParameter
        {
            get { return FieldOrPropertyExpression.Parameters[1]; }
        }

        protected override LambdaExpression CreateLeftExpression(Expression expressionBody, ParameterExpression primaryParameter)
        {
            return Expression.Lambda(expressionBody, InstanceParameter, primaryParameter);
        }

        #region [====== And ======]

        protected internal override MethodCallExpression CreateMethodCallExpression(LambdaExpression leftExpression, Identifier fieldOrPropertyName)
        {
            // IMemberConstraintBuilder<T, TValueOut>.And<TOther>(Func<TValueOut, TOther>, Identifier)
            var andMethod = GetAndMethod(typeof(TValueOut), leftExpression.ReturnType);            

            return Expression.Call(
                Expression.Constant(_builder),
                andMethod,
                leftExpression,
                Expression.Constant(fieldOrPropertyName, typeof(Identifier))
            );
        }        

        private static MethodInfo GetAndMethod(Type valueType, Type otherType)
        {
            // IMemberConstraintBuilder<T, TValueOut>.And<TOther>(Func<TValueOut, TOther>, Identifier)
            var builderType = typeof(IMemberConstraintBuilder<,>).MakeGenericType(typeof(T), valueType);

            var andMethod =
                from method in builderType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                where method.IsGenericMethodDefinition && method.Name == "And"
                let parameters = method.GetParameters()
                where parameters.Length == 2
                where parameters[0].ParameterType.IsGenericType
                where parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<,,>)
                where parameters[1].ParameterType == typeof(Identifier)
                select method.MakeGenericMethod(otherType);

            return andMethod.Single();
        }

        #endregion
    }
}
