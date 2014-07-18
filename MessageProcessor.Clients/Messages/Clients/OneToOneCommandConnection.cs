using System.Windows.Input;

namespace YellowFlare.MessageProcessing.Messages.Clients
{
    internal sealed class OneToOneCommandConnection : Connection
    {
        private readonly OneToOneCommand _command;
        private readonly ICommand _inputCommand;

        public OneToOneCommandConnection(OneToOneCommand command, ICommand inputCommand)
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
