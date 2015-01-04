using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a tree of errors that have been detected on a specific <see cref="IMessage" />.
    /// </summary>
    [Serializable]
    public sealed class MessageErrorTree
    {
        private readonly Type _messageType;
        private readonly IDictionary<string, string> _errors;
        private readonly MessageErrorTree[] _childErrorTrees;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageErrorTree" /> class.
        /// </summary>
        /// <param name="messageType">Type of the message these errors were found on.</param>
        /// <param name="errors">Error-messages indexed by property- or fieldname.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageType"/> is <c>null</c>.
        /// </exception>
        public MessageErrorTree(Type messageType, IDictionary<string, string> errors)
            : this(messageType, errors, null) { }
 
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageErrorTree" /> class.
        /// </summary>
        /// <param name="messageType">Type of the message these errors were found on.</param>
        /// <param name="errors">Error-messages indexed by property- or fieldname.</param>
        /// <param name="childErrorTrees">Trees containing errors for any child-messages of the specified <paramref name="messageType"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageType"/> is <c>null</c>.
        /// </exception>
        public MessageErrorTree(Type messageType, IDictionary<string, string> errors, IEnumerable<MessageErrorTree> childErrorTrees)
        {
            if (messageType == null)
            {
                throw new ArgumentNullException("messageType");
            }
            _messageType = messageType;
            _errors = new ReadOnlyDictionary<string, string>(errors);
            _childErrorTrees = childErrorTrees == null ? new MessageErrorTree[0] : childErrorTrees.ToArray();
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
        public int TotalErrorCount
        {
            get { return _errors.Count + ChildErrors.Sum(errorTree => errorTree.TotalErrorCount); }
        }

        /// <summary>
        /// Returns the errors that were detected on the message. The key represent a property, the value contains the error message.
        /// </summary>
        public IDictionary<string, string> Errors
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
