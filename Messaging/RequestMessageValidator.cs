﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace System.ComponentModel
{
    internal sealed class RequestMessageValidator : IServiceProvider
    {
        private readonly RequestMessage _requestMessage;
        private readonly Dictionary<Type, object> _services;        
        private RequestMessageErrorInfo _errorInfo;

        internal RequestMessageValidator(RequestMessage requestMessage)
        {
            _requestMessage = requestMessage;
            _services = new Dictionary<Type, object>();           
            _errorInfo = RequestMessageErrorInfo.NotYetValidated;
        }

        internal RequestMessageValidator(RequestMessage requestMessage, RequestMessageValidator validator)
        {
            _requestMessage = requestMessage;
            _services = new Dictionary<Type, object>(validator._services);            
            _errorInfo = validator._errorInfo == null ? null : new RequestMessageErrorInfo(validator._errorInfo);
        }        

        #region [====== DataErrorInfo ======]

        internal RequestMessageErrorInfo ErrorInfo
        {
            get { return _errorInfo; }
            set
            {
                var oldValue = _requestMessage.IsValid;

                _errorInfo = value;

                var newValue = _requestMessage.IsValid;

                if (oldValue != newValue)
                {
                    _requestMessage.OnIsValidChanged();
                }
            }
        }

        #endregion

        #region [====== ServiceProvider ======]

        internal void Add(Type serviceType, object service)
        {
            _services.Add(serviceType, service);
        }

        public object GetService(Type serviceType)
        {
            object service;

            if (_services.TryGetValue(serviceType, out service))
            {
                return service;
            }
            return null;
        }

        #endregion

        #region [====== Validation ======]

        internal bool TryValidateMessage(bool ignoreEditScope, ValidationContext validationContext)
        {
            if (ignoreEditScope || !RequestMessageEditScope.IsValidationSuppressed(_requestMessage))
            {
                ErrorInfo = RequestMessageErrorInfo.CreateErrorInfo(validationContext);
                return true;
            }
            return false;
        }

        #endregion
    }
}
