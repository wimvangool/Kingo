using System.Collections.Generic;

namespace Syztem.ComponentModel.FluentValidation
{
    internal sealed class ErrorMessageMerger : IErrorMessageConsumer
    {
        private readonly List<FormattedString> _errorMessages;

        internal ErrorMessageMerger()
        {
            _errorMessages = new List<FormattedString>();
        }

        public void Add(string memberName, FormattedString errorMessage)
        {
            _errorMessages.Add(errorMessage);
        }

        internal string MergeErrorMessages(FormattedString errorMessage)
        {
            // TODO: Do something with child errors...??
            return errorMessage.ToString();
        }
    }
}
