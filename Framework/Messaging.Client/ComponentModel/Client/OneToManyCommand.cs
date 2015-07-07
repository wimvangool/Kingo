using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Syztem.ComponentModel.Client
{
    /// <summary>
    /// Represents a command that can have many active connections at once.
    /// </summary>
    public sealed class OneToManyCommand : ICompositeCommand
    {
        private readonly List<ICommand> _activeCommands;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneToManyCommand" /> class.
        /// </summary>
        public OneToManyCommand()
        {
            _activeCommands = new List<ICommand>();
        }

        /// <inheritdoc />
        public IConnection Connect(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            return new OneToManyCommandConnection(this, command);
        }

        internal void OpenConnection(ICommand command)
        {            
            _activeCommands.Add(command);

            command.CanExecuteChanged += OnCanExecuteChanged;

            OnCanExecuteChanged(this, EventArgs.Empty);
        }

        internal void CloseConnection(ICommand command)
        {
            if (_activeCommands.Remove(command))
            {
                command.CanExecuteChanged -= OnCanExecuteChanged;
            }
            OnCanExecuteChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// If any connection is active, returns whether any actively connected command can be executed with
        /// the specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">A parameter.</param>
        /// <returns>
        /// <c>true</c> if this command has an active connections with another command and any active command
        /// can be executed with the specified <paramref name="parameter"/>; otherwise <c>false</c>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return _activeCommands.Any(command => command.CanExecute(parameter));
        }

        /// <summary>
        /// Occurs when <see cref="CanExecute(object)" /> must be reconsulted.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged.Raise(this, e);
        }

        /// <summary>
        /// Executes all connected commands that have an active connection.
        /// </summary>
        /// <param name="parameter">A parameter.</param>
        public void Execute(object parameter)
        {
            foreach (var command in _activeCommands)
            {
                command.Execute(parameter);
            }
        }
    }
}
