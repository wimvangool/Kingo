using System;

namespace Clients.ConsoleApp.States
{
    internal sealed class StoppedState : ChessApplicationState
    {        
        internal StoppedState(ChessApplication application)
            : base(application) { }

        public override bool IsRunningState
        {
            get { return false; }
        }                

        public override void OnEntering()
        {
            Console.WriteLine("See you next time!");
        }
    }
}
