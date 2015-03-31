namespace System.ComponentModel.FluentValidation
{
    internal sealed class Constraint<TValue> : Constraint
    {
        private readonly Member<TValue> _member;
        private readonly Func<TValue, bool> _constraint;
        private readonly ErrorMessage _errorMessage;

        internal Constraint(Member<TValue> member, Func<TValue, bool> constraint, ErrorMessage errorMessage)
        {            
            _member = member;
            _constraint = constraint;
            _errorMessage = errorMessage;
        }        

        public override int AddErrorMessagesTo(IErrorMessageConsumer consumer)
        {            
            if (consumer == null || _constraint.Invoke(_member.Value))
            {
                return 0;
            }
            consumer.Add(_member.FullName, _errorMessage);
            return 1;
        }        
    }
}
