using System;
using System.Windows.Input;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing.Messages.Clients
{
    /// <summary>
    /// Represents a command that can have at most one active connection at a time.
    /// </summary>
    public sealed class OneToOneCommand : ICompositeCommand
    {        
        private ICommand _activeCommand;

        /// <inheritdoc />
        public IConnection Connect(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            return new OneToOneCommandConnection(this, command);
        }                

        internal bool IsConnectedTo(ICommand command)
        {
            return _activeCommand != null && _activeCommand == command;
        }

        internal void OpenConnection(ICommand command)
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
