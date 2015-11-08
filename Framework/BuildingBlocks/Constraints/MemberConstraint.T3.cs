using System;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints
{    
    internal sealed class MemberConstraint<T, TValueIn, TValueOut> : IMemberConstraint<T, TValueOut>
    {
        private readonly MemberConstraintSet<T> _memberConstraintSet;        
        private readonly MemberConstraintFactory<T, TValueIn, TValueOut> _memberConstraintFactory;                

        internal MemberConstraint(MemberConstraintSet<T> memberConstraintSet, MemberConstraintFactory<T, TValueIn, TValueOut> memberConstraintFactory)
        {
            _memberConstraintSet = memberConstraintSet;            
            _memberConstraintFactory = memberConstraintFactory;
        }                    

        IMember IMemberConstraint<T>.Member
        {
            get { return _memberConstraintFactory.Member; }
        }        

        bool IErrorMessageWriter<T>.WriteErrorMessages(T message, IErrorMessageReader reader)
        {
            return _memberConstraintFactory.WriteErrorMessages(message, reader);
        }

        public IMemberConstraint<T, TMember> And<TMember>(Expression<Func<TValueOut, TMember>> fieldOrPropertyExpression)
        {
            if (fieldOrPropertyExpression == null)
            {
                throw new ArgumentNullException("fieldOrPropertyExpression");
            }
            return And(fieldOrPropertyExpression.Compile(), fieldOrPropertyExpression.ExtractMemberName());
        }

        public IMemberConstraint<T, TMember> And<TMember>(Func<TValueOut, TMember> fieldOrProperty, string fieldOrPropertyName)
        {
            return And(fieldOrProperty, Identifier.ParseOrNull(fieldOrPropertyName));
        }
        
        public IMemberConstraint<T, TMember> And<TMember>(Func<TValueOut, TMember> fieldOrProperty, Identifier fieldOrPropertyName)
        {
            return Satisfies(message => new DelegateFilter<TValueOut, TMember>(fieldOrProperty), new MemberSelectionTransformation(fieldOrPropertyName));
        }
        
        public void And(Action<IMemberConstraintSet<TValueOut>> innerConstraintFactory)
        {
            _memberConstraintSet.AddChildMemberConstraints(innerConstraintFactory, _memberConstraintFactory.CreateChildMember());            
        }       

        #region [====== InstanceOf ======]

        /// <inheritdoc />      
        public IMemberConstraint<T, TValueOut> IsNotInstanceOf<TOther>(string errorMessage = null)
        {
            return this.IsNotInstanceOf(typeof(TOther), errorMessage);            
        }        

        /// <inheritdoc />     
        public IMemberConstraint<T, TOther> IsInstanceOf<TOther>(string errorMessage = null)
        {
            return Satisfies(new IsInstanceOfFilter<TValueOut, TOther>().WithErrorMessage(errorMessage));            
        }

        /// <inheritdoc />
        public IMemberConstraint<T, TOther> As<TOther>() where TOther : class
        {
            return Satisfies(new AsFilter<TValueOut, TOther>());
        }

        #endregion

        #region [====== Satisfies ======]

        public IMemberConstraint<T, TValueOut> Satisfies(Func<TValueOut, bool> constraint, string errorMessage = null)
        {
            return Satisfies(new DelegateConstraint<TValueOut>(constraint).WithErrorMessage(errorMessage));
        }

        public IMemberConstraint<T, TValueOut> Satisfies(IConstraint<TValueOut> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return Satisfies(message => constraint);
        }
        
        public IMemberConstraint<T, TValueOut> Satisfies(Func<T, IConstraint<TValueOut>> constraintFactory)
        {
            if (constraintFactory == null)
            {
                throw new ArgumentNullException("constraintFactory");
            }
            return Satisfies(message => constraintFactory.Invoke(message).MapInputToOutput());
        }
        
        public IMemberConstraint<T, TOther> Satisfies<TOther>(IFilter<TValueOut, TOther> constraint, Func<string, string> nameSelector = null)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return Satisfies(message => constraint, nameSelector);
        }
        
        public IMemberConstraint<T, TOther> Satisfies<TOther>(Func<T, IFilter<TValueOut, TOther>> constraintFactory, Func<string, string> nameSelector = null)
        {
            return Satisfies(constraintFactory, new MemberNameTransformation(nameSelector));
        }

        private IMemberConstraint<T, TOther> Satisfies<TOther>(Func<T, IFilter<TValueOut, TOther>> constraintFactory, IMemberTransformation transformation)
        {
            if (constraintFactory == null)
            {
                throw new ArgumentNullException("constraintFactory");
            }
            var newConstraintFactory = _memberConstraintFactory.And(constraintFactory, transformation);
            var newMemberConstraint = new MemberConstraint<T, TValueIn, TOther>(_memberConstraintSet, newConstraintFactory);

            _memberConstraintSet.Replace(this, newMemberConstraint);

            return newMemberConstraint;
        }

        #endregion                    
    }
}
