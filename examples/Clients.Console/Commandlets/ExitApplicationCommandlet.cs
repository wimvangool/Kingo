using System.Collections.Generic;
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

        internal override void Execute(IReadOnlyList<string> arguments)
        {
            Execute();
        }  
     
        internal void Execute()
        {
            _application.SwitchTo(new StoppedState(_application));                
        }
    }
}
