using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Resources;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a basic implementation of the <see cref="IRequestMessageViewModel" /> interface, in which
    /// change tracking and validation are supported.
    /// </summary>
    [Serializable]
    public abstract class RequestMessageViewModel<TMessage> : PropertyChangedBase,
                                                              IRequestMessageViewModel<TMessage>,
                                                              IEditableObject,
                                                              IServiceProvider,
                                                              IExtensibleDataObject,
                                                              ISerializable
        where TMessage : RequestMessageViewModel<TMessage>
    {        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly LinkedList<IRequestMessageViewModel> _attachedMessages;

        private readonly Dictionary<Type, object> _services;
        private ValidationErrorTree _errorTree;
        private ExtensionDataObject _extensionData;

        private readonly bool _isReadOnly;                     
        private bool _hasChanges;       

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageViewModel{TMessage}" /> class.
        /// </summary>
        protected RequestMessageViewModel() : this(false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageViewModel{TMessage}" /> class.
        /// </summary>
        /// <param name="makeReadOnly">Indicates whether the new instance should be marked readonly.</param>
        protected RequestMessageViewModel(bool makeReadOnly)
        {           
            _attachedMessages = new LinkedList<IRequestMessageViewModel>();
            _services = new Dictionary<Type, object>();
            _errorTree = ValidationErrorTree.NoErrors;
            _isReadOnly = makeReadOnly;                        
        }                

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="message">The message to copy.</param>
        /// <param name="makeReadOnly">Indicates whether the new instance should be marked readonly.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected RequestMessageViewModel(TMessage message, bool makeReadOnly)
        {            
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _attachedMessages = new LinkedList<IRequestMessageViewModel>();
            _services = new Dictionary<Type, object>(message._services);
            _errorTree = ValidationErrorTree.NoErrors;
            _extensionData = message._extensionData;      
            _isReadOnly = makeReadOnly;                         
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageViewModel{TMessage}" /> class by deserializing it's contents
        /// from a <see cref="SerializationInfo" /> instance.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected RequestMessageViewModel(SerializationInfo info, StreamingContext context)
        {
            _attachedMessages = new LinkedList<IRequestMessageViewModel>();
            _services = new Dictionary<Type, object>();
            _errorTree = ValidationErrorTree.NoErrors;            
        }

        string IMessage.TypeId
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// Indicates whether or not this message is marked as read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
        }

        #region [====== ExtensibleObject ======]

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ExtensionDataObject IExtensibleDataObject.ExtensionData
        {
            get { return _extensionData; }
            set { _extensionData = value; }
        }

        #endregion

        #region [====== Serialization ======]

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            GetObjectData(info, context);
        }

        /// <summary>
        /// Populates a <see cref="SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context) { }

        #endregion

        #region [====== Copy ======]

        IMessage IMessage.Copy()
        {
            return Copy(true);
        }

        TMessage IMessage<TMessage>.Copy()
        {
            return Copy(true);
        }  
        
        IRequestMessageViewModel IRequestMessageViewModel.Copy(bool makeReadOnly)
        {
            return Copy(makeReadOnly);
        }

        TMessage IRequestMessageViewModel<TMessage>.Copy(bool makeReadOnly)
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

        /// <summary>
        /// Indicates whether or not this message is in edit-mode.
        /// </summary>
        public bool IsInEditMode
        {
            get { return RequestMessageViewModelEditScope.IsInEditMode(this); }
        }

        /// <inheritdoc />
        public RequestMessageViewModelEditScope CreateEditScope()
        {
            return CreateEditScope(false, null);
        }

        /// <inheritdoc />
        public RequestMessageViewModelEditScope CreateEditScope(object state)
        {
            return CreateEditScope(false, state);
        }

        /// <inheritdoc />
        public RequestMessageViewModelEditScope CreateEditScope(bool suppressValidation)
        {
            return CreateEditScope(suppressValidation, null);
        }

        /// <inheritdoc />
        public RequestMessageViewModelEditScope CreateEditScope(bool suppressValidation, object state)
        {
            return RequestMessageViewModelEditScope.BeginEdit(this, suppressValidation, state, true);
        }

        void IEditableObject.BeginEdit()
        {
            RequestMessageViewModelEditScope.BeginEdit(this);

            NotifyOfPropertyChange(() => IsInEditMode);
        }

        void IEditableObject.CancelEdit()
        {
            RequestMessageViewModelEditScope.CancelEdit(this);

            NotifyOfPropertyChange(() => IsInEditMode);
        }

        void IEditableObject.EndEdit()
        {
            RequestMessageViewModelEditScope.EndEdit(this);

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

            if (RequestMessageViewModelEditScope.IsValidationSuppressed(this))
            {
                return;
            }
            Validate();
        }

        #endregion        

        #region [====== Validation ======]

        string IDataErrorInfo.this[string columnName]
        {
            get { return _errorTree == null ? null : ErrorTree[columnName]; }
        }

        string IDataErrorInfo.Error
        {
            get { return _errorTree == null ? null : ErrorTree.Error; }
        }                
        
        private IDataErrorInfo ErrorTree
        {
            get { return _errorTree; }            
        }

        /// <inheritdoc />
        public bool IsValid
        {
            get { return _errorTree == null && (_attachedMessages == null || _attachedMessages.All(message => message.IsValid)); }            
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
            var validator = CreateValidator();
            if (validator != null)
            {
                var oldValue = IsValid;
                var newValue = !TryGetValidationErrors(validator, out _errorTree) && ValidateAttachedMessages();

                if (oldValue != newValue)
                {
                    OnIsValidChanged();
                }    
            }            
        }   
        
        private static bool TryGetValidationErrors(IMessageValidator validator, out ValidationErrorTree errorTree)
        {
            errorTree = validator.Validate();

            if (errorTree.Errors.Count == 0)
            {
                errorTree = null;
                return false;
            }
            return true;
        }

        internal bool ValidateAttachedMessages()
        {
            if (_attachedMessages.Count > 0)
            {
                foreach (var message in _attachedMessages)
                {
                    message.Validate();
                }
                return _attachedMessages.All(message => message.IsValid);
            }
            return true;
        }

        ValidationErrorTree IMessage.Validate()
        {
            return Validate(true);
        }

        internal virtual ValidationErrorTree Validate(bool includeChildErrors)
        {            
            // First, we validate this message.
            var errorTree = ValidationErrorTree.NoErrors;

            var validator = CreateValidator();
            if (validator != null)
            {
                errorTree = validator.Validate();
            }                        

            // Second, if required, we validate all children and merge all errors into a single ValidationErrorTree.
            if (includeChildErrors)
            {
                errorTree = ValidationErrorTree.Merge(errorTree, Validate(_attachedMessages));
            }            
            return errorTree;
        }

        internal static IEnumerable<ValidationErrorTree> Validate(IEnumerable<IMessage> messages)
        {
            return messages.Select(message => message.Validate());
        }        

        /// <inheritdoc />
        protected virtual IMessageValidator CreateValidator()
        {
            return new MessageValidator(this, () => new ValidationContext(this, this, null));
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
		    if (IsRequestMessageViewModel(typeof(TValue)))
		    {
			    Detach((IRequestMessageViewModel) currentValue);
			    Attach((IRequestMessageViewModel) value);
		    }
            var state = RequestMessageViewModelEditScope.GetEditScopeState(this);
            var eventArgs = new RequestMessagePropertyChangedEventArgs(propertyName, oldValue, newValue, state);

		    currentValue = value;				
		
		    NotifyOfPropertyChange(eventArgs, option);		
		    return true;
	    }

        internal static bool IsRequestMessageViewModel(Type type)
        {
            return typeof(IRequestMessageViewModel).IsAssignableFrom(type);
        }        

        #endregion

        #region [====== Attach and Detach ======]

        /// <summary>
        /// Creates and returns an <see cref="AttachedCollection{T}" /> which is attached to this message for change tracking
        /// and validation.
        /// </summary>
        /// <typeparam name="TValue">Type of the values stored in the collection.</typeparam>               
        /// <returns>
        /// An <see cref="AttachedCollection{T}" /> which is attached to this message for change tracking and validation.
        /// </returns>        
        /// <remarks>
        /// When <typeparamref name="TValue"/> is a <see cref="IRequestMessageViewModel" />-type, all values will be treated like child-messages.
        /// </remarks>
        protected AttachedCollection<TValue> AttachCollection<TValue>()
        {
            return AttachCollectionCopy(Enumerable.Empty<TValue>());
        }

        /// <summary>
        /// Creates and returns an <see cref="AttachedCollection{T}" /> which is attached to this message for change tracking
        /// and validation by deserializing it from the specified <see cref="SerializationInfo" />.
        /// </summary>
        /// <typeparam name="TValue">Type of the values stored in the collection.</typeparam>    
        /// <param name="info">The serialization info.</param>
        /// <param name="name">Name of the collection to retrieve.</param>
        /// <returns>An <see cref="AttachedCollection{T}" /> which is attached to this message for change tracking and validation.</returns>
        protected AttachedCollection<TValue> AttachCollection<TValue>(SerializationInfo info, string name)
        {
            return AttachCollection((IEnumerable<TValue>) info.GetValue(name, typeof(TValue[])));
        }

        /// <summary>
        /// Creates and returns an <see cref="AttachedCollection{T}" /> which is attached to this message for change tracking
        /// and validation.
        /// </summary>
        /// <typeparam name="TValue">Type of the values stored in the collection.</typeparam>        
        /// <param name="values">The initial values stored in the collection.</param>
        /// <returns>
        /// An <see cref="AttachedCollection{T}" /> which is attached to this message for change tracking and validation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// When <typeparamref name="TValue"/> is a <see cref="IRequestMessageViewModel" />-type, all values will be treated like child-messages.
        /// </remarks>
        protected AttachedCollection<TValue> AttachCollection<TValue>(IEnumerable<TValue> values)
        {
            return AttachCollectionCopy(values);
        }        

        /// <summary>
        /// Creates and returns an <see cref="AttachedCollection{T}" /> which is attached to this message for change tracking
        /// and validation.
        /// </summary>
        /// <typeparam name="TValue">Type of the values stored in the collection.</typeparam>        
        /// <param name="values">The initial values stored in the collection.</param>
        /// <returns>
        /// An <see cref="AttachedCollection{T}" /> which is attached to this message for change tracking and validation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// When <typeparamref name="TValue"/> is a <see cref="IRequestMessageViewModel" />-type, all values will be treated like child-messages.
        /// </remarks>
        protected AttachedCollection<TValue> AttachCollectionCopy<TValue>(IEnumerable<TValue> values)
        {            
            return Attach(new AttachedCollection<TValue>(values, IsReadOnly));            
        }

        /// <summary>
        /// Creates and returns a copy of the specified <paramref name="message"/> that is also automatically attached as child to this message.
        /// </summary>
        /// <typeparam name="T">Type of the message to copy.</typeparam>
        /// <param name="message">The message to copy.</param>
        /// <returns>
        /// A copy of <paramref name="message"/>, or <c>null</c> if <paramref name="message"/> is <c>null</c>.
        /// </returns>
        protected T AttachCopy<T>(T message) where T : class, IRequestMessageViewModel<T>
        {                        
            return message == null ? null : Attach(message.Copy(IsReadOnly));
        }

        /// <summary>
        /// Deserializes a message of type <typeparamref name="T"/> from the specified <see cref="SerializationInfo" />
        /// and attached it to this message.
        /// </summary>
        /// <typeparam name="T">Type of the message to deserialize.</typeparam>
        /// <param name="info">The serialization info.</param>
        /// <param name="name">Name of the message to retrieve.</param>
        /// <returns>The attached message.</returns>
        protected T Attach<T>(SerializationInfo info, string name) where T : class, IRequestMessageViewModel
        {
            return Attach((T) info.GetValue(name, typeof(T)));
        }

        /// <summary>
        /// Attaches the specified <paramref name="message"/> to this message and immediately returns it.
        /// </summary>
        /// <typeparam name="T">Type of the message to attach.</typeparam>
        /// <param name="message">The message to attach.</param>
        /// <returns>The specified <paramref name="message"/>.</returns>
        protected T Attach<T>(T message) where T : class, IRequestMessageViewModel
        {
            Attach(message as IRequestMessageViewModel);

            return message;
        }

        internal void Attach(IRequestMessageViewModel message)
        {
            if (message != null)
            {
                message.HasChangesChanged += HandleMessageHasChangesChanged;
                message.IsValidChanged += HandleMessageIsValidChanged;

                _attachedMessages.AddLast(message);
            }
        }

        internal void Detach(IRequestMessageViewModel message)
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

        /// <summary>
        /// Returns a human-readable string containing all relevant property-values.
        /// </summary>
        /// <returns>A human-readable string containing all relevant property-values.</returns>
        public override string ToString()
        {
            return string.Format("{0} ({1}, {2})",
                GetType().Name,
                IsReadOnly ? "ReadOnly" : (HasChanges ? "Changed" : "Unchanged"),
                IsValid ? "Valid" : string.Format("Invalid: {0} error(s)", _errorTree == null ? 0 : _errorTree.TotalErrorCount));            
        }        

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
