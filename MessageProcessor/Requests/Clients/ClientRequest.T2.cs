using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing.Requests.Clients
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="IClientRequest" /> interface.
    /// </summary>    
    /// <typeparam name="TRequest">Type of the encapsulated request.</typeparam>
    /// <typeparam name="TParameter">Type of the parameter that can be specified for executing this request.</typeparam>
    public abstract class ClientRequest<TRequest, TParameter> : PropertyChangedNotifier, IClientRequest
        where TRequest : class, IRequest
    {
        private readonly TRequest _request;
        private readonly IIsValidIndicator _isValidIndicator;
        private readonly Stack<TParameter> _parameterStack;
        private readonly ClientRequestExecutionOptions _options;

        internal ClientRequest(TRequest request, IIsValidIndicator isValidIndicator, ClientRequestExecutionOptions options)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            _request = request;
            _request.IsExecutingChanged += (s, e) => OnIsExecutingChanged();
            _isValidIndicator = isValidIndicator ?? new NullMessage(true, true);
            _isValidIndicator.IsValidChanged += (s, e) => OnIsValidChanged();
            _parameterStack = new Stack<TParameter>(3);
            _options = options;
        }

        /// <summary>
        /// Returns the encapsulated request.
        /// </summary>
        protected TRequest Request
        {
            get { return _request; }
        }

        /// <summary>
        /// Returns the indicator that indicates whether or not the associated request is valid to execute.
        /// </summary>
        protected IIsValidIndicator IsValidIndicator
        {
            get { return _isValidIndicator; }
        }

        /// <summary>
        /// Contains the parameter that is currently being used for execution of this request.
        /// </summary>
        protected TParameter Parameter
        {
            get { return _parameterStack.Count == 0 ? default(TParameter) : _parameterStack.Peek(); }            
        }

        /// <summary>
        /// Indicates whether or not the request is allowed to have nested (in case of synchronous) or
        /// parrallel (in case of asynchronous) executions.
        /// </summary>
        protected bool AllowMultipleExecutions
        {
            get { return IsSet(ClientRequestExecutionOptions.AllowMultipleExecutions); }
        }

        /// <summary>
        /// Indicates whether or not the encapsulated <see cref="Request" /> should be
        /// executed synchronously.
        /// </summary>
        protected bool ExecuteSynchronously
        {
            get { return IsSet(ClientRequestExecutionOptions.ExecuteSynchronously); }
        }

        private bool IsSet(ClientRequestExecutionOptions options)
        {
            return (_options & options) == options;
        }

        #region [====== ICommand ======]

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged.Raise(this);
        }

        bool System.Windows.Input.ICommand.CanExecute(object parameter)
        {
            TParameter parameterOut;

            if (TryConvertParameter(parameter, out parameterOut))
            {
                return CanExecute(parameterOut);
            }
            return false;
        }

        void System.Windows.Input.ICommand.Execute(object parameter)
        {
            TParameter parameterOut;

            if (TryConvertParameter(parameter, out parameterOut) && CanExecute(parameterOut))
            {
                Execute(parameterOut);
            }
        }

        /// <summary>
        /// Attempts to convert the specified <paramref name="parameterIn"/> to type <typeparamref name="TParameter"/>.       
        /// </summary>
        /// <param name="parameterIn">The incoming parameter.</param>
        /// <param name="parameterOut">
        /// If conversion succeeded, contains the converted value of <paramref name="parameterIn"/>. If the
        /// conversion fails, this parameter will be set to the default value of <typeparamref name="TParameter"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if conversion was successful; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The default implementation simply attempts to cast <paramref name="parameterIn"/> to
        /// an instance of <typeparamref name="TParameter"/>. If another conversion is required, this
        /// method may be overridden.
        /// </remarks>
        protected virtual bool TryConvertParameter(object parameterIn, out TParameter parameterOut)
        {
            try
            {
                parameterOut = (TParameter) parameterIn;
                return true;
            }
            catch (InvalidCastException)
            {
                parameterOut = default(TParameter);
                return false;
            }
        }

        /// <summary>
        /// Indicates whether or not this request can be executed with the specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to use for executing this request.
        /// </param>
        /// <returns>
        /// <c>true</c> if this request can be executed with <paramref name="parameter"/>; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The default implementation will ignore <paramref name="parameter"/> and return <c>true</c> if and only if
        /// <see cref="IsValidIndicator" /> is valid and the request is not already executing or <see cref="AllowMultipleExecutions" />
        /// is <c>true</c>.
        /// </remarks>
        public virtual bool CanExecute(TParameter parameter)
	    {
		    return IsValidIndicator.IsValid && (AllowMultipleExecutions || !IsExecuting);
	    }

        /// <summary>
        /// Executes this request using the specified <paramref name="parameter"/>, if possible.
        /// </summary>
        /// <param name="parameter">The parameter used to execute this request.</param>
        public void Execute(TParameter parameter)
        {
            if (CanExecute(parameter))
            {
                _parameterStack.Push(parameter);

                try
                {
                    ExecuteRequest();
                }
                finally
                {
                    _parameterStack.Pop();
                }
            }                       
        }

        private void ExecuteRequest()
        {
            if (ExecuteSynchronously)
            {
                Execute();
            }
            else
            {
                ExecuteAsync();
            } 
        }

        /// <summary>
        /// Executes the associated <see cref="Request" /> synchronously..
        /// </summary>
        protected abstract void Execute();

        /// <summary>
        /// Executes the associtaed <see cref="Request" /> asynchronously.
        /// </summary>
        protected abstract void ExecuteAsync();

        #endregion

        #region [====== IsValidIndicator ======]

        event EventHandler IIsValidIndicator.IsValidChanged
        {
            add { IsValidIndicator.IsValidChanged += value; }
            remove { IsValidIndicator.IsValidChanged -= value; }
        }

        bool IIsValidIndicator.IsValid
        {
            get { return IsValidIndicator.IsValid; }
        }

        /// <summary>
        /// Executes as soon as <see cref="IIsValidIndicator.IsValidChanged" /> is raised.
        /// </summary>
        protected virtual void OnIsValidChanged()
        {
            OnPropertyChanged("IsValid");
            OnCanExecuteChanged();
        }

        #endregion

        #region [====== IsBusyIndicator ======]

        event EventHandler IIsBusyIndicator.IsBusyChanged
        {
            add { IsExecutingChanged += value; }
            remove { IsExecutingChanged -= value; }
        }

        bool IIsBusyIndicator.IsBusy
        {
            get { return IsExecuting; }
        }

        #endregion

        #region [====== IsExecuting ======]

        /// <inheritdoc />
        public event EventHandler IsExecutingChanged;

        /// <summary>
        /// Raises the <see cref="IsExecutingChanged" />, <see cref="PropertyChangedNotifier.PropertyChanged" /> and
        /// possibly the <see cref="CanExecuteChanged" /> events, depending on whether or not <see cref="AllowMultipleExecutions"/>
        /// is <c>true</c>.
        /// </summary>
        protected virtual void OnIsExecutingChanged()
        {
            IsExecutingChanged.Raise(this);

            OnPropertyChanged(() => IsExecuting);
            OnPropertyChanged("IsBusy");

            if (AllowMultipleExecutions)
            {
                return;
            }
            OnCanExecuteChanged();
        }

        /// <inheritdoc />
        public bool IsExecuting
        {
            get { return Request.IsExecuting; }
        }

        #endregion
    }
}
