using System;
using System.Linq.Expressions;

namespace Kingo.Messaging.Validation
{
    internal abstract class MemberExpressionDecoder<T, TValue> : IMemberConstraintBuilder<T, TValue>
    {              
        private readonly Lazy<IMemberConstraintBuilder<T, TValue>> _member;
        private readonly LeftExpressionBuilder<T, TValue> _expressionBuilder;

        protected MemberExpressionDecoder()
        {
            _member = new Lazy<IMemberConstraintBuilder<T, TValue>>(CreateMember);   
            _expressionBuilder = new LeftExpressionBuilder<T, TValue>(this);         
        }

        protected internal abstract LambdaExpression FieldOrPropertyExpression
        {
            get;
        }

        protected internal ParameterExpression InstanceParameter =>
            FieldOrPropertyExpression.Parameters[0];

        protected internal abstract ParameterExpression PrimaryParameter
        {
            get;
        }

        #region [====== Member ======]

        private IMemberConstraintBuilder<T, TValue> Member =>
            _member.Value;

        private IMemberConstraintBuilder<T, TValue> CreateMember() =>
            CreateMemberExpression().Compile().Invoke();

        private Expression<Func<IMemberConstraintBuilder<T, TValue>>> CreateMemberExpression() => _expressionBuilder.BuildLeftExpression().BuildRightExpression().BuildMethodCallExpression();

        internal LambdaExpression CreateLeftExpression(Expression expressionBody) =>
            CreateLeftExpression(expressionBody, PrimaryParameter);

        protected abstract LambdaExpression CreateLeftExpression(Expression expressionBody, ParameterExpression primaryParameter);

        protected internal abstract MethodCallExpression CreateMethodCallExpression(LambdaExpression leftExpression, Identifier fieldOrPropertyName);        

        #endregion        

        #region [====== MemberConstraintBuilder<T, TValue>.IMemberConstraint<T> ======]

        public abstract Guid Key
        {
            get;
        } 

        public bool WriteErrorMessages(T instance, IErrorMessageCollection reader, bool haltOnFirstError) =>
            Member.WriteErrorMessages(instance, reader, haltOnFirstError);

        #endregion

        #region [====== MemberConstraintBuilder<T, TValue>.And ======]

        public IMemberConstraintBuilder<T, TMember> And<TMember>(Expression<Func<T, TValue, TMember>> fieldOrProperty) =>
            Member.And(fieldOrProperty);

        public IMemberConstraintBuilder<T, TMember> And<TMember>(Func<T, TValue, TMember> fieldOrProperty, string fieldOrPropertyName) =>
            Member.And(fieldOrProperty, fieldOrPropertyName);

        public IMemberConstraintBuilder<T, TMember> And<TMember>(Func<T, TValue, TMember> fieldOrProperty, Identifier fieldOrPropertyName) =>
            Member.And(fieldOrProperty, fieldOrPropertyName);

        public void And(Action<IMemberConstraintSet<TValue>> innerConstraintFactory) =>
            Member.And(innerConstraintFactory);

        #endregion

        #region [====== MemberConstraintBuilder<T, TValue>.IsInstanceOf ======]

        public IMemberConstraintBuilder<T, TValue> IsNotInstanceOf<TOther>(string errorMessage = null) =>
            Member.IsNotInstanceOf<TOther>(errorMessage);

        public IMemberConstraintBuilder<T, TOther> IsInstanceOf<TOther>(string errorMessage = null) =>
            Member.IsInstanceOf<TOther>(errorMessage);

        public IMemberConstraintBuilder<T, TOther> As<TOther>() where TOther : class =>
            Member.As<TOther>();

        #endregion

        #region [====== MemberConstraintBuilder<T, TValue>.HasItem ======]

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(int index, string errorMessage = null) =>
            Member.HasItem<TItem>(index, errorMessage);

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(string index, string errorMessage = null) =>
            Member.HasItem<TItem>(index, errorMessage);

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndex>(TIndex index, string errorMessage = null) =>
            Member.HasItem<TItem, TIndex>(index, errorMessage);

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndexA, TIndexB>(TIndexA indexA, TIndexB indexB, string errorMessage = null) =>
            Member.HasItem<TItem, TIndexA, TIndexB>(indexA, indexB, errorMessage);

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndex>(Func<T, TIndex> indexFactory, string errorMessage = null) =>
            Member.HasItem<TItem, TIndex>(indexFactory, errorMessage);

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndexA, TIndexB>(Func<T, TIndexA> indexAFactory, Func<T, TIndexB> indexBFactory, string errorMessage = null) =>
            Member.HasItem<TItem, TIndexA, TIndexB>(indexAFactory, indexBFactory, errorMessage);

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(IndexListFactory<T> indexListFactory, string errorMessage = null) =>
            Member.HasItem<TItem>(indexListFactory, errorMessage);

        #endregion

        #region [====== MemberConstraintBuilder<T, TValue>.Satisfies ======]

        public IMemberConstraintBuilder<T, TValue> Satisfies(Predicate<TValue> constraint, string errorMessage = null, object errorMessageArgument = null) =>
            Member.Satisfies(constraint, errorMessage, errorMessageArgument);

        public IMemberConstraintBuilder<T, TValue> Satisfies(IConstraint<TValue> constraint) =>
            Member.Satisfies(constraint);

        public IMemberConstraintBuilder<T, TValue> Satisfies(Func<T, IConstraint<TValue>> constraintFactory) =>
            Member.Satisfies(constraintFactory);

        public IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(IFilter<TValue, TOther> constraint) =>
            Member.Satisfies(constraint);

        public IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(Func<T, IFilter<TValue, TOther>> constraintFactory) =>
            Member.Satisfies(constraintFactory);

        #endregion        
    }
}
