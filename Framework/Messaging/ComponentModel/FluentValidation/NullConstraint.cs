namespace Syztem.ComponentModel.FluentValidation
{
    internal sealed class NullConstraint : Constraint
    {
        protected override Constraint And(Constraint constraint)
        {            
            return constraint;
        }

        public override void AddErrorMessagesTo(IErrorMessageConsumer consumer) { }
    }
}
