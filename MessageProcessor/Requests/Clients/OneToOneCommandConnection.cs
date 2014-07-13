using IInputCommand = System.Windows.Input.ICommand;

namespace YellowFlare.MessageProcessing.Requests.Clients
{
    internal sealed class OneToOneCommandConnection : Connection
    {
        private readonly OneToOneCommand _command;
        private readonly IInputCommand _inputCommand;

        public OneToOneCommandConnection(OneToOneCommand command, IInputCommand inputCommand)
        {
            _command = command;
            _inputCommand = inputCommand;
        }

        protected override bool IsOpen
        {
            get { return _command.IsConnectedTo(_inputCommand); }
        }

        protected override void OpenConnection()
        {
            _command.OpenConnection(_inputCommand);
        }

        protected override void CloseConnection()
        {
            _command.CloseConnection();
        }
    }
}
