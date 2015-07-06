namespace System.ComponentModel.FluentValidation
{
    internal abstract class Constraint : IErrorMessageProducer
    {
        #region [====== Or ======]

        internal Constraint Or<TValue>(Member<TValue> valueProvider, Action<IMemberConstraintSet, TValue>[] constraints, FormattedString errorMessage, IErrorMessageConsumer consumer)
        {
            return And(new OrConstraint<TValue>(valueProvider, constraints, errorMessage), consumer);
        }

        #endregion 

        #region [====== And ======]

        internal Constraint And<TValue>(Member<TValue> valueProvider, Func<TValue, bool> constraint, FormattedString errorMessage, IErrorMessageConsumer consumer)
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

        #endregion               

        public abstract void AddErrorMessagesTo(IErrorMessageConsumer consumer);        
    }
}
