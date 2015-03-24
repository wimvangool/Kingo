namespace System.ComponentModel.FluentValidation
{
    internal sealed class Constraint<TValue> : Constraint
    {
        private readonly Member<TValue> _valueProvider;
        private readonly Func<TValue, bool> _constraint;
        private readonly ErrorMessage _errorMessage;

        internal Constraint(Member<TValue> valueProvider, Func<TValue, bool> constraint, ErrorMessage errorMessage)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            if (errorMessage == null)
            {
                throw new ArgumentNullException("errorMessage");
            }
            _valueProvider = valueProvider;
            _constraint = constraint;
            _errorMessage = errorMessage;
        }        

        public override int Accept(IErrorMessageConsumer consumer)
        {            
            if (consumer == null || _constraint.Invoke(_valueProvider.Value))
            {
                return 0;
            }
            return _errorMessage.Accept(consumer);
        }        
    }
}
