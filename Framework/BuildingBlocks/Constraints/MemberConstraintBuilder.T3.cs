using System;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints
{    
    internal sealed class MemberConstraintBuilder<T, TValueIn, TValueOut> : IMemberConstraintBuilder<T, TValueOut>
    {
        private readonly MemberConstraintSet<T> _memberConstraintSet;        
        private readonly MemberConstraintFactory<T, TValueIn, TValueOut> _memberConstraintFactory;                

        internal MemberConstraintBuilder(MemberConstraintSet<T> memberConstraintSet, MemberConstraintFactory<T, TValueIn, TValueOut> memberConstraintFactory)
        {
            _memberConstraintSet = memberConstraintSet;            
            _memberConstraintFactory = memberConstraintFactory;
        }

        #region [====== IMemberConstraint<T> ======]

        IMember IMemberConstraintBuilder<T>.Member
        {
            get { return _memberConstraintFactory.Member; }
        }        

        bool IErrorMessageWriter<T>.WriteErrorMessages(T message, IErrorMessageReader reader)
        {
            return _memberConstraintFactory.WriteErrorMessages(message, reader);
        }

        #endregion

        #region [====== And ======]

        public IMemberConstraintBuilder<T, TMember> And<TMember>(Expression<Func<TValueOut, TMember>> fieldOrProperty)
        {
            // TODO: Replace with recursive member expression technique.
            if (fieldOrProperty == null)
            {
                throw new ArgumentNullException("fieldOrProperty");
            }
            return And(fieldOrProperty.Compile(), fieldOrProperty.ExtractMemberName());
        }

        public IMemberConstraintBuilder<T, TMember> And<TMember>(Func<TValueOut, TMember> fieldOrProperty, string fieldOrPropertyName)
        {
            return And(fieldOrProperty, Identifier.ParseOrNull(fieldOrPropertyName));
        }
        
        public IMemberConstraintBuilder<T, TMember> And<TMember>(Func<TValueOut, TMember> fieldOrProperty, Identifier fieldOrPropertyName)
        {            
            if (fieldOrProperty == null)
            {
                throw new ArgumentNullException("fieldOrProperty");
            }            
            if (fieldOrPropertyName == null)
            {
                throw new ArgumentNullException("fieldOrPropertyName");
            }
            return Satisfies(instance => new DelegateFilter<TValueOut, TMember>(fieldOrProperty), new PushIdentifierTransformer(fieldOrPropertyName));
        }
        
        public void And(Action<IMemberConstraintSet<TValueOut>> innerConstraintFactory)
        {
            _memberConstraintSet.AddChildMemberConstraints(innerConstraintFactory, _memberConstraintFactory.CreateChildMember());            
        }

        #endregion

        #region [====== InstanceOf ======]

        /// <inheritdoc />      
        public IMemberConstraintBuilder<T, TValueOut> IsNotInstanceOf<TOther>(string errorMessage = null)
        {
            return this.IsNotInstanceOf(typeof(TOther), errorMessage);            
        }        

        /// <inheritdoc />     
        public IMemberConstraintBuilder<T, TOther> IsInstanceOf<TOther>(string errorMessage = null)
        {
            return Satisfies(new IsInstanceOfFilter<TValueOut, TOther>().WithErrorMessage(errorMessage));            
        }

        /// <inheritdoc />
        public IMemberConstraintBuilder<T, TOther> As<TOther>() where TOther : class
        {
            return Satisfies(new AsFilter<TValueOut, TOther>());
        }

        #endregion

        #region [====== HasItem ======]

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(int index, string errorMessage = null)
        {
            return HasItem<TItem, int>(index, errorMessage);
        }

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(string index, string errorMessage = null)
        {
            return HasItem<TItem, string>(index, errorMessage);
        }

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndex>(TIndex index, string errorMessage = null)
        {
            return HasItem<TItem, TIndex>(instance => index, errorMessage);
        }

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndexA, TIndexB>(TIndexA indexA, TIndexB indexB, string errorMessage = null)
        {
            return HasItem<TItem, TIndexA, TIndexB>(instance => indexA, instance => indexB, errorMessage);
        }

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndex>(Func<T, TIndex> indexFactory, string errorMessage = null)
        {
            return HasItem<TItem>(new IndexListFactory<T> { indexFactory }, errorMessage);
        }

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndexA, TIndexB>(Func<T, TIndexA> indexA, Func<T, TIndexB> indexB, string errorMessage = null)
        {
            return HasItem<TItem>(new IndexListFactory<T> { indexA, indexB }, errorMessage);
        }

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(IndexListFactory<T> indexList, string errorMessage = null)
        {
            if (indexList == null)
            {
                throw new ArgumentNullException("indexList");
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

        public IMemberConstraintBuilder<T, TValueOut> Satisfies(Func<TValueOut, bool> constraint, string errorMessage = null)
        {
            return Satisfies(new DelegateConstraint<TValueOut>(constraint).WithErrorMessage(errorMessage));
        }

        public IMemberConstraintBuilder<T, TValueOut> Satisfies(IConstraint<TValueOut> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return Satisfies(message => constraint);
        }
        
        public IMemberConstraintBuilder<T, TValueOut> Satisfies(Func<T, IConstraint<TValueOut>> constraintFactory)
        {
            if (constraintFactory == null)
            {
                throw new ArgumentNullException("constraintFactory");
            }
            return Satisfies(message => constraintFactory.Invoke(message).MapInputToOutput());
        }
        
        public IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(IFilter<TValueOut, TOther> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return Satisfies(message => constraint);
        }
        
        public IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(Func<T, IFilter<TValueOut, TOther>> constraintFactory)
        {
            if (constraintFactory == null)
            {
                throw new ArgumentNullException("constraintFactory");
            }
            return Satisfies(constraintFactory, new MemberTransformer());
        }

        private IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(Func<T, IFilter<TValueOut, TOther>> constraintFactory, MemberTransformer transformer)
        {
            return Satisfies(instance => new Tuple<IFilter<TValueOut, TOther>, MemberTransformer>(constraintFactory.Invoke(instance), transformer));
        }

        private IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(Func<T, Tuple<IFilter<TValueOut, TOther>, MemberTransformer>> memberConstraintFactory)
        {            
            var newConstraintFactory = _memberConstraintFactory.And(memberConstraintFactory);
            var newMemberConstraint = new MemberConstraintBuilder<T, TValueIn, TOther>(_memberConstraintSet, newConstraintFactory);

            _memberConstraintSet.Replace(this, newMemberConstraint);

            return newMemberConstraint;
        }

        #endregion
    }
}
