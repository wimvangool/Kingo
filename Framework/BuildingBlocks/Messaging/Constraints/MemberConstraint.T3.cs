using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{    
    internal sealed class MemberConstraint<T, TValue, TResult> : IMemberConstraint<T, TResult>
    {
        private readonly MemberConstraintSet<T> _memberConstraintSet;
        private readonly Member<T, TValue> _member;
        private readonly Func<string, string> _nameSelector;
        private readonly IConstraintWithErrorMessage<TValue, TResult> _constraint;                     

        internal MemberConstraint(MemberConstraintSet<T> memberConstraintSet, Member<T, TValue> member, IConstraintWithErrorMessage<TValue, TResult> constraint)
            : this(memberConstraintSet, member, constraint, null) { }

        private MemberConstraint(MemberConstraintSet<T> memberConstraintSet, Member<T, TValue> member, IConstraintWithErrorMessage<TValue, TResult> constraint, Func<string, string> nameSelector)
        {
            _memberConstraintSet = memberConstraintSet;
            _member = member;
            _nameSelector = nameSelector;
            _constraint = constraint;
        }                    

        IMember IMemberConstraint<T>.Member
        {
            get { return _member; }
        }              

        public Member<T, TValue> Member
        {
            get { return _member; }
        }

        bool IErrorMessageProducer<T>.HasErrors(T item, IErrorMessageConsumer consumer, IFormatProvider formatProvider)
        {            
            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }            
            TResult result;
            IConstraintWithErrorMessage failedConstraint;
            var value = _member.GetValue(item);

            if (_constraint.IsSatisfiedBy(value, out result, out failedConstraint))
            {
                return false;                 
            }
            var member = new
            {
                _member.FullName,
                _member.Name,
                _member.Type,
                Value = value
            };
            var errorMessage = failedConstraint.FormatErrorMessage(formatProvider).Format(MemberConstraints.MemberId, member, formatProvider);

            consumer.Add(_member.FullName, errorMessage.ToString());
            return true;         
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", _member.FullName, _constraint.ToString(_member.Name));
        }

        /// <inheritdoc />
        public void And(Action<IMemberConstraintSet<TResult>> innerConstraintFactory)
        {
            _memberConstraintSet.AddChildMemberConstraints(innerConstraintFactory, _member.Transform(_constraint));
        }       

        #region [====== InstanceOf ======]

        /// <inheritdoc />      
        public IMemberConstraint<T, TResult> IsNotInstanceOf<TOther>(string errorMessage = null)
        {
            return this.IsNotInstanceOf(typeof(TOther), errorMessage);            
        }        

        /// <inheritdoc />     
        public IMemberConstraint<T, TOther> IsInstanceOf<TOther>(string errorMessage = null)
        {
            return Satisfies(MemberConstraints.IsInstanceOfConstraint<TResult, TOther>(errorMessage));
        }        

        #endregion

        #region [====== Satisfies ======]

        /// <inheritdoc />
        public IMemberConstraint<T, TOther> Satisfies<TOther>(IConstraintWithErrorMessage<TResult, TOther> constraint, Func<string, string> nameSelector = null)
        {
            var newConstraint = _constraint.And(constraint);
            var newNember = _member.Rename(_nameSelector);
            var newMemberConstraint = new MemberConstraint<T, TValue, TOther>(_memberConstraintSet, newNember, newConstraint, nameSelector);

            _memberConstraintSet.Replace(this, newMemberConstraint);

            return newMemberConstraint;                        
        }

        #endregion                                          
    }
}
