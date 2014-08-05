using System.Collections.Generic;
using System.Windows.Input;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="IRequestDispatcherCommand" /> interface.
    /// </summary>       
    /// <typeparam name="TParameter">Type of the parameter that can be specified for executing this request.</typeparam>
    public class AsyncCommand<TParameter> : PropertyChangedBase, IRequestDispatcherCommand       
    {
        private readonly IRequestDispatcher _dispatcher;
        private readonly IIsValidIndicator _isValidIndicator;        
        private readonly AsyncCommandOptions _options;
        private readonly List<IAsyncExecutionTask> _runningTasks;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{T}" /> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>             
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public AsyncCommand(IRequestDispatcher dispatcher)
            : this(dispatcher, null, AsyncCommandOptions.None) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{T}" /> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>
        /// <param name="isValidIndicator">
        /// The indicator that is used to determine whether the command can be executed.
        /// </param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public AsyncCommand(IRequestDispatcher dispatcher, IIsValidIndicator isValidIndicator)
            : this(dispatcher, isValidIndicator, AsyncCommandOptions.None) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{T}" /> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>
        /// <param name="isValidIndicator">
        /// The indicator that is used to determine whether the command can be executed.
        /// </param>
        /// <param name="options">
        /// The opions that determine the exact behavior of this command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public AsyncCommand(IRequestDispatcher dispatcher, IIsValidIndicator isValidIndicator, AsyncCommandOptions options)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            _dispatcher = dispatcher;
            _dispatcher.IsExecutingChanged += (s, e) => OnIsExecutingChanged();
            _isValidIndicator = isValidIndicator ?? new NullMessage(true, true);
            _isValidIndicator.IsValidChanged += (s, e) => OnIsValidChanged();           
            _options = options;
            _runningTasks = new List<IAsyncExecutionTask>();
        }

        /// <summary>
        /// Returns the encapsulated dispatcher.
        /// </summary>
        protected IRequestDispatcher Dispatcher
        {
            get { return _dispatcher; }
        }

        /// <summary>
        /// Returns the indicator that indicates whether or not the associated request is valid to execute.
        /// </summary>
        protected IIsValidIndicator IsValidIndicator
        {
            get { return _isValidIndicator; }
        }       

        /// <summary>
        /// Indicates whether or not the request is allowed to have parrallel executions.
        /// </summary>
        protected bool AllowParrallelExecutions
        {
            get { return IsSet(AsyncCommandOptions.AllowParrallelExecutions); }
        }

        /// <summary>
        /// Indicates whether or not the encapsulated <see cref="Dispatcher" /> should be
        /// executed synchronously.
        /// </summary>
        protected bool CancelPreviousOnExecution
        {
            get { return IsSet(AsyncCommandOptions.CancelPreviousOnExecution); }
        }

        private bool IsSet(AsyncCommandOptions options)
        {
            return (_options & options) == options;
        }

        #region [====== ICommand - CanExecute ======]

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged.Raise(this);
        }

        bool ICommand.CanExecute(object parameter)
        {
            TParameter parameterOut;

            if (TryConvertParameter(parameter, out parameterOut))
            {
                return CanExecute(parameterOut);
            }
            return false;
        }

        #endregion

        #region [====== ICommand - Execute ======]

        /// <summary>
        /// Occurs when this command is executed and a new <see cref="IAsyncExecutionTask" /> is started.
        /// </summary>
        public event EventHandler<AsyncExecutionTaskStartedEventArgs> TaskStarted;

        /// <summary>
        /// Raises the <see cref="TaskStarted" /> event.
        /// </summary>
        /// <param name="task">The task that has been started.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="task"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnTaskStarted(IAsyncExecutionTask task)
        {
            TaskStarted.Raise(this, new AsyncExecutionTaskStartedEventArgs(task));
            
            OnIsExecutingChanged();
        }

        void ICommand.Execute(object parameter)
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
            catch (NullReferenceException)
            {
                parameterOut = default(TParameter);
                return false;
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
        /// <see cref="IsValidIndicator" /> is valid and the request is not already executing or <see cref="AllowParrallelExecutions" />
        /// is <c>true</c>.
        /// </remarks>
        public virtual bool CanExecute(TParameter parameter)
	    {
		    return IsValidIndicator.IsValid && (AllowParrallelExecutions || !IsExecuting);
	    }

        /// <summary>
        /// Executes this request using the specified <paramref name="parameter"/>, if possible.
        /// </summary>
        /// <param name="parameter">The parameter used to execute this request.</param>
        public void Execute(TParameter parameter)
        {
            if (CanExecute(parameter))
            {
                Execute(CreateExecutionTask(parameter));
            }                       
        }  

        private void Execute(IAsyncExecutionTask task)
        {
            if (CancelPreviousOnExecution)
            {
                foreach (var runningTask in _runningTasks)
                {
                    runningTask.Cancel();
                }
            }

            // When a task has ended (canceled, failed or done), it should be removed again.
            task.StatusChanged += (s, e) =>
            {
                if (task.Status == AsyncExecutionTaskStatus.Running)
                {
                    return;
                }
                Finish(task);
            };
            task.Execute();

            _runningTasks.Add(task);

            OnTaskStarted(task);
        }

        private void Finish(IAsyncExecutionTask task)
        {
            if (_runningTasks.Remove(task))
            {
                OnIsExecutingChanged();
            }
        }
        
        /// <summary>
        /// Creates and returns a new <see cref="IAsyncExecutionTask" /> that will be executed.
        /// </summary>
        /// <param name="parameter">
        /// Parameter that can be used to initialize the request.
        /// </param>
        /// <returns>A new <see cref="IAsyncExecutionTask" />.</returns>
        protected virtual IAsyncExecutionTask CreateExecutionTask(TParameter parameter)
        {
            return Dispatcher.CreateAsyncExecutionTask();
        }

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
        /// Raises the <see cref="IsExecutingChanged" />, <see cref="PropertyChangedBase.PropertyChanged" /> and
        /// possibly the <see cref="CanExecuteChanged" /> events, depending on whether or not <see cref="AllowParrallelExecutions"/>
        /// is <c>true</c>.
        /// </summary>
        protected virtual void OnIsExecutingChanged()
        {
            IsExecutingChanged.Raise(this);

            OnPropertyChanged(() => IsExecuting);
            OnPropertyChanged("IsBusy");

            if (AllowParrallelExecutions)
            {
                return;
            }
            OnCanExecuteChanged();
        }

        /// <inheritdoc />
        public bool IsExecuting
        {
            get { return _runningTasks.Count > 0; }
        }

        #endregion
    }
}
