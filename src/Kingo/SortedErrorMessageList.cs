using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo
{
    /// <summary>
    /// Represents a list of error messages that is sorted by its inheritance level.
    /// </summary>
    public sealed class SortedErrorMessageList : IEnumerable<KeyValuePair<ErrorInheritanceLevel, string>>
    {
        internal const string DebugStringFormat = @"{0} error(s) ({1} excluding inherited errors)";

        private readonly List<KeyValuePair<ErrorInheritanceLevel, string>> _errorMessages;

        internal SortedErrorMessageList()
        {
            _errorMessages = new List<KeyValuePair<ErrorInheritanceLevel, string>>();
        }

        /// <summary>
        /// Returns the number of error messages in this list.
        /// </summary>
        public int Count
        {
            get { return _errorMessages.Count; }
        }

        /// <summary>
        /// Returns the number of non-herited error messages in this list.
        /// </summary>
        /// <returns>The number of non-herited error messages in this list.</returns>
        public int CountNonInheritedErrors()
        {
            return _errorMessages.Count(error => error.Key == ErrorInheritanceLevel.NotInherited);
        }

        internal void Add(ErrorInheritanceLevel inheritanceLevel, string errorMessage)
        {            
            _errorMessages.Add(new KeyValuePair<ErrorInheritanceLevel, string>(inheritanceLevel, errorMessage));
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<ErrorInheritanceLevel, string>> GetEnumerator()
        {
            return _errorMessages.OrderBy(errorMessage => errorMessage.Key).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(DebugStringFormat, Count, CountNonInheritedErrors());
        }
    }
}
