using System;
using System.Collections.Generic;
using System.Linq;
using Clients.ConsoleApp.Commandlets;

namespace Clients.ConsoleApp.States
{
    internal sealed class StartedState : ChessApplicationState
    {                
        private readonly RegisterPlayerCommandlet _registerPlayerCommand;
        private readonly LoginPlayerCommandlet _loginPlayerCommand;
        private readonly GetPlayerCommandlet _getPlayerCommand;

        internal StartedState(ChessApplication application)
            : base(application)
        {            
            _registerPlayerCommand = new RegisterPlayerCommandlet();
            _loginPlayerCommand = new LoginPlayerCommandlet(application);
            _getPlayerCommand = new GetPlayerCommandlet();
        }        

        public override void OnEntering()
        {
            ShowWelcome();
            
            base.OnEntering();
        }

        private static void ShowWelcome()
        {
            Console.WriteLine("Welcome to Console Chess!");
        }

        protected internal override IEnumerable<Commandlet> SupportedCommandLets()
        {
            var baseCommands = base.SupportedCommandLets();
            var thisCommands = new Commandlet[]
            {
                _registerPlayerCommand,
                _loginPlayerCommand,
                _getPlayerCommand
            };
            return thisCommands.Concat(baseCommands);
        }                        
    }
}
