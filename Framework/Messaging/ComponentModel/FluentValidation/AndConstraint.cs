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

        public override void AddErrorMessagesTo(IErrorMessageConsumer consumer)
        {
            if (consumer == null)
            {
                return;
            }
            var errorCounter = new ErrorMessageCounter(consumer);

            _left.AddErrorMessagesTo(errorCounter);

            if (errorCounter.ErrorCount == 0)
            {
                _right.AddErrorMessagesTo(consumer);
            }
        }
    }
}
