using System;
using YellowFlare.MessageProcessing.Resources;
using IInputCommand = System.Windows.Input.ICommand;

namespace YellowFlare.MessageProcessing.Requests.Clients
{
    /// <summary>
    /// Represents a command that can have at most one active connection at a time.
    /// </summary>
    public sealed class OneToOneCommand : ICompositeCommand
    {        
        private IInputCommand _activeCommand;

        /// <inheritdoc />
        public IConnection Connect(IInputCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            return new OneToOneCommandConnection(this, command);
        }                

        internal bool IsConnectedTo(IInputCommand command)
        {
            return _activeCommand != null && _activeCommand == command;
        }

        internal void OpenConnection(IInputCommand command)
        {
            if (_activeCommand != null)
            {
                throw NewOtherCommandAlreadyConnectedException();
            }
            _activeCommand = command;
            _activeCommand.CanExecuteChanged += OnCanExecuteChanged;

            OnCanExecuteChanged(this, EventArgs.Empty);
        }

        internal void CloseConnection()
        {
            _activeCommand.CanExecuteChanged -= OnCanExecuteChanged;
            _activeCommand = null;

            OnCanExecuteChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// If any connection is active, returns whether the actively connected command can be executed with
        /// the specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">A parameter.</param>
        /// <returns>
        /// <c>true</c> if this command has an active connection with another command and that command
        /// can be executed with the specified <paramref name="parameter"/>; otherwise <c>false</c>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return _activeCommand != null && _activeCommand.CanExecute(parameter);
        }

        /// <summary>
        /// Occurs when <see cref="CanExecute(object)" /> must be reconsulted.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged.Raise(sender, e);
        }        

        /// <summary>
        /// If any connection is active, executes the command that is pointed to by the active connection.
        /// </summary>
        /// <param name="parameter">A parameter.</param>
        public void Execute(object parameter)
        {
            if (_activeCommand != null)
            {
                _activeCommand.Execute(parameter);
            }
        }

        private static Exception NewOtherCommandAlreadyConnectedException()
        {
            return new InvalidOperationException(ExceptionMessages.OneToOneCommand_OtherAlreadyConnected);
        }
    }
}
