namespace System.ComponentModel.FluentValidation
{
    internal sealed class NullConstraint : Constraint
    {
        protected override Constraint And(Constraint constraint)
        {            
            return constraint;
        }

        public override int AddErrorMessagesTo(IErrorMessageConsumer consumer)
        {
            return 0;
        }
    }
}
