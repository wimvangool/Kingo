namespace Syztem.ComponentModel.FluentValidation
{
    internal sealed class ErrorMessageCounter : IErrorMessageConsumer
    {
        private readonly IErrorMessageConsumer _consumer;
        private int _errorCount;

        internal ErrorMessageCounter(IErrorMessageConsumer consumer)
        {
            _consumer = consumer;
        }

        internal int ErrorCount
        {
            get { return _errorCount; ;}
        }

        public void Add(string memberName, FormattedString errorMessage)
        {
            _errorCount++;
            _consumer.Add(memberName, errorMessage);
        }
    }
}
