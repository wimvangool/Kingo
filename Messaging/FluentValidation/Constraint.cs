namespace System.ComponentModel.FluentValidation
{
    internal abstract class Constraint : IErrorMessageProducer
    {
        internal Constraint And<TValue>(Member<TValue> valueProvider, Func<TValue, bool> constraint, ErrorMessage errorMessage, IErrorMessageConsumer consumer)
        {
            return And(new Constraint<TValue>(valueProvider, constraint, errorMessage), consumer);
        }

        private Constraint And(Constraint constraint, IErrorMessageConsumer consumer)
        {            
            constraint.AddErrorMessagesTo(consumer);

            return And(constraint);
        }

        protected virtual Constraint And(Constraint constraint)
        {
            return new AndConstraint(this, constraint);
        }

        void IErrorMessageProducer.AddErrorMessagesTo(IErrorMessageConsumer consumer)
        {
            AddErrorMessagesTo(consumer);
        }

        public abstract int AddErrorMessagesTo(IErrorMessageConsumer consumer);        
    }
}
