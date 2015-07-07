using System;

namespace Syztem.ComponentModel.FluentValidation
{
    internal sealed class Constraint<TValue> : Constraint
    {
        private readonly Member<TValue> _member;
        private readonly Func<TValue, bool> _constraint;
        private readonly FormattedString _errorMessage;

        internal Constraint(Member<TValue> member, Func<TValue, bool> constraint, FormattedString errorMessage)
        {            
            _member = member;
            _constraint = constraint;
            _errorMessage = errorMessage;
        }        

        public override void AddErrorMessagesTo(IErrorMessageConsumer consumer)
        {            
            if (consumer == null || _constraint.Invoke(_member.Value))
            {
                return;
            }
            consumer.Add(_member.FullName, _errorMessage);            
        }        
    }
}
