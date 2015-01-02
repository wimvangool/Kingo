using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Resources;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a basic implementation of the <see cref="IRequestMessage{TMessage}" /> interface, in which
    /// change tracking and validation are supported.
    /// </summary>
    [Serializable]
    public abstract class RequestMessage<TMessage> : PropertyChangedBase, IRequestMessage<TMessage>, IEditableObject, IServiceProvider, IExtensibleDataObject
        where TMessage : RequestMessage<TMessage>
    {
        // The _validator and _attachedMessages fields could have been made readonly but were not because we want to support the use
        // of WCF's DataContractSerializer, which does not call constructors upon deserialization. For this reason, we use the
        // lazy initialization pattern.

        [NonSerialized]
        private readonly bool _isReadOnly;

        [NonSerialized]
        private RequestMessageValidator<TMessage> _validator;

        [NonSerialized]
        private LinkedList<IRequestMessage> _attachedMessages;

        [NonSerialized]
        private bool _hasChanges;

        [NonSerialized]
        private ExtensionDataObject _extensionData;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage{TMessage}" /> class.
        /// </summary>
        protected RequestMessage() : this(false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessage{TMessage}" /> class.
        /// </summary>
        /// <param name="makeReadOnly">Indicates whether the new instance should be marked readonly.</param>
        protected RequestMessage(bool makeReadOnly)
        {            
            _isReadOnly = makeReadOnly;            
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
        protected RequestMessage(TMessage message, bool makeReadOnly)
        {            
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _isReadOnly = makeReadOnly;  
            _validator = message._validator == null ? null : new RequestMessageValidator<TMessage>(this, message._validator);                                  
            _hasChanges = false;
        }

        internal RequestMessageValidator<TMessage> Validator
        {
            get
            {
                if (_validator == null)
                {
                    _validator = new RequestMessageValidator<TMessage>(this);
                }
                return _validator;
            }
        }

        private LinkedList<IRequestMessage> AttachedMessages
        {
            get
            {
                if (_attachedMessages == null)
                {
                    _attachedMessages = new LinkedList<IRequestMessage>();
                }
                return _attachedMessages;
            }
        }

        /// <summary>
        /// Indicates whether or not this message is marked as read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
        }

        ExtensionDataObject IExtensibleDataObject.ExtensionData
        {
            get { return _extensionData; }
            set { _extensionData = value; }
        }

        #region [====== Message Copying ======]

        IMessage IMessage.Copy()
        {
            return Copy(true);
        }

        TMessage IMessage<TMessage>.Copy()
        {
            return Copy(true);
        }

        IRequestMessage IRequestMessage.Copy(bool makeReadOnly)
        {
            return Copy(makeReadOnly);
        }

        TMessage IRequestMessage<TMessage>.Copy(bool makeReadOnly)
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
        public abstract TMessage Copy(bool makeReadOnly);                      

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
                Validator.Add(typeof(TService), service);
            }
            catch (ArgumentException)
            {
                throw NewServiceAlreadyRegisteredException("service", typeof(TService));
            }
            return true;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return Validator.GetService(serviceType);
        }

        #endregion

        #region [====== EditableObject ======]

        /// <summary>
        /// Indicates whether or not this message is in edit-mode.
        /// </summary>
        public bool IsInEditMode
        {
            get { return RequestMessageEditScope.IsInEditMode(this); }
        }

        /// <inheritdoc />
        public RequestMessageEditScope CreateEditScope()
        {
            return CreateEditScope(false, null);
        }

        /// <inheritdoc />
        public RequestMessageEditScope CreateEditScope(object state)
        {
            return CreateEditScope(false, state);
        }

        /// <inheritdoc />
        public RequestMessageEditScope CreateEditScope(bool suppressValidation)
        {
            return CreateEditScope(suppressValidation, null);
        }

        /// <inheritdoc />
        public RequestMessageEditScope CreateEditScope(bool suppressValidation, object state)
        {
            return RequestMessageEditScope.BeginEdit(this, suppressValidation, state, true);
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
            get { return _hasChanges || AttachedMessages.Any(message => message.HasChanges); }
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

            if (_attachedMessages == null)
            {
                return;
            }
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
        
        #endregion

        #region [====== Property Changes ======]

        /// <inheritdoc />
        protected void NotifyOfPropertyChange(RequestMessagePropertyChangedEventArgs e)
        {            
            NotifyOfPropertyChange(e, PropertyChangedOption.None);
        }

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged" /> event.
        /// </summary>
        /// <param name="e">Arguments that contain the name of the property that has changed.</param>
        /// <param name="option">
        /// Determines which action(s) should follow the change of the property.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="option"/> is not a valid <see cref="PropertyChangedOption" />.
        /// </exception>
        protected virtual void NotifyOfPropertyChange(RequestMessagePropertyChangedEventArgs e, PropertyChangedOption option)
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

        internal void MarkAsChangedAndValidate()
        {
            MarkAsChanged();
            Validate(false, true);
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
        
        internal RequestMessageErrorInfo ErrorInfo
        {
            get { return Validator.ErrorInfo; }            
        }

        /// <inheritdoc />
        public bool IsValid
        {
            get { return Validator.ErrorInfo == null && (_attachedMessages == null || _attachedMessages.All(message => message.IsValid)); }            
        }

        /// <inheritdoc />
        public event EventHandler IsValidChanged;

        /// <summary>
        /// Raises the <see cref="IsValidChanged" />- and <see cref="INotifyPropertyChanged.PropertyChanged" />-events.
        /// </summary>        
        protected internal virtual void OnIsValidChanged()
        {
            IsValidChanged.Raise(this);

            NotifyOfPropertyChange(() => IsValid);
        }

        bool IMessageValidator<TMessage>.IsNotValid(TMessage message, out MessageErrorTree errorTree)
        {
            var requestMessage = message as IRequestMessage;
            if (requestMessage == null)
            {
                throw new ArgumentNullException("message");
            }
            return requestMessage.IsNotValid(out errorTree);
        }

        /// <inheritdoc />
        bool IRequestMessage.IsNotValid(out MessageErrorTree errorTree)
        {
            return IsNotValid(out errorTree);
        }

        internal virtual bool IsNotValid(out MessageErrorTree errorTree)
        {
            Validate(true, false);

            if (IsValid)
            {
                errorTree = null;
                return false;
            }
            errorTree = RequestMessageErrorInfo.CreateErrorTree(this, _attachedMessages);
            return true;
        }        

        /// <inheritdoc />
        public void Validate()
        {
            Validate(true, true);
        }        
        
        internal virtual void Validate(bool ignoreEditScope, bool validateAttachedMessages)
        {
            if (Validator.TryValidateMessage(ignoreEditScope, CreateValidationContext()) && validateAttachedMessages)
            {
                ValidateAttachedMessages();
            }           
        }        

        internal void ValidateAttachedMessages()
        {
            if (_attachedMessages == null)
            {
                return;
            }
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
            return SetValue(ref currentValue, value, propertyName, PropertyChangedOption.MarkAsChangedAndValidate);
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
            var oldValue = currentValue;
            var newValue = value;

		    if (Equals(oldValue, newValue))
		    {
			    return false;
		    }		
		    if (IsRequestMessage(typeof(TValue)))
		    {
			    Detach((IRequestMessage) currentValue);
			    Attach((IRequestMessage) value);
		    }
            var state = RequestMessageEditScope.GetEditScopeState(this);
            var eventArgs = new RequestMessagePropertyChangedEventArgs(propertyName, oldValue, newValue, state);

		    currentValue = value;				
		
		    NotifyOfPropertyChange(eventArgs, option);		
		    return true;
	    }

        internal static bool IsRequestMessage(Type type)
        {
            return typeof(IRequestMessage).IsAssignableFrom(type);
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

        /// <summary>
        /// Creates and returns a copy of the specified <paramref name="message"/> that is also automatically attached as child to this message.
        /// </summary>
        /// <typeparam name="T">Type of the message to copy.</typeparam>
        /// <param name="message">The message to copy.</param>
        /// <returns>
        /// A copy of <paramref name="message"/>, or <c>null</c> if <paramref name="message"/> is <c>null</c>.
        /// </returns>
        protected T AttachCopy<T>(T message) where T : class, IRequestMessage<T>
        {
            if (message == null)
            {
                return null;
            }
            return Attach(message.Copy(IsReadOnly));
        }

        /// <summary>
        /// Creates and returns a new instance of <typeparamref name="TMessage"/> that is also automatically attached as child to this message.
        /// </summary>
        /// <typeparam name="T">Type of the message to create.</typeparam>
        /// <returns>A new instance of <typeparamref name="TMessage"/>.</returns>
        protected T Attach<T>() where T : class, IRequestMessage, new()
        {
            return Attach(new T());
        }

        /// <summary>
        /// Attaches the specified <paramref name="message"/> to this message and immediately returns it.
        /// </summary>
        /// <typeparam name="T">Type of the message to attach.</typeparam>
        /// <param name="message">The message to attach.</param>
        /// <returns>The specified <paramref name="message"/>.</returns>
        protected T Attach<T>(T message) where T : class, IRequestMessage
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

                AttachedMessages.AddLast(message);
            }
        }

        internal void Detach(IRequestMessage message)
        {
            if (message != null)
            {
                message.IsValidChanged -= HandleMessageIsValidChanged;
                message.HasChangesChanged -= HandleMessageHasChangesChanged;

                AttachedMessages.Remove(message);
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
