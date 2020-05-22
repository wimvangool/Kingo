using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.DataContracts
{
    internal sealed class MemberErrorCollection : IMemberErrorCollection
    {        
        private readonly string _memberName;
        private readonly List<string> _validationErrors;

        public MemberErrorCollection(string memberName)
        {            
            _memberName = memberName;
            _validationErrors = new List<string>();
        }

        #region [====== IReadOnlyCollection<string> ======]

        public int Count =>
            _validationErrors.Count;

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public IEnumerator<string> GetEnumerator() =>
            _validationErrors.GetEnumerator();

        public override string ToString() =>
            $"{_memberName} --> {string.Join("; ", _validationErrors)}";

        #endregion

        #region [====== IMemberErrorCollection ======]

        public string MemberName =>
            _memberName;

        public void Add(string errorMessage) =>
            _validationErrors.Add(errorMessage);

        public void HasError(Func<string, bool> errorMessagePredicate, string message = null, params object[] args)
        {
            if (errorMessagePredicate == null)
            {
                throw new ArgumentNullException(nameof(errorMessagePredicate));
            }
            if (_validationErrors.Any(errorMessagePredicate))
            {
                return;
            }
            if (message == null)
            {
                throw NewNoSuchErrorMessageException(ExceptionMessages.Request_NoSuchErrorMessage, MemberName);
            }
            throw NewNoSuchErrorMessageException(message, args);
        }

        private static Exception NewNoSuchErrorMessageException(string messageFormat, params object[] args) =>
            new TestFailedException(string.Format(messageFormat, args));

        #endregion
    }
}
