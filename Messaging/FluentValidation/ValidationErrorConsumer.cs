namespace System.ComponentModel.FluentValidation
{
    internal sealed class ValidationErrorConsumer : IErrorMessageConsumer
    {
        private readonly ValidationErrorTreeBuilder _builder;
        private readonly string _memberName;

        internal ValidationErrorConsumer(ValidationErrorTreeBuilder builder, string memberName)
        {
            _builder = builder;
            _memberName = memberName;
        }

        public void Add(ErrorMessage errorMessage)
        {
            _builder.Add(_memberName, errorMessage);
        }
    }
}
