using System;

namespace Kingo.BuildingBlocks.ComponentModel.Constraints
{    
    internal sealed class MemberConstraint<TValue, TResult> : IMemberConstraint<TResult>
    {
        private readonly MemberConstraintSet _memberConstraintSet;
        private readonly Member<TValue> _member;
        private readonly Func<string, string> _nameSelector;
        private readonly IConstraintWithErrorMessage<TValue, TResult> _constraint;                     

        internal MemberConstraint(MemberConstraintSet memberConstraintSet, Member<TValue> member, IConstraintWithErrorMessage<TValue, TResult> constraint)
            : this(memberConstraintSet, member, constraint, null) { }

        private MemberConstraint(MemberConstraintSet memberConstraintSet, Member<TValue> member, IConstraintWithErrorMessage<TValue, TResult> constraint, Func<string, string> nameSelector)
        {
            _memberConstraintSet = memberConstraintSet;
            _member = member;
            _nameSelector = nameSelector;
            _constraint = constraint;
        }                    

        IMember IMemberConstraint.Member
        {
            get { return _member; }
        }              

        public Member<TValue> Member
        {
            get { return _member; }
        }

        bool IErrorMessageProducer.AddErrorMessagesTo(IErrorMessageConsumer consumer, IFormatProvider formatProvider)
        {            
            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }
            TResult result;
            IConstraintWithErrorMessage failedConstraint;

            if (_constraint.IsSatisfiedBy(_member.Value, out result, out failedConstraint))
            {
                return false;                 
            }
            var errorMessage = failedConstraint
                .FormatErrorMessage(formatProvider)
                .Format(MemberConstraints.MemberId, _member, formatProvider);

            consumer.Add(_member.FullName, errorMessage.ToString());
            return true;         
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", _member.FullName, _constraint.ToString(_member.Name));
        }

        /// <inheritdoc />
        public void And(Action<IMemberConstraintSet, TResult> innerConstraintFactory)
        {
            if (innerConstraintFactory == null)
            {
                throw new ArgumentNullException("innerConstraintFactory");
            }
            _memberConstraintSet.PushParent(_member.Name);

            try
            {
                //innerConstraintFactory.Invoke(_memberConstraintSet, _member.Value);                
                throw new NotImplementedException();
            }
            finally
            {
                _memberConstraintSet.PopParent();
            }
        }       

        #region [====== InstanceOf ======]

        /// <inheritdoc />      
        public IMemberConstraint<TResult> IsNotInstanceOf<TOther>(string errorMessage = null)
        {
            return this.IsNotInstanceOf(typeof(TOther), errorMessage);            
        }        

        /// <inheritdoc />     
        public IMemberConstraint<TOther> IsInstanceOf<TOther>(string errorMessage = null)
        {
            return Satisfies(MemberConstraints.IsInstanceOfConstraint<TResult, TOther>(errorMessage));
        }        

        #endregion

        #region [====== Satisfies ======]

        /// <inheritdoc />
        public IMemberConstraint<TOther> Satisfies<TOther>(IConstraintWithErrorMessage<TResult, TOther> constraint, Func<string, string> nameSelector = null)
        {
            var newConstraint = _constraint.And(constraint);
            var newNember = _member.Rename(_nameSelector);
            var newMemberConstraint = new MemberConstraint<TValue, TOther>(_memberConstraintSet, newNember, newConstraint, nameSelector);

            _memberConstraintSet.Replace(this, newMemberConstraint);

            return newMemberConstraint;                        
        }

        #endregion                                          
    }
}
