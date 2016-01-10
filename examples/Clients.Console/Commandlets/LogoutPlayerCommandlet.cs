using System;
using System.Collections.Generic;
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

        internal override void Execute(IReadOnlyList<string> arguments)
        {
            Execute();
        }

        internal void Execute()
        {
            _session.Dispose();
            _application.SwitchTo(new StartedState(_application));
        }
    }
}
