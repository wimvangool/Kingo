using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace YellowFlare.MessageProcessing.Messages.Clients
{
    /// <summary>
    /// Represents a <see cref="IMessage" /> that is composed of other messages.
    /// </summary>
    public class CompositeMessage : PropertyChangedNotifier, IMessage
    {        
        private readonly List<IMessage> _messages;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeMessage" /> class.
        /// </summary>
        public CompositeMessage()
        {            
            _messages = new List<IMessage>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeMessage" /> class and adds the two
        /// specified messages to this instance.
        /// </summary>        
        public CompositeMessage(IMessage a, IMessage b) : this()
        {
            Add(a);
            Add(b);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeMessage" /> class and adds the three
        /// specified messages to this instance.
        /// </summary>        
        public CompositeMessage(IMessage a, IMessage b, IMessage c) : this()
        {
            Add(a);
            Add(b);
            Add(c);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeMessage" /> class and adds all
        /// specified messages to this instance.
        /// </summary>
        /// <param name="messages">The list of messages to add to this instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        public CompositeMessage(params IMessage[] messages)
            : this(messages as IEnumerable<IMessage>) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeMessage" /> class and adds all
        /// specified messages to this instance.
        /// </summary>
        /// <param name="messages">The list of messages to add to this instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        public CompositeMessage(IEnumerable<IMessage> messages) : this()
        {
            Add(messages);
        }

        #region [====== Change Tracking ======]

        public event EventHandler HasChangesChanged;

        private void HandleHasChangesChanged(object sender, EventArgs e)
        {
            HasChangesChanged.Raise(sender, e);

            OnPropertyChanged(() => HasChanges);
        }

        public bool HasChanges
        {
            get { return _messages.Any(message => message.HasChanges); }
            set
            {
                foreach (var message in _messages)
                {
                    message.HasChanges = value;
                }
            }
        }

        #endregion

        #region [====== Validation ======]

        /// <inheritdoc />
        string IDataErrorInfo.Error
        {
            get { return MessageErrorInfo.Concatenate(_messages.Select(message => message.Error)); }
        }

        /// <inheritdoc />
        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                IMessage message;
                string messageColumnName;

                if (TrySplitColumnName(columnName, out message, out messageColumnName))
                {
                    return message[messageColumnName];
                }
                return null;
            }
        }

        private bool TrySplitColumnName(string columnName, out IMessage message, out string messageColumnName)
        {
            if (columnName != null)
            {
                int indexOfFirstDot = columnName.IndexOf('.');
                if (indexOfFirstDot > 0 && TryFindMessage(columnName.Substring(0, indexOfFirstDot), out message))
                {
                    messageColumnName = columnName.Substring(indexOfFirstDot + 1);
                    return true;
                }
            }
            message = null;
            messageColumnName = null;
            return false;
        }

        private bool TryFindMessage(string typeName, out IMessage message)
        {
            return (message = _messages.FirstOrDefault(m => m.GetType().Name == typeName)) != null;
        }

        /// <inheritdoc />
        public event EventHandler IsValidChanged;

        private void HandleIsValidChanged(object sender, EventArgs e)
        {
            IsValidChanged.Raise(sender, e);

            OnPropertyChanged(() => IsValid);
        }

        public bool IsValid
        {
            get { return _messages.All(message => message.IsValid); }
        }

        public void Validate()
        {
            foreach (var message in _messages)
            {
                message.Validate();
            }
        }

        #endregion

        #region [====== Adding and Removing of Indicators ======]

        /// <summary>
        /// Adds all specified messages to this message.
        /// </summary>
        /// <param name="messages">The list of messages to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        public void Add(params IMessage[] messages)
        {
            Add(messages as IEnumerable<IMessage>);
        }

        /// <summary>
        /// Adds all specified messages to this message.
        /// </summary>
        /// <param name="messages">The list of messages to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        public void Add(IEnumerable<IMessage> messages)
        {
            if (messages == null)
            {
                throw new ArgumentNullException("messages");
            }
            foreach (var message in messages)
            {
                Add(message);
            }
        }

        /// <summary>
        /// If not <c>null</c>, adds the specified <paramref name="message"/> to this message.
        /// </summary>
        /// <param name="message">The message to add.</param>
        /// <remarks>
        /// <paramref name="message"/> is only added if it not <c>null</c> and not already present.
        /// </remarks>
        public void Add(IMessage message)
        {
            if (message == null || _messages.Contains(message))
            {
                return;
            }
            _messages.Add(message);

            message.HasChangesChanged += HandleHasChangesChanged;
            message.IsValidChanged += HandleIsValidChanged;
        }

        /// <summary>
        /// Removes the specified <paramref name="message"/> from this message.
        /// </summary>
        /// <param name="message">The message to remove.</param>
        public void Remove(IMessage message)
        {
            if (_messages.Contains(message))
            {
                _messages.Remove(message);

                message.IsValidChanged -= HandleIsValidChanged;
                message.HasChangesChanged -= HandleHasChangesChanged;
            }
        }

        /// <summary>
        /// Removes all messages from this message.
        /// </summary>
        public void Clear()
        {
            var messages = new List<IMessage>(_messages);

            foreach (var message in messages)
            {
                Remove(message);
            }
        }

        #endregion
    }
}
