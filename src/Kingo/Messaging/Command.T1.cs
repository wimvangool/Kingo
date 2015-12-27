using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a command that re-validates itself on every change and implements the <see cref="INotifyPropertyChanging" />,
    /// <see cref="INotifyPropertyChanged" /> and <see cref="IDataErrorInfo" /> interfaces so that it can easily be bound to
    /// in WPF applications.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    [DataContract]
    [Serializable]
    public abstract class Command<TMessage> : Message<TMessage>, INotifyPropertyChanging, INotifyPropertyChanged, IDataErrorInfo where TMessage : Command<TMessage>
    {
        private const string _HasChangesProperty = "HasChanges";
        private const string _ErrorInfoProperty = "ErrorInfo";

        [NonSerialized]
        private ErrorInfo _errorInfo;

        [NonSerialized]
        private bool _hasChanges;        

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}" /> class.
        /// </summary>
        protected Command() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}" /> class.
        /// </summary>
        /// <param name="message">The message to copy.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected Command(Command<TMessage> message)
            : base(message) { }

        #region [====== HasChanges ======]

        /// <summary>
        /// Indicates whether or not this message has changes since it was created or the last time
        /// <see cref="AcceptChanges()" /> was called.
        /// </summary>
        public bool HasChanges
        {
            get { return _hasChanges; }
            private set { SetValue(ref _hasChanges, value, _HasChangesProperty); }
        }

        /// <summary>
        /// Sets <see cref="HasChanges" /> to <c>false</c>.
        /// </summary>
        public void AcceptChanges()
        {
            HasChanges = false;
        }

        #endregion

        #region [====== DataErrorInfo ======]

        /// <summary>
        /// Indicates whether or not this message is valid.
        /// </summary>
        public bool HasErrors
        {
            get { return ErrorInfo != null && ErrorInfo.HasErrors; }           
        }

        /// <summary>
        /// Returns the set of validation errors related to this message.
        /// </summary>
        public ErrorInfo ErrorInfo
        {
            get
            {
                // Just-In-Time validation only occurs the first time the ErrorInfo
                // property is accessed and no properties have changed to trigger validation
                // the 'normal' way.
                if (_errorInfo == null)
                {
                    _errorInfo = Validate();
                }
                return _errorInfo;
            }
            private set { SetValue(ref _errorInfo, value, _ErrorInfoProperty); }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {                
                string errorMessage;

                if (ErrorInfo.MemberErrors.TryGetValue(columnName, out errorMessage))
                {
                    return errorMessage;
                }
                return null;
            }
        }

        string IDataErrorInfo.Error
        {
            get { return ErrorInfo.Error; }
        }

        #endregion

        #region [====== SetValue ======]

        /// <summary>
        /// Assigns the specified <paramref name="newValue"/> to the <paramref name="oldValue"/> and raises
        /// the <see cref="PropertyChanging" /> and <see cref="PropertyChanged" /> events, if and only if
        /// the values are different.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property value.</typeparam>
        /// <param name="oldValue">The current value of the property.</param>
        /// <param name="newValue">The value to assign.</param>
        /// <param name="expression">
        /// An expression referring to the property that is about to be changed.
        /// </param>
        /// <param name="comparer">
        /// The comparer that is used to determine whether <paramref name="oldValue"/> and <paramref name="newValue"/> are
        /// equal or not. If <c>null</c> is specified, the default comparer is used.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="expression"/> does not refer to a property.
        /// </exception>
        protected void SetValue<TProperty>(ref TProperty oldValue, TProperty newValue, Expression<Func<TProperty>> expression, IEqualityComparer<TProperty> comparer = null)
        {
            SetValue(ref oldValue, newValue, expression.ExtractMemberName().ToString());
        }

        /// <summary>
        /// Assigns the specified <paramref name="newValue"/> to the <paramref name="oldValue"/> and raises
        /// the <see cref="PropertyChanging" /> and <see cref="PropertyChanged" /> events, if and only if
        /// the values are different.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property value.</typeparam>
        /// <param name="oldValue">The current value of the property.</param>
        /// <param name="newValue">The value to assign.</param>
        /// <param name="propertyName">
        /// Name of the property that is about to be changed.
        /// </param>
        /// <param name="comparer">
        /// The comparer that is used to determine whether <paramref name="oldValue"/> and <paramref name="newValue"/> are
        /// equal or not. If <c>null</c> is specified, the default comparer is used.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyName"/> is <c>null</c>.
        /// </exception>
        protected void SetValue<TProperty>(ref TProperty oldValue, TProperty newValue, string propertyName, IEqualityComparer<TProperty> comparer = null)
        {
            var valueComparer = comparer ?? EqualityComparer<TProperty>.Default;
            if (valueComparer.Equals(oldValue, newValue))
            {
                return;
            }
            OnPropertyChanging(propertyName);

            oldValue = newValue;

            OnPropertyChanged(propertyName);
        }

        #endregion

        #region [====== NotifyPropertyChanging ======]

        /// <inheritdoc />
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Raises the <see cref="PropertyChanging" /> event, indicating that the entire message is about to be changed.       
        /// </summary>
        protected void OnPropertyChanging()
        {
            OnPropertyChanging(string.Empty);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanging" /> event, indicating that the property referred to by the
        /// specified <paramref name="expression"/> is about to be changed.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property that has changed.</typeparam>
        /// <param name="expression">
        /// An expression referring to the property that has changed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="expression"/> does not refer to a property.
        /// </exception>
        protected void OnPropertyChanging<TProperty>(Expression<Func<TProperty>> expression)
        {
            OnPropertyChanging(expression.ExtractMemberName().ToString());
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanging" /> event, indicating that the property with the
        /// specified <paramref name="propertyName"/> is about to be changed.
        /// </summary>                
        /// <param name="propertyName">Name of the property that has changed.</param>
        protected void OnPropertyChanging(string propertyName)
        {
            OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanging" /> event with the specified argument.
        /// </summary>
        /// <param name="e">Argument of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
        {            
            PropertyChanging.Raise(this, e);
        }

        #endregion

        #region [====== NotifyPropertyChanged ======]

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event, indicating that the entire message has changed
        /// and all bindings need to be refreshed.
        /// </summary>
        protected void OnPropertyChanged()
        {
            OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event, indicating that the property referred to by the
        /// specified <paramref name="expression"/> has changed.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property that has changed.</typeparam>
        /// <param name="expression">
        /// An expression referring to the property that has changed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="expression"/> does not refer to a property.
        /// </exception>
        protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> expression)
        {
            OnPropertyChanged(expression.ExtractMemberName().ToString());
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event, indicating that the property with the
        /// specified <paramref name="propertyName"/> has changed.
        /// </summary>                
        /// <param name="propertyName">Name of the property that has changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event with the specified argument.
        /// </summary>
        /// <param name="e">Argument of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var isCustomProperty = IsCustomProperty(e.PropertyName);
            if (isCustomProperty)
            {
                HasChanges = true;
            }
            PropertyChanged.Raise(this, e);

            if (isCustomProperty)
            {
                ErrorInfo = Validate();
            }
        }

        private static bool IsCustomProperty(string propertyName)
        {
            return propertyName != _HasChangesProperty && propertyName != _ErrorInfoProperty;
        }

        #endregion
    }
}
