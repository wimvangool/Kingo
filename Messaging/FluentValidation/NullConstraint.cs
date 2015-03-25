namespace System.ComponentModel.FluentValidation
{
    internal sealed class NullConstraint : Constraint
    {
        internal override Constraint And(Constraint constraint, IErrorMessageConsumer consumer)
        {            
            return constraint;
        }

        public override int AddErrorMessagesTo(IErrorMessageConsumer consumer)
        {
            return 0;
        }
    }
}
