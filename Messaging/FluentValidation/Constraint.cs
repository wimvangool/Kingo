namespace System.ComponentModel.FluentValidation
{
    internal abstract class Constraint : IErrorMessageProducer
    {
        internal Constraint And<TValue>(Member<TValue> valueProvider, Func<TValue, bool> constraint, ErrorMessage errorMessage)
        {
            return And(new Constraint<TValue>(valueProvider, constraint, errorMessage));
        }

        internal virtual Constraint And(Constraint constraint)
        {            
            return new AndConstraint(this, constraint);
        }

        public abstract int Accept(IErrorMessageConsumer consumer);        
    }
}
