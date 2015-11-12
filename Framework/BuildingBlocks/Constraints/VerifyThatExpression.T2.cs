using System;
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
            //return CreateVerifyThatExpression(fieldOrProperty).Compile().Invoke();                        
            return _constraintSet.VerifyThat(fieldOrProperty.Compile(), fieldOrProperty.ExtractMemberName());
        }

        private Expression<Func<IMemberConstraintBuilder<T, TValue>>> CreateVerifyThatExpression(Expression<Func<T, TValue>> memberExpression)
        {
            // [m => m.A.B.C] becomes [m => m.A] + [a => a.B.C]
            var memberExpressionPair = MemberExpressionPair.SplitUp(memberExpression);

            // _constraintSet.VerifyThat(m => m.A, "A")
            var verifyThatMethodCallExpression = CreateVerifyThatMethodCallExpression(memberExpressionPair.LeftExpression);

            // If remainder [a => a.B.C] exists, then do _contraintSet.VerifyThat(m => m.A).IsNotNull().And(a => a.B.C).
            var remainderExpression = memberExpressionPair.RightExpression;
            if (remainderExpression == null)
            {
                return Expression.Lambda<Func<IMemberConstraintBuilder<T, TValue>>>(verifyThatMethodCallExpression);
            }
            throw new NotImplementedException();
        }        

        private MethodCallExpression CreateVerifyThatMethodCallExpression(LambdaExpression memberExpression)
        {                       
            var verifyThatMethod = GetVerifyThatMethod(_constraintSet.GetType(), memberExpression.ReturnType);

            return Expression.Call(
                Expression.Constant(_constraintSet),
                verifyThatMethod,
                memberExpression,
                Expression.Constant(memberExpression.ExtractMemberName())
            );            
        }                

        private static MethodInfo GetVerifyThatMethod(Type memberConstraintSetType, Type valueType)
        {
            var verifyThatMethod =
                from method in memberConstraintSetType.GetMethods(BindingFlags.Public | BindingFlags.Instance)                
                where method.IsGenericMethodDefinition && method.Name == "VerifyThat"
                let parameters = method.GetParameters()
                where parameters.Length == 2 && parameters[1].ParameterType == typeof(Identifier)
                select method.MakeGenericMethod(valueType);

            return verifyThatMethod.Single();
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
