namespace System.ComponentModel.FluentValidation
{
    internal sealed class NullConstraint : Constraint
    {
        internal override Constraint And(Constraint constraint)
        {            
            return constraint;
        }

        public override int Accept(IErrorMessageConsumer consumer)
        {
            return 0;
        }
    }
}
