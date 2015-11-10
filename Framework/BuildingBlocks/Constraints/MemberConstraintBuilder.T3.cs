using System;
using System.Collections.Generic;
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
            return Satisfies(message => new DelegateFilter<TValueOut, TMember>(fieldOrProperty), new PushIdentifierTransformer(fieldOrPropertyName));
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

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(object index, string errorMessage = null)
        {
            return HasItem<TItem>(new [] { index }, errorMessage);
        }

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(object indexA, object indexB, string errorMessage = null)
        {
            return HasItem<TItem>(new[] { indexA, indexB }, errorMessage);
        }

        public IMemberConstraintBuilder<T, TItem> HasItem<TItem>(IEnumerable<object> indices, string errorMessage = null)
        {            
            var indexList = new IndexList(indices);
            var constraint = new HasItemFilter<TValueOut>(indexList).WithErrorMessage(errorMessage);           

            return Satisfies(instance => constraint, new PushIndexListTransformer(indexList)).IsInstanceOf<TItem>();
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
            return Satisfies(constraintFactory, new MemberTransformer());
        }

        private IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(Func<T, IFilter<TValueOut, TOther>> constraintFactory, MemberTransformer transformer)
        {
            if (constraintFactory == null)
            {
                throw new ArgumentNullException("constraintFactory");
            }
            var newConstraintFactory = _memberConstraintFactory.And(constraintFactory, transformer);
            var newMemberConstraint = new MemberConstraintBuilder<T, TValueIn, TOther>(_memberConstraintSet, newConstraintFactory);

            _memberConstraintSet.Replace(this, newMemberConstraint);

            return newMemberConstraint;
        }

        #endregion                                
    }
}
