using System;
using System.Collections.Generic;
using Clients.ConsoleApp.States;

namespace Clients.ConsoleApp.Commandlets
{
    internal sealed class ExitGameCommandlet : Commandlet
    {
        private readonly ChessApplication _application;
        private readonly IDisposable _session;

        internal ExitGameCommandlet(ChessApplication application, IDisposable session)
            : base("Exit-Game", "Exit the current game.")
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
            _application.SwitchTo(new LoggedInState(_application, _session));
        }
    }
}
