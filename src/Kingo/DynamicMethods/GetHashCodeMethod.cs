using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace Kingo.DynamicMethods
{
    /// <summary>
    /// Represents an implementation of the <see cref="object.GetHashCode()" /> method, where the hashcode is calculated
    /// by XOR-ing the hashcode of the <see cref="Type" /> with all readonly, value-type fields.
    /// </summary>
    public abstract class GetHashCodeMethod : DynamicMethod<GetHashCodeMethod>
    {
        #region [====== Instance Members ======]

        internal GetHashCodeMethod() { }

        internal abstract int Execute(object instance);

        #endregion

        #region [====== Static Members ======]

        private const BindingFlags _PublicInstance = BindingFlags.Public | BindingFlags.Instance;
        private static readonly MethodInfo _GetTypeMethod = typeof(object).GetMethod("GetType", _PublicInstance);
        private static readonly MethodInfo _GetHashCodeMethod = typeof(object).GetMethod("GetHashCode", _PublicInstance);
        private static readonly MethodInfo _GetHashCodeOfMethod = typeof(GetHashCodeMethod).GetMethod("GetHashCodeOf", BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// Calculates a hashcode for the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The object to calculate the hashcode for.</param>
        /// <returns>
        /// The hashcode of the specified <paramref name="instance"/>, based on all immutable members of this object.
        /// </returns>
        public static int Invoke(object instance)
        {
            if (ReferenceEquals(instance, null))
            {
                return 0;
            }
            return GetHashCodeFromMembers(instance);
        }

        private static int GetHashCodeFromMembers(object instance) =>
             GetOrAddMethod(instance.GetType(), Build).Execute(instance);

        private static GetHashCodeMethod Build(Type type, MemberProvider memberProvider)
        {
            var instance = Expression.Parameter(type, "instance");
            var expression = BuildExpression(instance, memberProvider.Fields.Where(IsImmutable).ToArray());

            return FromExpressionMethod(type).Invoke(null, new object[] { expression, instance }) as GetHashCodeMethod;
        }

        private static Expression BuildExpression(Expression instance, IReadOnlyCollection<FieldInfo> fields)
        {
            Expression expression = Expression.Call(Expression.Call(instance, _GetTypeMethod), _GetHashCodeMethod);

            foreach (var field in fields)
            {
                expression = XorHashCodes(expression, Expression.Field(instance, field));
            }
            return expression;
        }

        private static Expression XorHashCodes(Expression left, Expression right) =>
             Expression.MakeBinary(ExpressionType.ExclusiveOr, left, GetHashCodeExpression(right));

        private static Expression GetHashCodeExpression(Expression instance)
        {
            if (instance.Type.IsValueType)
            {
                return Expression.Call(instance, _GetHashCodeMethod);
            }
            return Expression.Call(null, _GetHashCodeOfMethod, instance);
        }

        [UsedImplicitly]
        private static int GetHashCodeOf(object instance) =>
             instance == null ? 0 : instance.GetHashCode();

        private static bool IsImmutable(FieldInfo field) =>
             field.IsInitOnly && (field.FieldType.IsValueType || field.FieldType == typeof(string));

        private static MethodInfo FromExpressionMethod(Type type)
        {
            var hashCodeMethodTypeDefinition = typeof(GetHashCodeMethod<>);
            var hashCodeMethodType = hashCodeMethodTypeDefinition.MakeGenericType(type);
            var methods =
                from method in hashCodeMethodType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                where method.Name == "FromExpression" && method.GetParameters().Length == 2
                select method;

            return methods.Single();
        }

        #endregion
    }
}
