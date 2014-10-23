using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Messaging.Resources;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// Represents a basic implementation of the <see cref="IRequestMessage" /> interface, in which
    /// change tracking and validation are supported.
    /// </summary>
    public abstract class RequestMessage : PropertyChangedBase, IRequestMessage, IServiceProvider
    {
        private readonly Dictionary<Type, object> _services;
        private readonly Dictionary<string, IRequestMessage> _requestMessageProperties;
        private readonly bool _isReadOnly;
        private RequestMessageErrorInfo _errorInfo;                       
        private bool _hasChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage" /> class.
        /// </summary>
        protected RequestMessage()
        {
            _services = new Dictionary<Type, object>();
            _requestMessageProperties = new Dictionary<string, IRequestMessage>();
            _isReadOnly = false;
            _errorInfo = RequestMessageErrorInfo.NotYetValidated;
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
        protected RequestMessage(RequestMessage message, bool makeReadOnly)
        {            
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _services = new Dictionary<Type, object>(message._services);
            _requestMessageProperties = Copy(message._requestMessageProperties, makeReadOnly);
            _isReadOnly = makeReadOnly;
            _errorInfo = message._errorInfo == null ? null : new RequestMessageErrorInfo(message._errorInfo);
            _hasChanges = false;
        }

        #region [====== Message Copying ======]

        IMessage IMessage.Copy()
        {
            return Copy(true);
        }

        IRequestMessage IRequestMessage.Copy(bool makeReadOnly)
        {
            return Copy(makeReadOnly);
        }

        /// <summary>
        /// Creates and returns a copy of this message.
        /// </summary>
        /// <param name="makeReadOnly">Indicates whether or not the copy should be readonly.</param>
        /// <returns>
        /// A copy of this message, including the validation-state. If <paramref name="makeReadOnly"/> is <c>true</c>,
        /// all data properties of the copy will be readonly. In addition, the returned copy will be marked unchanged,
        /// even if this message is marked as changed. If the copy is readonly, the HasChanges-flag cannot be
        /// set to <c>true</c>.
        /// </returns>
        public abstract RequestMessage Copy(bool makeReadOnly);

        private static Dictionary<string, IRequestMessage> Copy(Dictionary<string, IRequestMessage> properties, bool makeReadOnly)
        {
            var propertiesCopy = new Dictionary<string, IRequestMessage>(properties.Count);

            foreach (var property in properties)
            {
                propertiesCopy.Add(property.Key, Copy(property.Value, makeReadOnly));
            }
            return propertiesCopy;
        }

        /// <summary>
        /// Creates and returns a copy of the specified <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">The message to copy.</param>
        /// <param name="makeReadOnly">Indicates whether or not the copy should be readonly.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="message"/> is <c>null</c>; otherwise, a copy of the message.
        /// </returns>
        protected static TMessage Copy<TMessage>(TMessage message, bool makeReadOnly) where TMessage : class, IRequestMessage
        {
            return message == null ? null : (TMessage) message.Copy(makeReadOnly);
        }

        /// <summary>
        /// Creates and returns a deep copy of the specified collection of messages.
        /// </summary>
        /// <typeparam name="TMessage">Type of the messages in the collection.</typeparam>
        /// <param name="messages">The collection to copy.</param>
        /// <param name="makeReadOnly">Indicates whether or not the copy should be readonly.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="messages"/> is <c>null</c>; otherwise, a new collection containing
        /// copies of all elements of <paramref name="messages"/>.
        /// </returns>
        protected static IList<TMessage> Copy<TMessage>(IEnumerable<TMessage> messages, bool makeReadOnly) where TMessage : IRequestMessage
        {
            if (messages == null)
            {
                return null;
            }
            return messages.Select(message => (TMessage) message.Copy(makeReadOnly)).ToArray();
        }

        #endregion

        #region [====== Service Provider ======]

        /// <summary>
        /// Registers the specified service with the <see cref="IServiceProvider" /> that is used for validation.
        /// </summary>
        /// <typeparam name="TService">Type of the service to register.</typeparam>
        /// <param name="service">The service to register.</param>
        /// <exception cref="ArgumentException">
        /// A service of type <typeparamref name="TService"/> has already been registered for this message.
        /// </exception>
        protected void RegisterService<TService>(TService service) where TService : class
        {
            TryRegisterService(service);
        }

        /// <summary>
        /// Registers the specified service with the <see cref="IServiceProvider" /> that is used for validation
        /// and wires up the service's <see cref="IAsyncValidationService.ValidationRequired" /> event to
        /// (re)validate this message when required.
        /// </summary>
        /// <typeparam name="TService">Type of the service to register.</typeparam>
        /// <param name="service">The service to register.</param>
        /// <exception cref="ArgumentException">
        /// A service of type <typeparamref name="TService"/> has already been registered for this message.
        /// </exception>
        protected void RegisterAsyncService<TService>(TService service) where TService : class, IAsyncValidationService
        {
            if (TryRegisterService(service))
            {
                service.ValidationRequired += (s, e) => Validate();
            }
        }

        private bool TryRegisterService<TService>(TService service) where TService : class
        {
            if (service == null)
            {
                return false;
            }
            try
            {
                _services.Add(typeof(TService), service);
            }
            catch (ArgumentException)
            {
                throw NewServiceAlreadyRegisteredException("service", typeof(TService));
            }
            return true;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            object service;

            if (_services.TryGetValue(serviceType, out service))
            {
                return service;
            }
            return null;
        }

        #endregion

        #region [====== Change Tracking ======]

        /// <inheritdoc />
        public event EventHandler HasChangesChanged;

        /// <inheritdoc />
        public bool HasChanges
        {
            get { return _hasChanges || _requestMessageProperties.Values.Any(message => message.HasChanges); }
            private set
            {                
                if (_hasChanges != value)
                {
                    _hasChanges = value;

                    OnHasChangesChanged();
                }
            }
        }

        private void MarkAsChanged()
        {
            if (_isReadOnly)
            {
                throw NewMessageIsReadOnlyException(this);
            }
            HasChanges = true;
        }

        /// <inheritdoc />
        public void AcceptChanges()
        {
            HasChanges = false;

            foreach (var message in _requestMessageProperties.Values)
            {
                message.AcceptChanges();
            }
        }        

        /// <summary>
        /// Raises the <see cref="HasChangesChanged" /> and <see cref="INotifyPropertyChanged.PropertyChanged" /> events.
        /// </summary>        
        protected virtual void OnHasChangesChanged()
        {
            HasChangesChanged.Raise(this);

            NotifyOfPropertyChange(() => HasChanges);
        }

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged" /> event, signalling all properties have changed.
        /// </summary>
        /// <remarks>
        /// In addition to the base implementation, this method also marks the message as changed and triggers validation.
        /// </remarks>
        protected override void NotifyOfPropertyChange()
        {
            if (_isReadOnly)
            {
                throw NewMessageIsReadOnlyException(this);
            }
            NotifyOfPropertyChange(string.Empty);
            HasChanges = true;
            Validate();
        }

        /// <inheritdoc />
        protected override void NotifyOfPropertyChange(PropertyChangedEventArgs e)
        {            
            base.NotifyOfPropertyChange(e);

            var option = RequestMessageProperty.GetPropertyChangeOption(GetType(), e.PropertyName);

            switch (option)
            {
                case PropertyChangedOption.None:
                    return;

                case PropertyChangedOption.MarkAsChanged:
                    MarkAsChanged();
                    return;

                case PropertyChangedOption.MarkAsChangedAndValidate:
                    MarkAsChanged();
                    Validate();
                    return;

                default:
                    throw NewInvalidOptionException(option);
            }   
        }

        #endregion

        #region [====== SetValue ======]

        /// <summary>
        /// Sets the specified property to the specified value and raises the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
        /// when the value was changed.
        /// </summary>
        /// <typeparam name="TValue">Type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="value">New value of the property.</param>
        /// <param name="currentValue">Old value of the property.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="property"/> is <c>null</c>.
        /// </exception>
        protected void SetValue<TValue>(Expression<Func<TValue>> property, TValue value, ref TValue currentValue)
        {
            SetValue(NameOf(property), value, ref currentValue, null);
        }

        /// <summary>
        /// Sets the specified property to the specified value and raises the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
        /// when the value was changed.
        /// </summary>
        /// <typeparam name="TValue">Type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="value">New value of the property.</param>
        /// <param name="currentValue">Old value of the property.</param>
        /// <param name="propertyChanged">
        /// Optional. when not <c>null</c>, is called when <paramref name="value"/> is unequal to <paramref name="currentValue"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="property"/> is <c>null</c>.
        /// </exception>
        protected void SetValue<TValue>(Expression<Func<TValue>> property, TValue value, ref TValue currentValue, Action propertyChanged)
        {
            SetValue(NameOf(property), value, ref currentValue, propertyChanged);
        }

        /// <summary>
        /// Sets the specified property to the specified value and raises the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
        /// when the value was changed.
        /// </summary>
        /// <typeparam name="TValue">Type of the property.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">New value of the property.</param>
        /// <param name="currentValue">Old value of the property.</param>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyName"/> is <c>null</c>.
        /// </exception>
        protected void SetValue<TValue>(string propertyName, TValue value, ref TValue currentValue)
        {
            SetValue(propertyName, value, ref currentValue, null);
        }

        /// <summary>
        /// Sets the specified property to the specified value and raises the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
        /// when the value was changed.
        /// </summary>
        /// <typeparam name="TValue">Type of the property.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">New value of the property.</param>
        /// <param name="currentValue">Old value of the property.</param>
        /// <param name="propertyChanged">
        /// Optional. when not <c>null</c>, is called when <paramref name="value"/> is unequal to <paramref name="currentValue"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyName"/> is <c>null</c>.
        /// </exception>
        protected void SetValue<TValue>(string propertyName, TValue value, ref TValue currentValue, Action propertyChanged)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }
            if (Equals(currentValue, value))
            {
                return;
            }
            currentValue = value;

            if (propertyChanged != null)
            {
                propertyChanged.Invoke();
            }
            NotifyOfPropertyChange(propertyName);
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
        protected RequestMessageErrorInfo ErrorInfo
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
            get { return ErrorInfo == null && _requestMessageProperties.Values.All(message => message.IsValid); }            
        }

        /// <inheritdoc />
        public event EventHandler IsValidChanged;

        /// <summary>
        /// Raises the <see cref="IsValidChanged" />- and <see cref="INotifyPropertyChanged.PropertyChanged" />-events.
        /// </summary>        
        protected virtual void OnIsValidChanged()
        {
            IsValidChanged.Raise(this);

            NotifyOfPropertyChange(() => IsValid);
        }

        /// <inheritdoc />
        public void Validate()
        {
            ErrorInfo = RequestMessageErrorInfo.CreateErrorInfo(CreateValidationContext());

            foreach (var message in _requestMessageProperties.Values)
            {
                message.Validate();
            }
        }    
        
        /// <summary>
        /// Creates and returns a <see cref="ValidationContext" /> that is used during validation of this message.
        /// </summary>
        /// <returns>
        /// A <see cref="ValidationContext" /> pointing to the current message and contains all services that were
        /// registered through one the message's <see cref="RegisterService{T}" /> methods.
        /// </returns>
        /// <remarks>
        /// A subclass can override this method to return a more specific <see cref="ValidationContext" /> is required.
        /// This may be necessary when a custom <see cref="IServiceProvider">service provider</see> is needed to
        /// perform validation on this message.
        /// </remarks>
        protected virtual ValidationContext CreateValidationContext()
        {
            return new ValidationContext(this, this, null);            
        }

        #endregion

        #region [====== GetMessage and SetMessage ======]

        /// <summary>
        /// Gets the <see cref="IRequestMessage" /> that was set for the specified <paramref name="property"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="property">The property for which the message was set.</param>
        /// <returns>
        /// The message that was set for the specified message; returns <c>null</c> when no message was set.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="property"/> is <c>null</c>.
        /// </exception>
        protected TMessage GetMessage<TMessage>(Expression<Func<TMessage>> property) where TMessage : class, IRequestMessage
        {
            return GetMessage(NameOf(property)) as TMessage;
        }

        /// <summary>
        /// Gets the <see cref="IRequestMessage" /> that was set for the specified <paramref name="propertyName"/>.
        /// </summary>       
        /// <param name="propertyName">The property for which the message was set.</param>
        /// <returns>
        /// The message that was set for the specified message; returns <c>null</c> when no message was set.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyName"/> is <c>null</c>.
        /// </exception>
        protected IRequestMessage GetMessage(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }
            IRequestMessage message;

            if (_requestMessageProperties.TryGetValue(propertyName, out message))
            {
                return message;
            }
            return null;
        }

        /// <summary>
        /// Sets a message for the specified <paramref name="property"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="property">The property for which the message is set.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="property"/> is <c>null</c>.
        /// </exception>
        protected void SetMessage<TMessage>(Expression<Func<TMessage>> property, TMessage message) where TMessage : class, IRequestMessage
        {
            SetMessage(NameOf(property), message);
        }

        /// <summary>
        /// Sets a message for the specified <paramref name="propertyName"/>.
        /// </summary>        
        /// <param name="propertyName">The property for which the message is set.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyName"/> is <c>null</c>.
        /// </exception>
        protected void SetMessage(string propertyName, IRequestMessage message)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }
            IRequestMessage oldMessage;

            if (_requestMessageProperties.TryGetValue(propertyName, out oldMessage))
            {
                if (ReferenceEquals(oldMessage, message))
                {
                    return;
                }
                Detach(oldMessage);
            }
            if (message == null)
            {
                _requestMessageProperties.Remove(propertyName);
            }
            else
            {
                _requestMessageProperties[propertyName] = message;

                Attach(message);
            }
            NotifyOfPropertyChange(propertyName);
        }

        #endregion

        #region [====== Attach and Detach ======]

        private void Attach(IRequestMessage message)
        {            
            message.HasChangesChanged += HandleMessageHasChangesChanged;
            message.IsValidChanged += HandleMessageIsValidChanged;
        }

        private void Detach(IRequestMessage message)
        {            
            message.IsValidChanged -= HandleMessageIsValidChanged;
            message.HasChangesChanged -= HandleMessageHasChangesChanged;
        }

        private void HandleMessageHasChangesChanged(object sender, EventArgs e)
        {
            OnHasChangesChanged();
        }

        private void HandleMessageIsValidChanged(object sender, EventArgs e)
        {
            OnIsValidChanged();
        }

        #endregion

        #region [====== ToString ======]

        ///// <summary>
        ///// Returns a human-readable string containing all relevant property-values.
        ///// </summary>
        ///// <returns>A human-readable string containing all relevant property-values.</returns>
        //public override string ToString()
        //{
        //    var message = new StringBuilder("<");

        //    foreach (var property in RequestMessageProperty.GetProperties(GetType()))
        //    {
        //        var propertyName = property.Name;
        //        var propertyValue = property.GetValue(this);

        //        message.AppendFormat(CultureInfo.InvariantCulture, "{0}: {1}", propertyName, propertyValue);                
        //    }
        //    message.Append(">");

        //    return message.ToString();
        //}        

        #endregion

        #region [====== Exception Factory Methods ======]

        private static Exception NewInvalidOptionException(PropertyChangedOption option)
        {
            var messageFormat = ExceptionMessages.Message_InvalidOption;
            var message = string.Format(messageFormat, typeof(PropertyChangedOption), option);
            return new ArgumentOutOfRangeException("option", message);
        }

        internal static Exception NewMessageIsReadOnlyException(object readonlyMessage)
        {
            var messageFormat = ExceptionMessages.Message_IsReadOnly;
            var message = string.Format(messageFormat, readonlyMessage);
            return new InvalidOperationException(message);
        }

        private static Exception NewServiceAlreadyRegisteredException(string paramName, Type type)
        {
            var messageFormat = ExceptionMessages.Message_ServiceAlreadyRegistered;
            var message = string.Format(messageFormat, type.Name);
            return new ArgumentException(message, paramName);
        }

        #endregion
    }
}
