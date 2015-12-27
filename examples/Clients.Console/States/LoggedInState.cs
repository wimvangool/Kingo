using System;
using System.Collections.Generic;
using System.Linq;
using Clients.ConsoleApp.Commandlets;

namespace Clients.ConsoleApp.States
{
    internal sealed class LoggedInState : ChessApplicationState
    {        
        private readonly LogoutPlayerCommandlet _logoutPlayerCommand;
        private readonly ChallengePlayerCommandlet _challengePlayerCommand;
        private readonly GetPlayerCommandlet _getPlayerCommand;

        internal LoggedInState(ChessApplication application, IDisposable session)
            : base(application)
        {            
            _logoutPlayerCommand = new LogoutPlayerCommandlet(application, session);
            _challengePlayerCommand = new ChallengePlayerCommandlet();
            _getPlayerCommand = new GetPlayerCommandlet();
        }        

        protected internal override IEnumerable<Commandlet> SupportedCommandLets()
        {
            var baseCommands = base.SupportedCommandLets();
            var thisCommands = new Commandlet[]
            {
                _logoutPlayerCommand,
                _challengePlayerCommand,
                _getPlayerCommand
            };
            return thisCommands.Concat(baseCommands);
        }
    }
}
