using System.Windows.Input;

namespace System.ComponentModel.Client
{
    /// <summary>
    /// Represents a command that delegates it's work to some specified functions.
    /// </summary>
    /// <typeparam name="TParameter">Type of the parameter of this command.</typeparam>
    public class RelayCommand<TParameter> : ICommand
    {
        private readonly Func<TParameter, bool> _canExecute;
        private readonly Action<TParameter> _execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}" /> class.
        /// </summary>
        /// <param name="execute">The function to execute when this command is executed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="execute"/> is <c>null</c>.
        /// </exception>
        public RelayCommand(Action<TParameter> execute) : this(execute, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}" /> class.
        /// </summary>
        /// <param name="execute">The function to execute when this command is executed.</param>
        /// <param name="canExecute">
        /// Optional predicate function that is used to check whether or not this command can be executed
        /// based on a certain parameter. If this parameter is <c>null</c>, this command is assumed to be
        /// able to execute with any parameter of the right type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="execute"/> is <c>null</c>.
        /// </exception>
        public RelayCommand(Action<TParameter> execute, Func<TParameter, bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            _canExecute = canExecute ?? (parameter => true);
            _execute = execute;
        }

        #region [====== CanExecute ======]

        bool ICommand.CanExecute(object parameter)
        {
            TParameter parameterOut;

            if (TryConvert(parameter, out parameterOut))
            {
                return CanExecute(parameterOut);
            }
            return false;
        }

        /// <summary>
        /// Determines whether or not this command can be executed with the specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">A parameter.</param>
        /// <returns>
        /// <c>true</c> if this command can be executed with the specified <paramref name="parameter"/>; otherwise <c>false</c>.
        /// </returns>
        public virtual bool CanExecute(TParameter parameter)
        {
            return _canExecute.Invoke(parameter);
        }

        /// <summary>
        /// Occurs when <see cref="CanExecute(TParameter)" /> must be reconsulted.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        public virtual void NotifyCanExecuteChanged()
        {
            CanExecuteChanged.Raise(this);
        }

        #endregion

        #region [====== Execute ======]

        void ICommand.Execute(object parameter)
        {
            TParameter parameterOut;

            if (TryConvert(parameter, out parameterOut))
            {
                Execute(parameterOut);
            }
        }

        /// <summary>
        /// If possible, executes this command with the specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">A parameter.</param>
        public virtual void Execute(TParameter parameter)
        {
            if (CanExecute(parameter))
            {
                _execute.Invoke(parameter);
            }
        }

        #endregion

        /// <summary>
        /// Attempts to convert the specified <paramref name="parameterIn"/> to a typed version.
        /// </summary>
        /// <param name="parameterIn">The input-parameter.</param>
        /// <param name="parameterOut">The converted parameter.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="parameterIn"/> was succesfully converted to
        /// an instance of <typeparamref name="TParameter"/> and assigned this value to <paramref name="parameterOut"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        protected virtual bool TryConvert(object parameterIn, out TParameter parameterOut)
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
    }
}
