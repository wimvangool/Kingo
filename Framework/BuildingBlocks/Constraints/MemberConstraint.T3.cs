using System;

namespace Kingo.BuildingBlocks.Constraints
{    
    internal sealed class MemberConstraint<TMessage, TValueIn, TValueOut> : IMemberConstraint<TMessage, TValueOut>
    {
        private readonly MemberConstraintSet<TMessage> _memberConstraintSet;        
        private readonly MemberConstraintFactory<TMessage, TValueIn, TValueOut> _memberConstraintFactory;                

        internal MemberConstraint(MemberConstraintSet<TMessage> memberConstraintSet, MemberConstraintFactory<TMessage, TValueIn, TValueOut> memberConstraintFactory)
        {
            _memberConstraintSet = memberConstraintSet;            
            _memberConstraintFactory = memberConstraintFactory;
        }                    

        IMember IMemberConstraint<TMessage>.Member
        {
            get { return _memberConstraintFactory.Member; }
        }        

        bool IErrorMessageWriter<TMessage>.WriteErrorMessages(TMessage message, IErrorMessageReader reader)
        {
            return _memberConstraintFactory.WriteErrorMessages(message, reader);
        }

        /// <inheritdoc />
        public void And(Action<IMemberConstraintSet<TValueOut>> innerConstraintFactory)
        {
            _memberConstraintSet.AddChildMemberConstraints(innerConstraintFactory, _memberConstraintFactory.CreateChildMember());            
        }       

        #region [====== InstanceOf ======]

        /// <inheritdoc />      
        public IMemberConstraint<TMessage, TValueOut> IsNotInstanceOf<TOther>(string errorMessage = null)
        {
            return this.IsNotInstanceOf(typeof(TOther), errorMessage);            
        }        

        /// <inheritdoc />     
        public IMemberConstraint<TMessage, TOther> IsInstanceOf<TOther>(string errorMessage = null)
        {
            return Satisfies(new IsInstanceOfConstraint<TValueOut, TOther>().WithErrorMessage(errorMessage));            
        }        

        #endregion

        #region [====== Satisfies ======]

        public IMemberConstraint<TMessage, TValueOut> Satisfies(Func<TValueOut, bool> constraint, string errorMessage = null)
        {
            return Satisfies(new DelegateConstraint<TValueOut>(constraint).WithErrorMessage(errorMessage));
        }

        public IMemberConstraint<TMessage, TValueOut> Satisfies(IConstraint<TValueOut> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return Satisfies(message => constraint);
        }
        
        public IMemberConstraint<TMessage, TValueOut> Satisfies(Func<TMessage, IConstraint<TValueOut>> constraintFactory)
        {
            if (constraintFactory == null)
            {
                throw new ArgumentNullException("constraintFactory");
            }
            return Satisfies(message => constraintFactory.Invoke(message).MapInputToOutput());
        }
        
        public IMemberConstraint<TMessage, TOther> Satisfies<TOther>(IConstraint<TValueOut, TOther> constraint, Func<string, string> nameSelector = null)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return Satisfies(message => constraint, nameSelector);
        }
        
        public IMemberConstraint<TMessage, TOther> Satisfies<TOther>(Func<TMessage, IConstraint<TValueOut, TOther>> constraintFactory, Func<string, string> nameSelector = null)
        {
            if (constraintFactory == null)
            {
                throw new ArgumentNullException("constraintFactory");
            }
            var newConstraintFactory = _memberConstraintFactory.And(constraintFactory, nameSelector);            
            var newMemberConstraint = new MemberConstraint<TMessage, TValueIn, TOther>(_memberConstraintSet, newConstraintFactory);

            _memberConstraintSet.Replace(this, newMemberConstraint);

            return newMemberConstraint;                        
        }

        #endregion                    
    }
}
