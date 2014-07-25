using System.Windows.Input;

namespace System.ComponentModel.Messaging.Client
{
    internal sealed class OneToManyCommandConnection : Connection
    {
        private readonly OneToManyCommand _command;
        private readonly ICommand _inputCommand;
        private bool _isOpen;

        public OneToManyCommandConnection(OneToManyCommand command, ICommand inputCommand)
        {
            _command = command;
            _inputCommand = inputCommand;
        }

        protected override bool IsOpen
        {
            get { return _isOpen; }
        }

        protected override void OpenConnection()
        {
            _command.OpenConnection(_inputCommand);
            _isOpen = true;
        }

        protected override void CloseConnection()
        {
            _command.CloseConnection(_inputCommand);
            _isOpen = false;
        }
    }
}
