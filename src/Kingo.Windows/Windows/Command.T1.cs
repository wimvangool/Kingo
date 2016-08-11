using System;
using System.Windows.Input;

namespace Kingo.Windows
{
    /// <summary>
    /// Serves as a base-class implementation for strong-typed <see cref="ICommand" />-implementations.
    /// </summary>
    /// <typeparam name="TParameter">Type of the parameter accepted by this command.</typeparam>
    public abstract class Command<TParameter> : ICommand
    {
        #region [====== CanExecuteChanged ======]

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;    
        
        /// <summary>
        /// Raised the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged.Raise(this);
        }

        #endregion

        #region [====== CanExecute ======]
        
        bool ICommand.CanExecute(object parameter)
        {
            TParameter convertedParameter;

            if (TryConvertParameter(parameter, out convertedParameter))
            {
                return CanExecuteCommand(convertedParameter);
            }
            return false;
        }

        /// <summary>
        /// Indicates whether or not the command can be executed with the specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>
        /// <c>true</c> if this command can be executed with the specified <paramref name="parameter"/>; otherwise <c>false</c>.
        /// </returns>
        public bool CanExecute(TParameter parameter)
        {
            return CanExecuteCommand(parameter);
        }

        /// <summary>
        /// Indicates whether or not the command can be executed with the specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>
        /// <c>true</c> if this command can be executed with the specified <paramref name="parameter"/>; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool CanExecuteCommand(TParameter parameter)
        {
            return true;
        }

        #endregion

        #region [====== Execute ======]
        
        void ICommand.Execute(object parameter)
        {
            TParameter convertedParameter;

            if (TryConvertParameter(parameter, out convertedParameter))
            {
                Execute(convertedParameter);              
            }            
        }

        /// <summary>
        /// Executes this command with the specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The parameter of this command.</param>
        /// <exception cref="InvalidOperationException">
        /// The command could not be executed with the specified <paramref name="parameter"/>.
        /// </exception>
        public void Execute(TParameter parameter)
        {
            if (CanExecuteCommand(parameter))
            {
                ExecuteCommand(parameter);                
            }            
        }

        /// <summary>
        /// Executes this command with the specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The parameter of this command.</param>
        protected abstract void ExecuteCommand(TParameter parameter);        

        #endregion

        /// <summary>
        /// Attempts to convert the specified <paramref name="parameter"/> into an instance of <typeparamref name="TParameter"/>.
        /// </summary>
        /// <param name="parameter">A parameter value.</param>
        /// <param name="convertedParameter">
        /// If conversion succeeds, this parameter will refer to the converted value; otherwise, it will have the default value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the conversion succeeds; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool TryConvertParameter(object parameter, out TParameter convertedParameter)
        {
            try
            {
                convertedParameter = (TParameter) parameter;
                return true;
            }
            catch (NullReferenceException)
            {
                convertedParameter = default(TParameter);
                return false;
            }
            catch (InvalidCastException)
            {
                convertedParameter = default(TParameter);
                return false;
            }
        }
    }
}
