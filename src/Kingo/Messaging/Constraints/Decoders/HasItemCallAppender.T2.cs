using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.Messaging.Constraints.Decoders
{
    internal sealed class HasItemCallAppender<T, TValue> : MethodCallDecorator<T, TValue>
    {
        private readonly ParameterExpression _instanceParameter;
        private readonly IReadOnlyList<Expression> _indexerArguments;

        public HasItemCallAppender(ParameterExpression instanceParameter, IReadOnlyList<Expression> indexerArguments)
        {
            _instanceParameter = instanceParameter;
            _indexerArguments = indexerArguments;
        }

        internal override IEnumerable<string> MethodCalls()
        {
            yield return "HasItem<TItem>(<arguments>)";
        }

        public override MethodCallExpression Decorate(MethodCallExpression expression)
        {
            // IMemberConstraintBuilder<T, TValue>.HasItem<TItem>(IndexListFactory<T>, string).
            var valueType = GetGenericArgumentType(expression.Type, 1);
            var hasItemMethod = GetHasItemMethod(valueType, typeof(TValue));
            var hasItemArguments = CreateHasItemArgumentExpression(_instanceParameter, _indexerArguments);

            return Expression.Call(expression, hasItemMethod, hasItemArguments, NoErrorMessage());
        }

        private static MethodInfo GetHasItemMethod(Type valueType, Type itemType)
        {
            // IMemberConstraintBuilder<T, TValue>.HasItem<TItem>(IndexListFactory<T>, string).
            var builderType = typeof(IMemberConstraintBuilder<,>).MakeGenericType(typeof(T), valueType);

            var hasItemMethod =
                from method in builderType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                where method.IsGenericMethodDefinition && method.Name == "HasItem"
                let parameters = method.GetParameters()
                where parameters.Length == 2
                where parameters[0].ParameterType.IsGenericType
                where parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IndexListFactory<>)
                where parameters[1].ParameterType == typeof(string)
                select method.MakeGenericMethod(itemType);

            return hasItemMethod.Single();
        }

        private static Expression CreateHasItemArgumentExpression(ParameterExpression parameter, IReadOnlyList<Expression> indexerArguments)
        {
            var indexListFactory = new IndexListFactory<T>();

            foreach (var indexerArgument in indexerArguments)
            {
                AddIndexerArgumentTo(indexListFactory, parameter, indexerArgument);
            }
            return Expression.Constant(indexListFactory);
        }

        private static void AddIndexerArgumentTo(IndexListFactory<T> indexListFactory, ParameterExpression parameter, Expression indexerArgument)
        {
            // indexListFactory.Add<TValue>(Func<T, TValue>)
            var indexerFunc = Expression.Lambda(indexerArgument, parameter);
            var indexerType = indexerFunc.ReturnType;
            var addMethod = GetAddMethod(indexerType);

            addMethod.Invoke(indexListFactory, new object[] { indexerFunc.Compile() });
        }

        private static MethodInfo GetAddMethod(Type indexerType)
        {
            var addMethod =
                from method in typeof(IndexListFactory<T>).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                where method.IsGenericMethodDefinition && method.Name == "Add"
                let parameters = method.GetParameters()
                where parameters.Length == 1
                where parameters[0].ParameterType.IsGenericType
                where parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>)
                select method.MakeGenericMethod(indexerType);

            return addMethod.Single();
        }
    }
}
