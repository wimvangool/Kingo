using System;
using Clients.ConsoleApp.States;

namespace Clients.ConsoleApp.Commandlets
{
    internal sealed class LogoutPlayerCommandlet : Commandlet
    {
        private readonly ChessApplication _application;
        private readonly IDisposable _session;

        internal LogoutPlayerCommandlet(ChessApplication application, IDisposable session)
            : base("Logout-Player", "End the session of the current player.")
        {
            _application = application;
            _session = session;
        }

        public override void Execute(string[] args)
        {
            if (args.Length > 1)
            {
                throw new UnknownCommandArgumentException(args[1]);
            }
            Execute();
        }

        private void Execute()
        {
            _session.Dispose();
            _application.SwitchTo(new StartedState(_application));
        }
    }
}
