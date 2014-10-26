using System;
using System.Collections.Generic;
using System.ComponentModel.Messaging.Resources;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace System.ComponentModel.Messaging
{
    internal sealed class RequestMessageEditScope : ITransactionalScope
    {
        private readonly IRequestMessage _message;
        private readonly IRequestMessage _messageBackup;

        private readonly HashSet<string> _changedProperties;
        private readonly bool _suppressValidation;
        private bool _messageValidationWasFired;
        private bool _hasCompleted;
        private bool _isDisposed;        

        private RequestMessageEditScope(IRequestMessage message, IRequestMessage messageBackup, bool suppressValidation)
        {
            _message = message;
            _message.PropertyChanged += HandleMessagePropertyChanged;
            _messageBackup = messageBackup;
            
            _changedProperties = new HashSet<string>();
            _suppressValidation = suppressValidation;
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

        public void Complete()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(typeof(RequestMessageEditScope).Name);
            }
            if (_hasCompleted)
            {
                throw NewScopeAlreadyCompletedException();
            }
            _changedProperties.Clear();
            _hasCompleted = true;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            _message.PropertyChanged -= HandleMessagePropertyChanged;

            if (_hasCompleted)
            {
                EndEdit(_message);
            }
            else
            {
                CancelEdit(_message);
            }
        }

        internal void RestoreBackup()
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

        private static Exception NewScopeAlreadyCompletedException()
        {
            return new InvalidOperationException(ExceptionMessages.TransactionScope_ScopeAlreadyCompleted);
        }

        #region [====== BeginEdit, CancelEdit & EndEdit ======]

        private static readonly ThreadLocal<Dictionary<IRequestMessage, RequestMessageEditScope>> _MessagesInEditMode;

        static RequestMessageEditScope()
        {
            _MessagesInEditMode = new ThreadLocal<Dictionary<IRequestMessage, RequestMessageEditScope>>(CreateScopeDictionary);
        }   
     
        private static Dictionary<IRequestMessage, RequestMessageEditScope> CreateScopeDictionary()
        {
            return new Dictionary<IRequestMessage, RequestMessageEditScope>();
        }

        private static Dictionary<IRequestMessage, RequestMessageEditScope> MessagesInEditMode
        {
            get { return _MessagesInEditMode.Value; }
        }

        internal static bool IsInEditMode(RequestMessage requestMessage)
        {
            return MessagesInEditMode.ContainsKey(requestMessage);
        }

        internal static RequestMessageEditScope BeginEdit(IRequestMessage message)
        {
            return BeginEdit(message, false);
        }        

        internal static RequestMessageEditScope BeginEdit(IRequestMessage message, bool createNewScope)
        {
            RequestMessageEditScope editScope;

            if (MessagesInEditMode.TryGetValue(message, out editScope))
            {
                if (createNewScope)
                {
                    // TODO...
                }
            }
            else
            {
                MessagesInEditMode.Add(message, editScope = new RequestMessageEditScope(message, message.Copy(true), createNewScope));
            }
            return editScope;
        }

        internal static void CancelEdit(IRequestMessage message)
        {
            RequestMessageEditScope editScope;

            if (MessagesInEditMode.TryGetValue(message, out editScope))
            {
                editScope.RestoreBackup();
                editScope.Dispose();

                MessagesInEditMode.Remove(message);
            }
        }

        internal static void EndEdit(IRequestMessage message)
        {
            RequestMessageEditScope editScope;

            if (MessagesInEditMode.TryGetValue(message, out editScope))
            {                
                editScope.Dispose();

                MessagesInEditMode.Remove(message);
            }
        }

        #endregion
    }
}
