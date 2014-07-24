using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a message-stub for scenario's where a <see cref="IMessage" /> or <see cref="IMessage{T}" />
    /// instance is required but none is available.
    /// </summary>
    public sealed class NullMessage : PropertyChangedNotifier, IMessage<NullMessage>
    {
        private readonly bool _isReadOnly;
        private bool _hasChanges;
        private bool _isValid;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullMessage" /> class.
        /// </summary>
        /// <param name="isValid">Specifies whether or not this message must start in the valid or invalid state.</param>
        public NullMessage(bool isValid)
        {
            _isValid = isValid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullMessage" /> class.
        /// </summary>
        /// <param name="isValid">Specifies whether or not this message must start in the valid or invalid state.</param>
        /// <param name="makeReadOnly">
        /// Specified whether or not this message must be marked read-only.
        /// </param>
        public NullMessage(bool isValid, bool makeReadOnly)
        {
            _isValid = isValid;
            _isReadOnly = makeReadOnly;
        }

        private NullMessage(NullMessage message, bool makeReadOnly)
        {
            _isReadOnly = makeReadOnly;
            _isValid = message._isValid;
        }

        #region [====== Change Tracking ======]

        /// <inheritdoc />
        public event EventHandler HasChangesChanged;

        private void OnHasChangesChanged()
        {
            HasChangesChanged.Raise(this);

            OnPropertyChanged(() => HasChanges);
        }

        /// <inheritdoc />
        public bool HasChanges
        {
            get { return _hasChanges;}
            set
            {
                if (_isReadOnly && value)
                {
                    throw Message.NewMessageIsReadOnlyException(this);
                }
                if (_hasChanges != value)
                {
                    _hasChanges = value;

                    OnHasChangesChanged();
                }
            }
        }

        #endregion

        #region [====== Validation ======]

        /// <inheritdoc />
        public event EventHandler IsValidChanged;

        /// <inheritdoc />
        public bool IsValid
        {
            get { return _isValid; }
            private set
            {
                if (_isValid != value)
                {
                    _isValid = value;

                    OnIsValidChanged();
                }
            }
        }

        private void OnIsValidChanged()
        {
            IsValidChanged.Raise(this);
        }

        /// <summary>
        /// Marks this message as valid.
        /// </summary>
        public void Validate()
        {
            IsValid = true;
        }        

        /// <summary>
        /// Returns <c>null</c>.
        /// </summary>
        public string Error
        {
            get { return null; }
        }

        /// <summary>
        /// Returns <c>null</c> for any <paramref name="columnName"/>.
        /// </summary>
        /// <param name="columnName">A column-name (ignored).</param>
        /// <returns><c>null</c></returns>
        public string this[string columnName]
        {
            get { return null; }
        }

        #endregion

        /// <inheritdoc />
        public NullMessage Copy(bool makeReadOnly)
        {
            return new NullMessage(this, makeReadOnly);
        }
    }
}
