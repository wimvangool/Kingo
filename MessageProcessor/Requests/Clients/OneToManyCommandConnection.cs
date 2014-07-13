using IInputCommand = System.Windows.Input.ICommand;

namespace YellowFlare.MessageProcessing.Requests.Clients
{
    internal sealed class OneToManyCommandConnection : Connection
    {
        private readonly OneToManyCommand _command;
        private readonly IInputCommand _inputCommand;
        private bool _isOpen;

        public OneToManyCommandConnection(OneToManyCommand command, IInputCommand inputCommand)
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
