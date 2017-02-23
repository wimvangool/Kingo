using System;
using System.Linq.Expressions;

namespace Kingo.Messaging.Constraints.Decoders
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

        protected internal ParameterExpression InstanceParameter
        {
            get { return FieldOrPropertyExpression.Parameters[0]; }
        }

        protected internal abstract ParameterExpression PrimaryParameter
        {
            get;
        }

        #region [====== Member ======]

        private IMemberConstraintBuilder<T, TValue> Member
        {
            get { return _member.Value; }
        }

        private IMemberConstraintBuilder<T, TValue> CreateMember()
        {
            return CreateMemberExpression().Compile().Invoke();
        }

        private Expression<Func<IMemberConstraintBuilder<T, TValue>>> CreateMemberExpression()
        {
            //                          Left          Right
            // [m => m]         becomes [m => m]    + <null>
            // [m => m.A]       becomes [m => m.A]  + [a => a]
            // [m => m.A.B]     becomes [m => m.A]  + [a => a.B]  
            // [m => m[0]]      becomes [m => m][0] + <null>
            // [m => m.A[0]]    becomes [m => m.A]  + [a => a[0]]                                                         

            // [m => m]     becomes VerifyThat(m => m, null)    or And((m, x) => x, null)
            // [m => m[0]]  becomes VerifyThat(m => m, null)    or And((m, x) => x, null)
            // [m => m.A]   becomes VerifyThat(m => m.A, "A")   or And((m, a) => a, "A")                        
            return _expressionBuilder.BuildLeftExpression().BuildRightExpression().BuildMethodCallExpression();
        }

        internal LambdaExpression CreateLeftExpression(Expression expressionBody)
        {
            return CreateLeftExpression(expressionBody, PrimaryParameter);
        }

        protected abstract LambdaExpression CreateLeftExpression(Expression expressionBody, ParameterExpression primaryParameter);

        protected internal abstract MethodCallExpression CreateMethodCallExpression(LambdaExpression leftExpression, Identifier fieldOrPropertyName);        

        #endregion        

        #region [====== MemberConstraintBuilder<T, TValue>.IMemberConstraint<T> ======]

        public abstract Guid Key
        {
            get;
        } 

        public bool WriteErrorMessages(T instance, IErrorMessageReader reader)
        {
            return Member.WriteErrorMessages(instance, reader);
        }                      

        #endregion

        #region [====== MemberConstraintBuilder<T, TValue>.And ======]

        public IMemberConstraintBuilder<T, TMember> And<TMember>(Expression<Func<T, TValue, TMember>> fieldOrProperty)
        {
            return Member.And(fieldOrProperty);
        }

        public IMemberConstraintBuilder<T, TMember> And<TMember>(Func<T, TValue, TMember> fieldOrProperty, string fieldOrPropertyName)
        {
            return Member.And(fieldOrProperty, fieldOrPropertyName);
        }

        public IMemberConstraintBuilder<T, TMember> And<TMember>(Func<T, TValue, TMember> fieldOrProperty, Identifier fieldOrPropertyName)
        {
            return Member.And(fieldOrProperty, fieldOrPropertyName);
        }

        public void And(Action<IMemberConstraintSet<TValue>> innerConstraintFactory)
        {
            Member.And(innerConstraintFactory);
        }

        #endregion

        #region [====== MemberConstraintBuilder<T, TValue>.IsInstanceOf ======]

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

        #region [====== MemberConstraintBuilder<T, TValue>.HasItem ======]

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

        #region [====== MemberConstraintBuilder<T, TValue>.Satisfies ======]

        public IMemberConstraintBuilder<T, TValue> Satisfies(Predicate<TValue> constraint, string errorMessage = null, object errorMessageArgument = null)
        {
            return Member.Satisfies(constraint, errorMessage, errorMessageArgument);
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
