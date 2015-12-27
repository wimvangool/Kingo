using Clients.ConsoleApp.States;

namespace Clients.ConsoleApp.Commandlets
{
    internal sealed class ExitApplicationCommand : Commandlet
    {
        private readonly ChessApplication _application;

        internal ExitApplicationCommand(ChessApplication application)
            : base("Exit", "Exit the application.")
        {
            _application = application;
        }

        public override void Execute(string[] args)
        {
            if (args.Length > 1)
            {
                throw new UnknownCommandArgumentException(args[1]);
            }
            Execute();
        }

        internal void Execute()
        {
            _application.SwitchTo(new StoppedState(_application));                
        }
    }
}
