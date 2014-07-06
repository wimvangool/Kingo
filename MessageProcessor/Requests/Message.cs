using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a basic implementation of the <see cref="IMessage" /> interface, in which
    /// change tracking and validation are supported.
    /// </summary>
    public abstract class Message : PropertyChangedNotifier, IMessage
    {
        private readonly bool _isReadOnly;
        private MessageErrorInfo _errorInfo;                       
        private bool _hasChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="Message" /> class.
        /// </summary>
        protected Message()
        {
            _isReadOnly = false;
            _errorInfo = MessageErrorInfo.NotYetValidated;
            _hasChanges = false;
        }                

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="message">The message to copy.</param>
        /// <param name="makeReadOnly">Indicates whether the new instance should be marked readonly.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected Message(Message message, bool makeReadOnly)
        {            
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _isReadOnly = makeReadOnly;
            _errorInfo = message._errorInfo;
            _hasChanges = false;
        }        

        #region [====== Change Tracking ======]

        /// <inheritdoc />
        public event EventHandler HasChangesChanged;

        /// <inheritdoc />
        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {                                
                if (_isReadOnly && value)
                {
                    throw NewMessageIsReadOnlyException(this);
                }
                if (_hasChanges != value)
                {
                    _hasChanges = value;

                    OnHasChangesChanged();
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="HasChangesChanged" /> and <see cref="INotifyPropertyChanged.PropertyChanged" /> events.
        /// </summary>        
        protected virtual void OnHasChangesChanged()
        {
            HasChangesChanged.Raise(this);

            OnPropertyChanged(() => HasChanges);
        }

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged" /> event, signalling all properties have changed.
        /// </summary>
        /// <remarks>
        /// In addition to the base implementation, this method also marks the message as changed and triggers validation.
        /// </remarks>
        protected override void OnAllPropertiesChanged()
        {
            OnPropertyChanged(string.Empty, MessageChangedOption.MarkAsChangedAndValidate);
        }

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged" />-event, signaling the specified <paramref name="property"/> has changed.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="property">The property for which the event is raised.</param>        
        /// <param name="option">Indicates whether or not this message should be marked as changed and/or must revalidate itself.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="property"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="option"/> does not specify a valid value.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This message if readonly and the <paramref name="option"/> parameter indicates that this message
        /// must be marked as changed.
        /// </exception>
        protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> property, MessageChangedOption option)
        {
            OnPropertyChanged(GetPropertyNameOf(property), option);            
        }

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged" />-event, signaling the specified property has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        /// <param name="option">Indicates whether or not this message should be marked as changed and/or must revalidate itself.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="option"/> does not specify a valid value.
        /// </exception>
        /// <remarks>
        /// A <c>null </c> or <c>string.Empty</c> value for <paramref name="propertyName"/> indicates that
        /// all properties have changed.
        /// </remarks>
        protected void OnPropertyChanged(string propertyName, MessageChangedOption option)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            
            switch (option)
            {
                case MessageChangedOption.None:                
                    return;
                
                case MessageChangedOption.MarkAsChanged:                
                    HasChanges = true;
                    return;
                
                case MessageChangedOption.MarkAsChangedAndValidate:                
                    HasChanges = true;
                    Validate();
                    return;

                default:
                    throw NewInvalidOptionException(option);
            }                        
        }

        #endregion

        #region [====== Validation ======]

        string IDataErrorInfo.this[string columnName]
        {
            get { return IsValid ? null : ErrorInfo[columnName]; }
        }

        string IDataErrorInfo.Error
        {
            get { return IsValid ? null : ErrorInfo.Error; }
        }               

        /// <summary>
        /// Gets or sets the validation-errors.
        /// </summary>
        protected MessageErrorInfo ErrorInfo
        {
            get { return _errorInfo; }
            set
            {
                var oldValue = IsValid;

                _errorInfo = value;

                var newValue = IsValid;

                if (oldValue != newValue)
                {
                    OnIsValidChanged();
                }
            }
        }

        /// <inheritdoc />
        public bool IsValid
        {
            get { return ErrorInfo == null; }            
        }

        /// <inheritdoc />
        public event EventHandler IsValidChanged;

        /// <summary>
        /// Raises the <see cref="IsValidChanged" />- and <see cref="INotifyPropertyChanged.PropertyChanged" />-events.
        /// </summary>        
        protected virtual void OnIsValidChanged()
        {
            IsValidChanged.Raise(this);

            OnPropertyChanged(() => IsValid, MessageChangedOption.None);
        }

        /// <inheritdoc />
        public void Validate()
        {
            ErrorInfo = MessageErrorInfo.CreateErrorInfo(CreateValidationContext());                       
        }    
        
        /// <summary>
        /// Creates and returns a <see cref="ValidationContext" /> that is used during validation of this message.
        /// </summary>
        /// <returns>
        /// A default <see cref="ValidationContext" /> with no <see cref="IServiceProvider" /> or items.
        /// </returns>
        /// <remarks>
        /// A subclass can override this method to return a more specific <see cref="ValidationContext" /> is required.
        /// This may be necessary when a custom <see cref="IServiceProvider">service provider</see> is needed to
        /// perform validation on this message.
        /// </remarks>
        protected virtual ValidationContext CreateValidationContext()
        {
            return new ValidationContext(this, null, null);
        }

        #endregion

        #region [====== Exception Factory Methods ======]

        private static Exception NewInvalidOptionException(MessageChangedOption option)
        {
            var messageFormat = ExceptionMessages.Message_InvalidOption;
            var message = string.Format(messageFormat, typeof(MessageChangedOption), option);
            return new ArgumentOutOfRangeException("option", message);
        }

        internal static Exception NewMessageIsReadOnlyException(object readonlyMessage)
        {
            var messageFormat = ExceptionMessages.Message_IsReadOnly;
            var message = string.Format(messageFormat, readonlyMessage);
            return new InvalidOperationException(message);
        }

        #endregion
    }
}
