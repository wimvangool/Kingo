using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.ComponentModel
{
    /// <summary>
    /// Represents a scope in which changes can be made to a <see cref="IRequestMessageViewModel" /> that can be rolled back if required.
    /// </summary>
    public sealed class RequestMessageViewModelEditScope : IDisposable
    {
        private readonly RequestMessageViewModelEditScope _parentScope;
        private readonly IRequestMessageViewModel _message;
        private readonly IRequestMessageViewModel _messageBackup;        
        private readonly bool _suppressValidation;
        private readonly object _state;

        private readonly HashSet<string> _changedProperties;
        private bool _messageValidationWasFired;
        private bool _hasCompleted;
        private bool _isDisposed;        

        private RequestMessageViewModelEditScope(IRequestMessageViewModel message, IRequestMessageViewModel messageBackup, bool suppressValidation, object state)
        {
            _message = message;
            _message.PropertyChanged += HandleMessagePropertyChanged;
            _messageBackup = messageBackup;
            
            _changedProperties = new HashSet<string>();
            _suppressValidation = suppressValidation;
            _state = state;
        }

        private RequestMessageViewModelEditScope(RequestMessageViewModelEditScope parentScope, IRequestMessageViewModel messageBackup, bool suppressValidation, object state)
        {
            _parentScope = parentScope;
            _message = parentScope._message;
            _message.PropertyChanged += HandleMessagePropertyChanged;
            _messageBackup = messageBackup;

            _changedProperties = new HashSet<string>();
            _suppressValidation = suppressValidation;
            _state = state;
        }

        private void HandleMessagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName;
            if (propertyName == null)
            {
                return;
            }
            _changedProperties.Add(propertyName);            
        }

        private RequestMessageViewModelEditScope CreateNestedScope(bool suppressValidation, object state)
        {
            return new RequestMessageViewModelEditScope(this, _message.Copy(true), suppressValidation, state); ;
        }

        private RequestMessageViewModelEditScope ParentScope
        {
            get { return _parentScope; }
        }

        private IRequestMessageViewModel Message
        {
            get { return _message; }
        }

        /// <summary>
        /// The message that is used as a backup to rollback any changes if so required.
        /// </summary>
        public IRequestMessageViewModel MessageBackup
        {
            get { return _messageBackup; }
        }        

        /// <summary>
        /// Custom state object that can be used to relate changes to a specific scope.
        /// </summary>
        public object State
        {
            get { return _state; }
        }

        private bool SuppressesValidation()
        {
            _messageValidationWasFired = true;

            return _suppressValidation | (_parentScope != null && _parentScope.SuppressesValidation());
        }

        /// <inheritdoc />
        public void Complete()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(typeof(RequestMessageViewModelEditScope).Name);
            }
            if (_hasCompleted)
            {
                throw NewScopeAlreadyCompletedException();
            }
            if (IsNotCurrentScope())
            {
                throw NewCannotCompleteScopeException();
            }            
            if (_messageValidationWasFired && _suppressValidation && (_parentScope == null || !_parentScope.SuppressesValidation()))
            {
                _message.Validate();
            }
            _changedProperties.Clear();
            _hasCompleted = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }            
            if (IsNotCurrentScope())
            {
                throw NewIncorrectNestingOfScopesException();
            }
            _message.PropertyChanged -= HandleMessagePropertyChanged;
            _isDisposed = true;

            if (_hasCompleted)
            {
                EndEdit(_message);
            }
            else
            {
                CancelEdit(_message);
            }
        }

        private bool IsNotCurrentScope()
        {
            RequestMessageViewModelEditScope editScope;

            if (MessagesInEditMode.TryGetValue(Message, out editScope))
            {
                return !ReferenceEquals(this, editScope);
            }
            return true;
        }

        private void RestoreBackup()
        {
            foreach (var property in PropertiesToRestore())
            {
                RestorePropertyValue(property);
            }
            _changedProperties.Clear();
        }

        private IEnumerable<PropertyInfo> PropertiesToRestore()
        {
            if (_changedProperties.Count == 0)
            {
                return Enumerable.Empty<PropertyInfo>();
            }
            return from property in _message.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   where _changedProperties.Contains(property.Name)
                   select property;
        }

        private void RestorePropertyValue(PropertyInfo property)
        {
            var propertySetter = property.GetSetMethod(true);
            if (propertySetter != null)
            {
                propertySetter.Invoke(_message, new [] { property.GetValue(_messageBackup, null) });
            }            
        }

        #region [====== Exception Factory Methods ======]

        private static Exception NewScopeAlreadyCompletedException()
        {
            return new InvalidOperationException(ExceptionMessages.Scope_ScopeAlreadyCompleted);
        }

        private static Exception NewCannotCompleteScopeException()
        {
            return new InvalidOperationException(ExceptionMessages.Scope_CannotCompleteScope);
        }

        private static Exception NewIncorrectNestingOfScopesException()
        {
            return new InvalidOperationException(ExceptionMessages.Scope_IncorrectNesting);
        }

        #endregion

        #region [====== BeginEdit, CancelEdit & EndEdit ======]

        private static readonly ThreadLocal<Dictionary<IRequestMessageViewModel, RequestMessageViewModelEditScope>> _MessagesInEditMode;

        static RequestMessageViewModelEditScope()
        {
            _MessagesInEditMode = new ThreadLocal<Dictionary<IRequestMessageViewModel, RequestMessageViewModelEditScope>>(CreateScopeDictionary);
        }   
     
        private static Dictionary<IRequestMessageViewModel, RequestMessageViewModelEditScope> CreateScopeDictionary()
        {
            return new Dictionary<IRequestMessageViewModel, RequestMessageViewModelEditScope>();
        }

        private static Dictionary<IRequestMessageViewModel, RequestMessageViewModelEditScope> MessagesInEditMode
        {
            get { return _MessagesInEditMode.Value; }
        }

        internal static bool IsInEditMode(IRequestMessageViewModel message)
        {
            return MessagesInEditMode.ContainsKey(message);
        }

        internal static object GetEditScopeState(IRequestMessageViewModel message)
        {
            RequestMessageViewModelEditScope editScope;

            if (MessagesInEditMode.TryGetValue(message, out editScope))
            {
                return editScope.State;
            }
            return null;
        }

        internal static bool IsValidationSuppressed(IRequestMessageViewModel message)
        {
            RequestMessageViewModelEditScope editScope;

            if (MessagesInEditMode.TryGetValue(message, out editScope))
            {
                return editScope.SuppressesValidation();
            }
            return false;
        }

        internal static RequestMessageViewModelEditScope BeginEdit(IRequestMessageViewModel message)
        {
            return BeginEdit(message, false, null, false);
        }        

        internal static RequestMessageViewModelEditScope BeginEdit(IRequestMessageViewModel message, bool suppressValidation, object state, bool createNewScope)
        {
            RequestMessageViewModelEditScope editScope;

            if (MessagesInEditMode.TryGetValue(message, out editScope))
            {
                if (createNewScope)
                {
                    MessagesInEditMode[message] = editScope = editScope.CreateNestedScope(suppressValidation, state);
                }
            }
            else
            {
                MessagesInEditMode.Add(message, editScope = new RequestMessageViewModelEditScope(message, message.Copy(true), suppressValidation, state));
            }
            return editScope;
        }

        internal static void CancelEdit(IRequestMessageViewModel message)
        {
            RequestMessageViewModelEditScope editScope;

            if (MessagesInEditMode.TryGetValue(message, out editScope))
            {
                editScope.RestoreBackup();
                editScope.Dispose();

                EndScope(editScope);
            }
        }

        internal static void EndEdit(IRequestMessageViewModel message)
        {
            RequestMessageViewModelEditScope editScope;

            if (MessagesInEditMode.TryGetValue(message, out editScope))
            {                
                editScope.Dispose();

                EndScope(editScope);
            }
        }

        private static void EndScope(RequestMessageViewModelEditScope editScope)
        {
            if (editScope.ParentScope == null)
            {
                MessagesInEditMode.Remove(editScope.Message);
            }
            else
            {
                MessagesInEditMode[editScope.Message] = editScope.ParentScope;
            }
        }

        #endregion
    }
}
