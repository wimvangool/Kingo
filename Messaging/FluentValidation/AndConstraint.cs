namespace System.ComponentModel.FluentValidation
{
    internal sealed class AndConstraint : Constraint
    {        
        private readonly Constraint _left;
        private readonly Constraint _right;

        internal AndConstraint(Constraint left, Constraint right)
        {
            _left = left;
            _right = right;
        }        

        public override int Accept(IErrorMessageConsumer consumer)
        {
            int errorCount = _left.Accept(consumer);
            if (errorCount == 0)
            {
                errorCount = _right.Accept(consumer);
            }
            return errorCount;
        }
    }
}
