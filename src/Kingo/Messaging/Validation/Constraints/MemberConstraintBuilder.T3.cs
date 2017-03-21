using System;
using System.Linq.Expressions;

namespace Kingo.Messaging.Validation.Constraints
{    
    internal sealed class MemberConstraintBuilder<T, TValueIn, TValueOut> : IMemberConstraintBuilder<T, TValueOut>
    {
        private readonly Guid _key;
        private readonly MemberConstraintSet<T> _memberConstraintSet;        
        private readonly MemberConstraintFactory<T, TValueIn, TValueOut> _memberConstraintFactory;                

        internal MemberConstraintBuilder(Guid key, MemberConstraintSet<T> memberConstraintSet, MemberConstraintFactory<T, TValueIn, TValueOut> memberConstraintFactory)
        {
            _key = key;
            _memberConstraintSet = memberConstraintSet;            
            _memberConstraintFactory = memberConstraintFactory;
        }

        #region [====== IMemberConstraint<T> ======]

        Guid IMemberConstraintBuilder<T>.Key =>
            _key;      

        bool IErrorMessageWriter<T>.WriteErrorMessages(T message, IErrorMessageReader reader, bool haltOnFirstError) =>
            _memberConstraintFactory.WriteErrorMessages(message, reader, haltOnFirstError);

        #endregion

        #region [====== And ======]

        public IMemberConstraintBuilder<T, TOther> And<TOther>(Expression<Func<T, TValueOut, TOther>> fieldOrProperty) =>
            new AndExpressionDecoder<T, TValueOut, TOther>(this, fieldOrProperty);

        public IMemberConstraintBuilder<T, TOther> And<TOther>(Func<T, TValueOut, TOther> fieldOrProperty, string fieldOrPropertyName) =>
            And(fieldOrProperty, Identifier.ParseOrNull(fieldOrPropertyName));

        public IMemberConstraintBuilder<T, TOther> And<TOther>(Func<T, TValueOut, TOther> fieldOrProperty, Identifier fieldOrPropertyName)
        {            
            if (fieldOrProperty == null)
            {
                throw new ArgumentNullException(nameof(fieldOrProperty));
            }
            var transformer = fieldOrPropertyName == null
                ? new MemberTransformer()
                : new PushIdentifierTransformer(fieldOrPropertyName);

            return Satisfies(instance => new DelegateFilter<T, TValueOut, TOther>(instance, fieldOrProperty), transformer);
        }
        
        public void And(Action<IMemberConstraintSet<TValueOut>> innerConstraintFactory) =>
            _memberConstraintSet.AddChildMemberConstraints(innerConstraintFactory, _memberConstraintFactory.CreateChildMember());

        #endregion

        #region [====== InstanceOf ======]

        /// <inheritdoc />      
        public IMemberConstraintBuilder<T, TValueOut> IsNotInstanceOf<TOther>(string errorMessage = null) =>
            this.IsNotInstanceOf(typeof(TOther), errorMessage);

        /// <inheritdoc />     
        public IMemberConstraintBuilder<T, TOther> IsInstanceOf<TOther>(string errorMessage = null) =>
            Satisfies(new IsInstanceOfFilter<TValueOut, TOther>().WithErrorMessage(errorMessage));

        /// <inheritdoc />
        public IMemberConstraintBuilder<T, TOther> As<TOther>() where TOther : class =>
            Satisfies(new AsFilter<TValueOut, TOther>());

        #endregion

        #region [====== HasItem ======]

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(int index, string errorMessage = null) =>
            HasItem<TItem, int>(index, errorMessage);

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(string index, string errorMessage = null) =>
            HasItem<TItem, string>(index, errorMessage);

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndex>(TIndex index, string errorMessage = null) =>
            HasItem<TItem, TIndex>(instance => index, errorMessage);

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndexA, TIndexB>(TIndexA indexA, TIndexB indexB, string errorMessage = null) =>
            HasItem<TItem, TIndexA, TIndexB>(instance => indexA, instance => indexB, errorMessage);

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndex>(Func<T, TIndex> indexFactory, string errorMessage = null) =>
            HasItem<TItem>(new IndexListFactory<T> { indexFactory }, errorMessage);

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndexA, TIndexB>(Func<T, TIndexA> indexA, Func<T, TIndexB> indexB, string errorMessage = null) =>
            HasItem<TItem>(new IndexListFactory<T> { indexA, indexB }, errorMessage);

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(IndexListFactory<T> indexList, string errorMessage = null)
        {
            if (indexList == null)
            {
                throw new ArgumentNullException(nameof(indexList));
            }
            var builder = Satisfies(instance =>
            {
                var materializedList = indexList.Materialize(instance);
                var constraint = new HasItemFilter<TValueOut>(materializedList).WithErrorMessage(errorMessage);
                var transformer = new PushIndexListTransformer(materializedList);

                return new Tuple<IFilter<TValueOut, object>, MemberTransformer>(constraint, transformer);
            });
            return builder.IsInstanceOf<TItem>();
        }               

        #endregion

        #region [====== Satisfies ======]

        public IMemberConstraintBuilder<T, TValueOut> Satisfies(Predicate<TValueOut> constraint, string errorMessage = null, object errorMessageArgument = null) =>
            Satisfies(new DelegateConstraint<TValueOut>(constraint, errorMessageArgument).WithErrorMessage(errorMessage));

        public IMemberConstraintBuilder<T, TValueOut> Satisfies(IConstraint<TValueOut> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }
            return Satisfies(message => constraint);
        }
        
        public IMemberConstraintBuilder<T, TValueOut> Satisfies(Func<T, IConstraint<TValueOut>> constraintFactory)
        {
            if (constraintFactory == null)
            {
                throw new ArgumentNullException(nameof(constraintFactory));
            }
            return Satisfies(message => constraintFactory.Invoke(message).MapInputToOutput());
        }
        
        public IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(IFilter<TValueOut, TOther> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }
            return Satisfies(message => constraint);
        }
        
        public IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(Func<T, IFilter<TValueOut, TOther>> constraintFactory)
        {
            if (constraintFactory == null)
            {
                throw new ArgumentNullException(nameof(constraintFactory));
            }
            return Satisfies(constraintFactory, new MemberTransformer());
        }

        private IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(Func<T, IFilter<TValueOut, TOther>> constraintFactory, MemberTransformer transformer) =>
            Satisfies(instance => new Tuple<IFilter<TValueOut, TOther>, MemberTransformer>(constraintFactory.Invoke(instance), transformer));

        private IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(Func<T, Tuple<IFilter<TValueOut, TOther>, MemberTransformer>> memberConstraintFactory)
        {            
            var newConstraintFactory = _memberConstraintFactory.And(memberConstraintFactory);
            var newMemberConstraint = new MemberConstraintBuilder<T, TValueIn, TOther>(_key, _memberConstraintSet, newConstraintFactory);

            _memberConstraintSet.Replace(this, newMemberConstraint);

            return newMemberConstraint;
        }

        #endregion
    }
}
