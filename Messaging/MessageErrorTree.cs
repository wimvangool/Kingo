using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a tree of errors that have been detected on a specific <see cref="IMessage" />.
    /// </summary>
    public sealed class MessageErrorTree
    {
        private readonly Type _messageType;
        private readonly Dictionary<string, string> _errors;
        private readonly MessageErrorTree[] _childErrorTrees;
 
        internal MessageErrorTree(Type messageType, Dictionary<string, string> errors, IEnumerable<MessageErrorTree> childErrorTrees)
        {
            _messageType = messageType;
            _errors = errors;
            _childErrorTrees = childErrorTrees.ToArray();
        }

        /// <summary>
        /// Returns the type of the message these errors relate to.
        /// </summary>
        public Type MessageType
        {
            get { return _messageType; }
        }

        /// <summary>
        /// Returns the number of errors that were detected on the related message.
        /// </summary>
        public int ErrorCount
        {
            get { return _errors.Count; }
        }

        /// <summary>
        /// Returns the errors that were detected on the message. The key represent a property, the value contains the error message.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Errors
        {
            get { return _errors; }
        }

        /// <summary>
        /// Returns a collection of <see cref="MessageErrorTree">error trees</see> that contain the errors of child-messages, if present.
        /// </summary>
        public IEnumerable<MessageErrorTree> ChildErrors
        {
            get { return _childErrorTrees; }
        }
    }
}
