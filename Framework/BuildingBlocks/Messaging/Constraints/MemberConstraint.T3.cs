using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{    
    internal sealed class MemberConstraint<TMessage, TValue, TResult> : IMemberConstraint<TMessage, TResult>
    {
        private readonly MemberConstraintSet<TMessage> _memberConstraintSet;
        private readonly Member<TMessage, TValue> _member;
        private readonly Func<string, string> _nameSelector;
        private readonly IConstraintWithErrorMessage<TMessage, TValue, TResult> _constraint;        

        internal MemberConstraint(MemberConstraintSet<TMessage> memberConstraintSet, Member<TMessage, TValue> member, IConstraintWithErrorMessage<TMessage, TValue, TResult> constraint)
            : this(memberConstraintSet, member, constraint, null) { }

        private MemberConstraint(MemberConstraintSet<TMessage> memberConstraintSet, Member<TMessage, TValue> member, IConstraintWithErrorMessage<TMessage, TValue, TResult> constraint, Func<string, string> nameSelector)
        {
            _memberConstraintSet = memberConstraintSet;
            _member = member;
            _nameSelector = nameSelector;
            _constraint = constraint;
        }                    

        IMember IMemberConstraint<TMessage>.Member
        {
            get { return _member; }
        }        

        bool IErrorMessageProducer<TMessage>.HasErrors(TMessage message, IErrorMessageConsumer consumer, IFormatProvider formatProvider)
        {            
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException("message");
            }
            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }            
            TResult result;
            IConstraintWithErrorMessage<TMessage> failedConstraint;
            var member = _member.ToMember(message);

            if (_constraint.IsSatisfiedBy(member.Value, message, out result, out failedConstraint))
            {
                return false;                 
            }                        
            var errorMessage = failedConstraint.ErrorMessage                
                .Format(MemberConstraints.MemberId, member, formatProvider)                
                .Format(MemberConstraints.ConstraintId, failedConstraint.ErrorMessageArguments(message), formatProvider)
                .ToString();

            consumer.Add(_member.FullName, errorMessage);
            return true;         
        }                                

        /// <inheritdoc />
        public void And(Action<IMemberConstraintSet<TResult>> innerConstraintFactory)
        {
            _memberConstraintSet.AddChildMemberConstraints(innerConstraintFactory, _member.Transform(_constraint));
        }       

        #region [====== InstanceOf ======]

        /// <inheritdoc />      
        public IMemberConstraint<TMessage, TResult> IsNotInstanceOf<TOther>(string errorMessage = null)
        {
            return this.IsNotInstanceOf(typeof(TOther), errorMessage);            
        }        

        /// <inheritdoc />     
        public IMemberConstraint<TMessage, TOther> IsInstanceOf<TOther>(string errorMessage = null)
        {
            return Satisfies(MemberConstraints.IsInstanceOfConstraint<TMessage, TResult, TOther>(errorMessage));
        }        

        #endregion

        #region [====== Satisfies ======]

        /// <inheritdoc />
        public IMemberConstraint<TMessage, TOther> Satisfies<TOther>(IConstraintWithErrorMessage<TMessage, TResult, TOther> constraint, Func<string, string> nameSelector = null)
        {
            var newConstraint = _constraint.And(constraint);
            var newNember = _member.Rename(_nameSelector);
            var newMemberConstraint = new MemberConstraint<TMessage, TValue, TOther>(_memberConstraintSet, newNember, newConstraint, nameSelector);

            _memberConstraintSet.Replace(this, newMemberConstraint);

            return newMemberConstraint;                        
        }

        #endregion                                          
    }
}
