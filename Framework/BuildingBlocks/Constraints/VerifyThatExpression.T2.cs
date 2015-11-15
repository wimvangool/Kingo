using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class VerifyThatExpression<T, TValue> : IMemberConstraintBuilder<T, TValue>
    {
        private readonly IMemberConstraintSet<T> _constraintSet;
        private readonly Lazy<IMemberConstraintBuilder<T, TValue>> _member;

        internal VerifyThatExpression(IMemberConstraintSet<T> constraintSet, Expression<Func<T, TValue>> fieldOrProperty)
        {
            if (fieldOrProperty == null)
            {
                throw new ArgumentNullException("fieldOrProperty");
            }
            _constraintSet = constraintSet;
            _member = new Lazy<IMemberConstraintBuilder<T, TValue>>(() => CreateMemberConstraint(fieldOrProperty));
        }        

        #region [====== Member & Expression Analysis ======]

        private IMemberConstraintBuilder<T, TValue> Member
        {
            get { return _member.Value; }
        }

        private IMemberConstraintBuilder<T, TValue> CreateMemberConstraint(Expression<Func<T, TValue>> fieldOrProperty)
        {                        
            return CreateVerifyThatExpression(fieldOrProperty).Compile().Invoke();                                    
        }

        private Expression<Func<IMemberConstraintBuilder<T, TValue>>> CreateVerifyThatExpression(Expression<Func<T, TValue>> memberExpression)
        {
            //                          Left          Right
            // [m => m]         becomes [m => m]    + <null>
            // [m => m.A]       becomes [m => m.A]  + [a => a]
            // [m => m.A.B]     becomes [m => m.A]  + [a => a.B]  
            // [m => m[0]]      becomes [m => m][0] + <null>
            // [m => m.A[0]]    becomes [m => m.A]  + [a => a[0]]
            var memberExpressionPair = MemberExpressionPair.SplitUp(memberExpression);
            var left = memberExpressionPair.Left;
            var right = memberExpressionPair.Right;

            // [m => m]     becomes _constraintSet.VerifyThat(m => m, null)
            // [m => m[0]]  becomes _constraintSet.VerifyThat(m => m[0], null)
            // [m => m.A]   becomes _constraintSet.VerifyThat(m => m.A, "A")
            var methodCallExpression = CreateVerifyThatMethodCallExpression(left);            
            
            if (left.IsIndexer)
            {
                // Append <left_expression>.HasItem<TItem>(new IndexListFactory<T> { <arguments> })              
                AppendHasItem(ref methodCallExpression, typeof(TValue), left.Parameter, left.IndexerArguments);                
            }
            if (right != null && !right.IsTrivialParameterExpression())
            {
                // Append <left_expression>.IsNotNull()
                if (AppendIsNotNull(ref methodCallExpression))
                {
                    // Append <left_expression>.And(<right_expression>);
                    AppendAnd(ref methodCallExpression, right.Expression);    
                }                
            }                    
            return ConvertToLambda(methodCallExpression);
        }                   

        private MethodCallExpression CreateVerifyThatMethodCallExpression(MemberExpressionLeftNode left)
        {
            // IMemberConstraintSet<T>.VerifyThat<TValue>(Func<T, TValue>, Identifier).
            var verifyThatMethod = GetVerifyThatMethod(_constraintSet.GetType(), left.Expression.ReturnType);
            var fieldOrPropertyName = left.IsMemberAccess ? left.Expression.ExtractMemberName() : null;

            return Expression.Call(
                Expression.Constant(_constraintSet),
                verifyThatMethod,
                left.Expression,
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

        private static void AppendHasItem(ref MethodCallExpression methodCallExpression, Type itemType, ParameterExpression parameter, IReadOnlyList<Expression> indexerArguments)
        {
            // IMemberConstraintBuilder<T, TValue>.HasItem<TItem>(IndexListFactory<T>, string).
            var valueType = GetValueType(methodCallExpression.Type);
            var hasItemMethod = GetHasItemMethod(valueType, itemType);
            var hasItemArguments = CreateHasItemArgumentExpression(parameter, indexerArguments);

            methodCallExpression = Expression.Call(methodCallExpression, hasItemMethod, hasItemArguments, NoErrorMessage());
        }

        private static MethodInfo GetHasItemMethod(Type valueType, Type itemType)
        {
            // IMemberConstraintBuilder<T, TValue>.HasItem<TItem>(IndexListFactory<T>, string).
            var builderType = MakeMemberConstraintBuilderType(valueType);

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
     
        private static bool AppendIsNotNull(ref MethodCallExpression methodCallExpression)
        {
            MethodInfo isNotNullMethod;

            var valueType = GetValueType(methodCallExpression.Type);
            if (valueType.IsValueType)
            {
                if (IsNullable(valueType))
                {
                    isNotNullMethod = GetIsNotNullMethod(typeof(NullableConstraints), valueType.GetGenericArguments()[0]);
                    methodCallExpression = Expression.Call(isNotNullMethod, methodCallExpression, NoErrorMessage());
                    return false;
                }                
                return true;
            }
            isNotNullMethod = GetIsNotNullMethod(typeof(BasicConstraints), valueType);
            methodCallExpression = Expression.Call(isNotNullMethod, methodCallExpression, NoErrorMessage());
            return true;            
        }        

        private static bool IsNullable(Type valueType)
        {
            return valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static MethodInfo GetIsNotNullMethod(Type classType, Type valueType)
        {
            // IMemberConstraintBuilder<T, TValue>.IsNotNull<TValue>(string).
            var isNotNullMethod =
                from method in classType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                where method.IsGenericMethodDefinition && method.Name == "IsNotNull"
                let parameters = method.GetParameters()
                where parameters.Length == 2                
                where parameters[0].ParameterType.IsGenericType
                where parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IMemberConstraintBuilder<,>)
                where parameters[1].ParameterType == typeof(string)
                select method.MakeGenericMethod(typeof(T), valueType);

            return isNotNullMethod.Single();
        }
      
        private static void AppendAnd(ref MethodCallExpression methodCallExpression, Expression rightExpression)
        {
            // IMemberConstraintBuilder<T, TValue>.And<TOther>(Expression<Func<TValue, TOther>>).
            var valueType = GetValueType(methodCallExpression.Type);
            var otherType = GetValueType(rightExpression.Type);
            var andMethod = GetAndMethod(valueType, otherType);

            methodCallExpression = Expression.Call(methodCallExpression, andMethod, Expression.Quote(rightExpression));
        }

        private static MethodInfo GetAndMethod(Type valueType, Type memberType)
        {
            // IMemberConstraintBuilder<T, TValue>.And<TOther>(Expression<Func<TValue, TOther>>).
            var builderType = MakeMemberConstraintBuilderType(valueType);

            var andMethod =
                from method in builderType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                where method.IsGenericMethodDefinition && method.Name == "And"
                let parameters = method.GetParameters()
                where parameters.Length == 1
                where parameters[0].ParameterType.IsGenericType
                where parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>)
                select method.MakeGenericMethod(memberType);

            return andMethod.Single();
        }        

        private static Type GetValueType(Type builderType)
        {
            return builderType.GetGenericArguments()[1];
        }

        private static Type MakeMemberConstraintBuilderType(Type valueType)
        {
            return typeof(IMemberConstraintBuilder<,>).MakeGenericType(typeof(T), valueType);
        }

        private static Expression NoErrorMessage()
        {
            return Expression.Constant(null, typeof(string));
        }

        private static Expression<Func<IMemberConstraintBuilder<T, TValue>>> ConvertToLambda(MethodCallExpression methodCallExpression)
        {
            return Expression.Lambda<Func<IMemberConstraintBuilder<T, TValue>>>(methodCallExpression);
        } 

        #endregion

        #region [====== IMemberConstraint<T> ======]

        IMember IMemberConstraintBuilder<T>.Member
        {
            get { return Member.Member; }
        }

        public bool WriteErrorMessages(T instance, IErrorMessageReader reader)
        {
            return Member.WriteErrorMessages(instance, reader);
        }

        #endregion

        #region [====== And ======]

        public IMemberConstraintBuilder<T, TMember> And<TMember>(Expression<Func<TValue, TMember>> fieldOrProperty)
        {
            return Member.And(fieldOrProperty);
        }

        public IMemberConstraintBuilder<T, TMember> And<TMember>(Func<TValue, TMember> fieldOrProperty, string fieldOrPropertyName)
        {
            return Member.And(fieldOrProperty, fieldOrPropertyName);
        }

        public IMemberConstraintBuilder<T, TMember> And<TMember>(Func<TValue, TMember> fieldOrProperty, Identifier fieldOrPropertyName)
        {
            return Member.And(fieldOrProperty, fieldOrPropertyName);
        }

        public void And(Action<IMemberConstraintSet<TValue>> innerConstraintFactory)
        {
            Member.And(innerConstraintFactory);
        }

        #endregion

        #region [====== IsInstanceOf ======]

        public IMemberConstraintBuilder<T, TValue> IsNotInstanceOf<TOther>(string errorMessage = null)
        {
            return Member.IsNotInstanceOf<TOther>(errorMessage);
        }

        public IMemberConstraintBuilder<T, TOther> IsInstanceOf<TOther>(string errorMessage = null)
        {
            return Member.IsInstanceOf<TOther>(errorMessage);
        }

        public IMemberConstraintBuilder<T, TOther> As<TOther>() where TOther : class
        {
            return Member.As<TOther>();
        }

        #endregion

        #region [====== HasItem ======]

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(int index, string errorMessage = null)
        {
            return Member.HasItem<TItem>(index, errorMessage);
        }

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(string index, string errorMessage = null)
        {
            return Member.HasItem<TItem>(index, errorMessage);
        }

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndex>(TIndex index, string errorMessage = null)
        {
            return Member.HasItem<TItem, TIndex>(index, errorMessage);
        }

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndexA, TIndexB>(TIndexA indexA, TIndexB indexB, string errorMessage = null)
        {
            return Member.HasItem<TItem, TIndexA, TIndexB>(indexA, indexB, errorMessage);
        }

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndex>(Func<T, TIndex> indexFactory, string errorMessage = null)
        {
            return Member.HasItem<TItem, TIndex>(indexFactory, errorMessage);
        }

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndexA, TIndexB>(Func<T, TIndexA> indexAFactory, Func<T, TIndexB> indexBFactory, string errorMessage = null)
        {
            return Member.HasItem<TItem, TIndexA, TIndexB>(indexAFactory, indexBFactory, errorMessage);
        }

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(IndexListFactory<T> indexListFactory, string errorMessage = null)
        {
            return Member.HasItem<TItem>(indexListFactory, errorMessage);
        }

        #endregion

        #region [====== Satisfies ======]

        public IMemberConstraintBuilder<T, TValue> Satisfies(Func<TValue, bool> constraint, string errorMessage = null)
        {
            return Member.Satisfies(constraint, errorMessage);
        }

        public IMemberConstraintBuilder<T, TValue> Satisfies(IConstraint<TValue> constraint)
        {
            return Member.Satisfies(constraint);
        }

        public IMemberConstraintBuilder<T, TValue> Satisfies(Func<T, IConstraint<TValue>> constraintFactory)
        {
            return Member.Satisfies(constraintFactory);
        }

        public IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(IFilter<TValue, TOther> constraint)
        {
            return Member.Satisfies(constraint);
        }

        public IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(Func<T, IFilter<TValue, TOther>> constraintFactory)
        {
            return Member.Satisfies(constraintFactory);
        }

        #endregion        
    }
}
