using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace Kingo.DynamicMethods
{
    /// <summary>
    /// Represents an implementation of the <see cref="object.Equals(object)" /> method where two instances are considered
    /// equal based on their members.
    /// </summary>
    public abstract class EqualsMethod : DynamicMethod<EqualsMethod>
    {
        #region [====== MemberType ======]

        private enum MemberType
        {
            ReferenceType,

            ValueType,

            EnumerableType,

            DictionaryType,

            GenericEnumerableType,

            GenericDictionaryType
        }

        #endregion

        #region [====== Instance Members ======]

        internal EqualsMethod() { }

        internal abstract bool Execute(object left, object right);

        #endregion

        #region [====== Static Members ======]        

        /// <summary>
        /// Compares two instances and determines whether they are equal or not based on their members.
        /// </summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if both <paramref name="left"/> and <paramref name="right"/> are the same instance (or <c>null</c>),
        /// or when <paramref name="left"/> and <paramref name="right"/> are of the same type and have equal members. Otherwise
        /// <c>false</c>.
        /// </returns>
        public static bool Invoke(object left, object right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
		    {
                return false;
            }
            if (left.GetType() == right.GetType())
            {
                return HaveEqualMembers(left, right, right.GetType());
            }
            return false;
        }        

        private static bool HaveEqualMembers(object left, object right, Type type)
        {
            return GetOrAddMethod(type, Build).Execute(left, right);
        }                      

        private static EqualsMethod Build(Type type, MemberProvider memberProvider)
        {
            var left = Expression.Parameter(type, "left");
            var right = Expression.Parameter(type, "right");
            var expression = BuildExpression(left, right, memberProvider.Fields.ToArray(), memberProvider.Properties.ToArray());
            
            return FromExpressionMethod(type).Invoke(null, new object[] { expression, left, right }) as EqualsMethod;            
        }

        private static Expression BuildExpression(Expression left, Expression right, IReadOnlyCollection<FieldInfo> fields, IReadOnlyCollection<PropertyInfo> properties)
        {
            if (fields.Count + properties.Count == 0)
            {
                return Expression.Constant(true);
            }
            Expression expression = null;

            foreach (var field in fields)
            {
                var leftField = Expression.Field(left, field);
                var rightField = Expression.Field(right, field);

                expression = AreEqual(leftField, rightField, field.FieldType,expression);
            }
            foreach (var property in properties)
            {
                var leftProperty = Expression.Property(left, property);
                var rightProperty = Expression.Property(right, property);

                expression = AreEqual(leftProperty, rightProperty, property.PropertyType, expression);
            }
            return expression;
        }

        private static MethodInfo FromExpressionMethod(Type type)
        {
            var equalsMethodTypeDefinition = typeof(EqualsMethod<>);
            var equalsMethodType = equalsMethodTypeDefinition.MakeGenericType(type);
            var methods =
                from method in equalsMethodType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                where method.Name == "FromExpression" && method.GetParameters().Length == 3
                select method;

            return methods.Single();
        }       

        private static Expression AreEqual(Expression left, Expression right, Type type, Expression equalsExpression)
        {
            if (equalsExpression == null)
            {
                return AreEqual(left, right, type);
            }
            return Expression.AndAlso(equalsExpression, AreEqual(left, right, type));
        }

        private static Expression AreEqual(Expression left, Expression right, Type type)
        {
            switch (DetectMemberType(ref type))
            {
                case MemberType.ValueType:
                    return AreEqualValueTypes(left, right, type);

                case MemberType.EnumerableType:
                    return AreEqualEnumerableTypes(left, right);

                case MemberType.GenericEnumerableType:
                    return AreEqualGenericEnumerableTypes(left, right, type);

                case MemberType.DictionaryType:
                    return AreEqualDictionaryTypes(left, right);

                case MemberType.GenericDictionaryType:
                    return AreEqualGenericDictionaryTypes(left, right, type);

                default:
                    return AreEqualReferenceTypes(left, right);
            }
        }

        private static MemberType DetectMemberType(ref Type type)
        {
            if (type.IsValueType)
            {
                return MemberType.ValueType;
            }
            if (IsGenericDictionaryType(ref type))
            {
                return MemberType.GenericDictionaryType;
            }
            if (IsDictionaryType(type))
            {
                return MemberType.DictionaryType;
            }
            if (IsGenericEnumerableType(ref type))
            {
                return MemberType.GenericEnumerableType;
            }
            if (IsEnumerableType(type))
            {
                return MemberType.EnumerableType;
            }
            return MemberType.ReferenceType;
        }

        private static bool IsGenericDictionaryType(ref Type type)
        {
            if (IsGenericDictionaryInterfaceType(type))
            {
                return true;
            }
            var interfaceTypes =
                from interfaceType in type.GetInterfaces()
                where IsGenericDictionaryInterfaceType(interfaceType)
                select interfaceType;

            var matchingType = interfaceTypes.FirstOrDefault();
            if (matchingType == null)
            {
                return false;
            }
            type = matchingType;
            return true;
        }

        private static bool IsGenericDictionaryInterfaceType(Type type)
        {
            return type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>);
        }

        private static bool IsDictionaryType(Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type);
        }

        private static bool IsGenericEnumerableType(ref Type type)
        {
            if (IsGenericEnumerableInterfaceType(type))
            {
                return true;
            }
            var interfaceTypes =
                from interfaceType in type.GetInterfaces()
                where IsGenericEnumerableInterfaceType(interfaceType)
                select interfaceType;

            var matchingType = interfaceTypes.FirstOrDefault();
            if (matchingType == null)
            {
                return false;
            }
            type = matchingType;
            return true;
        }

        private static bool IsGenericEnumerableInterfaceType(Type type)
        {
            return type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static bool IsEnumerableType(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        #region [====== Value Types ======]

        private static Expression AreEqualValueTypes(Expression left, Expression right, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return AreEqualReferenceTypes(Box(left), Box(right));
            }
            return Expression.Call(left, GetEqualsMethod(type), right);
        }

        private static Expression Box(Expression expression)
        {
            return Expression.Convert(expression, typeof(object));
        }

        private static MethodInfo GetEqualsMethod(Type type)
        {
            var typeSafeMethod = GetEqualsMethod(type, type);
            if (typeSafeMethod == null)
            {
                return GetEqualsMethod(type, typeof(object));
            }
            return typeSafeMethod;
        }

        private static MethodInfo GetEqualsMethod(Type type, Type parameterType)
        {
            return type.GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance, null, new[] { parameterType }, null);
        }

        #endregion

        #region [====== Reference Types ======]

        private static readonly MethodInfo _EqualsMethod = typeof(object).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static);

        private static Expression AreEqualReferenceTypes(Expression left, Expression right)
        {
            return Expression.Call(null, _EqualsMethod, left, right);
        }

        #endregion

        #region [====== Collection Types (Enumerables) ======]

        private static readonly MethodInfo _AreEqualEnumerableTypesMethod = FindAreEqualEnumerableTypesMethod();
        private static readonly MethodInfo _AreEqualGenericEnumerableTypesMethod = FindAreEqualGenericEnumerableTypesMethod();

        private static MethodInfo FindAreEqualEnumerableTypesMethod()
        {
            var methods =
                from method in typeof(EqualsMethod).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                where method.Name == "AreEqualEnumerableTypes"
                let parameters = method.GetParameters()
                where parameters.Length == 2 && IsOfEnumerableType(parameters[0]) && IsOfEnumerableType(parameters[1])
                select method;

            return methods.Single();
        }

        private static bool IsOfEnumerableType(ParameterInfo parameter)
        {
            return parameter.ParameterType == typeof(IEnumerable);
        }

        private static MethodInfo FindAreEqualGenericEnumerableTypesMethod()
        {
            var methods =
                from method in typeof(EqualsMethod).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                where method.IsGenericMethod && method.IsGenericMethodDefinition
                where method.Name == "AreEqualGenericEnumerableTypes"
                where method.GetParameters().Length == 2
                select method;

            return methods.Single();
        }

        private static Expression AreEqualEnumerableTypes(Expression left, Expression right)
        {
            return Expression.Call(null, _AreEqualEnumerableTypesMethod, left, right);
        }

        [UsedImplicitly]
        private static bool AreEqualEnumerableTypes(IEnumerable left, IEnumerable right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            if (ReferenceEquals(right, null))
            {
                return false;
            }
            return left.Cast<object>().SequenceEqual(right.Cast<object>());
        }

        private static Expression AreEqualGenericEnumerableTypes(Expression left, Expression right, Type interfaceType)
        {
            var valueType = interfaceType.GetGenericArguments()[0];
            var method = _AreEqualGenericEnumerableTypesMethod.MakeGenericMethod(valueType);

            return Expression.Call(null, method, left, right);
        }

        [UsedImplicitly]
        private static bool AreEqualGenericEnumerableTypes<TValue>(IEnumerable<TValue> left, IEnumerable<TValue> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            if (ReferenceEquals(right, null))
            {
                return false;
            }
            return left.SequenceEqual(right);
        }

        #endregion

        #region [====== Collection Types (Dictionaries) ======]

        private static readonly MethodInfo _AreEqualDictionaryTypesMethod = FindAreEqualDictionaryTypesMethod();
        private static readonly MethodInfo _AreEqualGenericDictionaryTypesMethod = FindAreEqualGenericDictionaryTypesMethod();

        private static MethodInfo FindAreEqualDictionaryTypesMethod()
        {
            var methods =
                from method in typeof(EqualsMethod).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                where method.Name == "AreEqualDictionaryTypes"
                let parameters = method.GetParameters()
                where parameters.Length == 2 && IsOfDictionaryType(parameters[0]) && IsOfDictionaryType(parameters[1])
                select method;

            return methods.Single();
        }

        private static bool IsOfDictionaryType(ParameterInfo parameter)
        {
            return parameter.ParameterType == typeof(IDictionary);
        }

        private static MethodInfo FindAreEqualGenericDictionaryTypesMethod()
        {
            var methods =
                from method in typeof(EqualsMethod).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                where method.IsGenericMethod && method.IsGenericMethodDefinition
                where method.Name == "AreEqualGenericDictionaryTypes"
                where method.GetParameters().Length == 2
                select method;

            return methods.Single();
        }

        private static Expression AreEqualDictionaryTypes(Expression left, Expression right)
        {
            return Expression.Call(null, _AreEqualDictionaryTypesMethod, left, right);
        }

        [UsedImplicitly]
        private static bool AreEqualDictionaryTypes(IDictionary left, IDictionary right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            if (ReferenceEquals(right, null))
            {
                return false;
            }
            if (left.Count == right.Count)
            {
                return HaveEqualKeyValuePairs(left, right);
            }
            return false;
        }

        private static bool HaveEqualKeyValuePairs(IDictionary left, IDictionary right)
        {
            foreach (var key in left.Keys)
            {
                if (!right.Contains(key) || !Equals(left[key], right[key]))
                {
                    return false;
                }
            }
            return true;
        }

        private static Expression AreEqualGenericDictionaryTypes(Expression left, Expression right, Type interfaceType)
        {
            var parameters = interfaceType.GetGenericArguments();
            var keyType = parameters[0];
            var valueType = parameters[1];
            var method = _AreEqualGenericDictionaryTypesMethod.MakeGenericMethod(keyType, valueType);

            return Expression.Call(null, method, left, right);
        }

        [UsedImplicitly]
        private static bool AreEqualGenericDictionaryTypes<TKey, TValue>(IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            if (ReferenceEquals(right, null))
            {
                return false;
            }
            if (left.Count == right.Count)
            {
                return HaveEqualKeyValuePairs(left, right);
            }
            return false;
        }

        private static bool HaveEqualKeyValuePairs<TKey, TValue>(IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right)
        {
            foreach (var key in left.Keys)
            {
                TValue value;

                if (!right.TryGetValue(key, out value) || !Equals(left[key], value))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion          

        #endregion
    }
}
