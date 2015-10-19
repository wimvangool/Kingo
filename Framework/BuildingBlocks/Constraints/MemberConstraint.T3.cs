using System;

namespace Kingo.BuildingBlocks.Constraints
{    
    internal sealed class MemberConstraint<TMessage, TValueIn, TValueOut> : IMemberConstraint<TMessage, TValueOut>
    {
        private readonly MemberConstraintSet<TMessage> _memberConstraintSet;
        private readonly Member<TMessage, TValueIn> _member;
        private readonly Func<string, string> _nameSelector;
        private readonly ConstraintFactory<TMessage, TValueIn, TValueOut> _constraintFactory;        

        internal MemberConstraint(MemberConstraintSet<TMessage> memberConstraintSet, Member<TMessage, TValueIn> member, ConstraintFactory<TMessage, TValueIn, TValueOut> constraint)
            : this(memberConstraintSet, member, constraint, null) { }

        private MemberConstraint(MemberConstraintSet<TMessage> memberConstraintSet, Member<TMessage, TValueIn> member, ConstraintFactory<TMessage, TValueIn, TValueOut> constraint, Func<string, string> nameSelector)
        {
            _memberConstraintSet = memberConstraintSet;
            _member = member;
            _nameSelector = nameSelector;
            _constraintFactory = constraint;
        }                    

        IMember IMemberConstraint<TMessage>.Member
        {
            get { return _member; }
        }        

        bool IErrorMessageWriter<TMessage>.WriteErrorMessages(TMessage message, IErrorMessageReader reader)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            var constraint = _constraintFactory.CreateConstraint(message);
            IErrorMessage errorMessage;

            if (constraint.IsNotSatisfiedBy(_member.GetValue(message), out errorMessage))
            {
                reader.Add(_member.FullName, errorMessage);
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public void And(Action<IMemberConstraintSet<TValueOut>> innerConstraintFactory)
        {
            _memberConstraintSet.AddChildMemberConstraints(innerConstraintFactory, _member.Transform(_constraintFactory));
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
            //return Satisfies(MemberConstraints.IsInstanceOfConstraint<TMessage, TValueOut, TOther>(errorMessage));
            throw new NotImplementedException();
        }        

        #endregion

        #region [====== Satisfies ======]

        public IMemberConstraint<TMessage, TValueOut> Satisfies(Func<TValueOut, bool> constraint, string errorMessage = null)
        {
            throw new NotImplementedException();
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
            var newConstraint = _constraintFactory.And(constraintFactory);
            var newNember = _member.Rename(_nameSelector);
            var newMemberConstraint = new MemberConstraint<TMessage, TValueIn, TOther>(_memberConstraintSet, newNember, newConstraint, nameSelector);

            _memberConstraintSet.Replace(this, newMemberConstraint);

            return newMemberConstraint;                        
        }

        #endregion                    
    }
}
