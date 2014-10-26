using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Messaging.Resources;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Channels;
using System.Text;

namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// Represents a basic implementation of the <see cref="IRequestMessage" /> interface, in which
    /// change tracking and validation are supported.
    /// </summary>
    public abstract class RequestMessage : PropertyChangedBase, IRequestMessage, IServiceProvider, IEditableObject
    {
        private readonly Dictionary<Type, object> _services;                        
        private readonly LinkedList<IRequestMessage> _attachedMessages;
        private readonly bool _isReadOnly;

        private RequestMessageErrorInfo _errorInfo;                       
        private bool _hasChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage" /> class.
        /// </summary>
        protected RequestMessage() : this(false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage" /> class.
        /// </summary>
        /// <param name="makeReadOnly">Indicates whether the new instance should be marked readonly.</param>
        protected RequestMessage(bool makeReadOnly)
        {
            _services = new Dictionary<Type, object>();                                    
            _attachedMessages = new LinkedList<IRequestMessage>();
            _isReadOnly = makeReadOnly;

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
            _attachedMessages = new LinkedList<IRequestMessage>();
            _isReadOnly = makeReadOnly;

            _errorInfo = message._errorInfo == null ? null : new RequestMessageErrorInfo(message._errorInfo);
            _hasChanges = false;
        }

        /// <summary>
        /// Indicates whether or not this message is marked as read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
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

        #region [====== EditableObject ======]        

        public bool IsInEditMode
        {
            get { return RequestMessageEditScope.IsInEditMode(this); }
        }

        /// <summary>
        /// Creates and returns a scope in which members of this message can be altered and
        /// rolled back if required. Validation will be suppressed while the scope is active.
        /// </summary>
        /// <returns>A new scope.</returns>
        public ITransactionalScope CreateEditScope()
        {
            return RequestMessageEditScope.BeginEdit(this, true);
        }

        void IEditableObject.BeginEdit()
        {
            RequestMessageEditScope.BeginEdit(this);

            NotifyOfPropertyChange(() => IsInEditMode);
        }

        void IEditableObject.CancelEdit()
        {
            RequestMessageEditScope.CancelEdit(this);

            NotifyOfPropertyChange(() => IsInEditMode);
        }

        void IEditableObject.EndEdit()
        {
            RequestMessageEditScope.EndEdit(this);

            NotifyOfPropertyChange(() => IsInEditMode);
        }

        #endregion

        #region [====== Change Tracking ======]

        /// <inheritdoc />
        public event EventHandler HasChangesChanged;

        /// <inheritdoc />
        public bool HasChanges
        {
            get { return _hasChanges || _attachedMessages.Any(message => message.HasChanges); }
            private set
            {                
                if (_hasChanges != value)
                {
                    _hasChanges = value;

                    OnHasChangesChanged();
                }
            }
        }

        /// <summary>
        /// Marks this message as changed.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This message is read-only.
        /// </exception>
        protected void MarkAsChanged()
        {            
            HasChanges = true;
        }

        /// <inheritdoc />
        public void AcceptChanges()
        {
            HasChanges = false;

            foreach (var message in _attachedMessages)
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

        /// <inheritdoc />
        protected override void NotifyOfPropertyChange(PropertyChangedEventArgs e)
        {            
            NotifyOfPropertyChange(e, GetDeclaredOptionFor(e.PropertyName));
        }

        protected virtual void NotifyOfPropertyChange(PropertyChangedEventArgs e, PropertyChangedOption option)
        {
            base.NotifyOfPropertyChange(e);

            switch (option)
            {
                case PropertyChangedOption.None:
                    return;

                case PropertyChangedOption.MarkAsChanged:
                    MarkAsChanged();
                    return;

                case PropertyChangedOption.MarkAsChangedAndValidate:
                    MarkAsChangedAndValidate();
                    return;

                default:
                    throw NewInvalidOptionException(option);
            }   
        }

        protected void MarkAsChangedAndValidate()
        {
            MarkAsChanged();
            Validate();
        }

        #endregion        

        #region [====== Validation ======]

        string IDataErrorInfo.this[string columnName]
        {
            get { return IsValid ? null : ErrorInfo[columnName]; }
        }

        string IDataErrorInfo.Error
        {
            get { return IsValid ? null : RequestMessageErrorInfo.Concatenate(GetErrorMessages()); }
        }
        
        internal IEnumerable<string> GetErrorMessages()
        {                  
            return new[] { ErrorInfo == null ? null : ErrorInfo.Error }.Concat(_attachedMessages.Select(message => message.Error));
        }
        
        internal RequestMessageErrorInfo ErrorInfo
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
            get { return ErrorInfo == null && _attachedMessages.All(message => message.IsValid); }            
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
            Validate(true);
        }    

        /// <summary>
        /// Validates this message.
        /// </summary>
        /// <param name="recursive">
        /// Indicates whether or not any attached messages should also be validated.
        /// </param>
        internal virtual void Validate(bool recursive)
        {            
            ErrorInfo = RequestMessageErrorInfo.CreateErrorInfo(CreateValidationContext());

            if (recursive)
            {
                ValidateAttachedMessages();
            }
        }        

        internal void ValidateAttachedMessages()
        {
            foreach (var message in _attachedMessages)
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

        #region [====== SetValue ======]

        /// <summary>
        /// Sets the specified property to the specified value and raises the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
        /// when the value was changed.
        /// </summary>
        /// <typeparam name="TValue">Type of the property.</typeparam>        
        /// <param name="currentValue">Old value of the property.</param> 
        /// <param name="value">New value of the property.</param>               
        /// <param name="property">The property.</param>
        /// <returns><c>true</c> is the specified <paramref name="property"/> was changed; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="property"/> is <c>null</c>.
        /// </exception>
        protected bool SetValue<TValue>(ref TValue currentValue, TValue value, Expression<Func<TValue>> property)
        {
            return SetValue(ref currentValue, value, NameOf(property));
        }

        /// <summary>
        /// Sets the specified property to the specified value and raises the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
        /// when the value was changed.
        /// </summary>
        /// <typeparam name="TValue">Type of the property.</typeparam>        
        /// <param name="currentValue">Old value of the property.</param>
        /// <param name="value">New value of the property.</param>        
        /// <param name="property">The property.</param>
        /// <param name="option">
        /// Indicates which action should be taken when the specified <paramref name="property"/> changes.
        /// </param>
        /// <returns><c>true</c> is the specified <paramref name="property"/> was changed; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="property"/> is <c>null</c>.
        /// </exception>
        protected bool SetValue<TValue>(ref TValue currentValue, TValue value, Expression<Func<TValue>> property, PropertyChangedOption option)
        {
            return SetValue(ref currentValue, value, NameOf(property), option);
        }

        /// <summary>
        /// Sets the specified property to the specified value and raises the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
        /// when the value was changed.
        /// </summary>
        /// <typeparam name="TValue">Type of the property.</typeparam>        
        /// <param name="currentValue">Old value of the property.</param>
        /// <param name="value">New value of the property.</param>        
        /// <param name="propertyName">Name of the property.</param> 
        /// <returns><c>true</c> is the specified <paramref name="propertyName"/> was changed; otherwise <c>false</c>.</returns>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyName"/> is <c>null</c>.
        /// </exception>
        protected bool SetValue<TValue>(ref TValue currentValue, TValue value, string propertyName)
        {
            return SetValue(ref currentValue, value, propertyName, GetDeclaredOptionFor(propertyName));
        }

        /// <summary>
        /// Sets the specified property to the specified value and raises the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
        /// when the value was changed.
        /// </summary>
        /// <typeparam name="TValue">Type of the property.</typeparam>        
        /// <param name="value">New value of the property.</param>
        /// <param name="currentValue">Old value of the property.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="option">
        /// Indicates which action should be taken when the specified <paramref name="propertyName"/> changes.
        /// </param>
        /// <returns><c>true</c> is the specified <paramref name="propertyName"/> was changed; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyName"/> is <c>null</c>.
        /// </exception>
        protected bool SetValue<TValue>(ref TValue currentValue, TValue value, string propertyName, PropertyChangedOption option)
	    {
		    if (IsReadOnly)
		    {
			    throw NewMessageIsReadOnlyException(this);
		    }			
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }
		    if (Equals(currentValue, value))
		    {
			    return false;
		    }		
		    if (IsRequestMessage(typeof(TValue)))
		    {
			    Detach((IRequestMessage) currentValue);
			    Attach((IRequestMessage) value);
		    }		
		    currentValue = value;				
		
		    NotifyOfPropertyChange(new PropertyChangedEventArgs(propertyName), option);
		
		    return true;
	    }

        internal static bool IsRequestMessage(Type type)
        {
            return typeof(IRequestMessage).IsAssignableFrom(type);
        }

        private PropertyChangedOption GetDeclaredOptionFor(string propertyName)
        {
            return RequestMessageProperty.GetPropertyChangeOption(GetType(), propertyName);
        }

        #endregion       

        #region [====== CreateCollection ======]

        /// <summary>
        /// Creates and returns an <see cref="ObservableCollection{T}" /> which is attached to this message for change tracking
        /// and validation.
        /// </summary>
        /// <typeparam name="TValue">Type of the values stored in the collection.</typeparam>               
        /// <returns>
        /// An <see cref="ObservableCollection{T}" /> which is attached to this message for change tracking and validation.
        /// </returns>        
        /// <remarks>
        /// When <typeparamref name="TValue"/> is a <see cref="IRequestMessage" />-type, all values will be treated like child-messages.
        /// </remarks>
        protected ObservableCollection<TValue> AttachCollection<TValue>()
        {
            return AttachCollection(Enumerable.Empty<TValue>());
        }

        /// <summary>
        /// Creates and returns an <see cref="ObservableCollection{T}" /> which is attached to this message for change tracking
        /// and validation.
        /// </summary>
        /// <typeparam name="TValue">Type of the values stored in the collection.</typeparam>        
        /// <param name="values">The initial values stored in the collection.</param>
        /// <returns>
        /// An <see cref="ObservableCollection{T}" /> which is attached to this message for change tracking and validation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// When <typeparamref name="TValue"/> is a <see cref="IRequestMessage" />-type, all values will be treated like child-messages.
        /// </remarks>
        protected ObservableCollection<TValue> AttachCollection<TValue>(IEnumerable<TValue> values)
        {
            var collection = new ObservableCollection<TValue>(values);

            Attach(new ObservableCollectionWrapper<TValue>(collection, IsReadOnly));

            return collection;
        }               

        #endregion

        #region [====== Attach and Detach ======]

        protected TMessage AttachCopy<TMessage>(TMessage messageToCopy) where TMessage : class, IRequestMessage
        {
            if (messageToCopy == null)
            {
                return null;
            }
            return Attach((TMessage) messageToCopy.Copy(IsReadOnly));
        }

        protected TMessage Attach<TMessage>() where TMessage : class, IRequestMessage, new()
        {
            return Attach(new TMessage());
        }

        protected TMessage Attach<TMessage>(TMessage message) where TMessage : class, IRequestMessage
        {
            Attach(message as IRequestMessage);

            return message;
        }

        internal void Attach(IRequestMessage message)
        {
            if (message != null)
            {
                message.HasChangesChanged += HandleMessageHasChangesChanged;
                message.IsValidChanged += HandleMessageIsValidChanged;

                _attachedMessages.AddLast(message);
            }
        }

        internal void Detach(IRequestMessage message)
        {
            if (message != null)
            {
                message.IsValidChanged -= HandleMessageIsValidChanged;
                message.HasChangesChanged -= HandleMessageHasChangesChanged;

                _attachedMessages.Remove(message);
            }
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

        internal static Exception NewInvalidOptionException(PropertyChangedOption option)
        {
            var messageFormat = ExceptionMessages.Message_InvalidOption;
            var message = string.Format(messageFormat, typeof(PropertyChangedOption), option);
            return new ArgumentOutOfRangeException("option", message);
        }
        
        internal static InvalidOperationException NewMessageIsReadOnlyException(object instance)
        {
            var messageFormat = ExceptionMessages.Message_IsReadOnly;
            var message = string.Format(messageFormat, instance);
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
