using System;
using System.Collections.Generic;
using System.Linq;
using Clients.ConsoleApp.Commandlets;
using Kingo.Samples.Chess;

namespace Clients.ConsoleApp.States
{
    internal sealed class LoggedInState : ChessApplicationState
    {        
        private readonly LogoutPlayerCommandlet _logoutPlayerCommand;
        private readonly ChallengePlayerCommandlet _challengePlayerCommand;
        private readonly GetPlayerCommandlet _getPlayerCommand;
        private readonly GetChallengeCommandlet _getChallengeCommandlet;
        private readonly GetGameCommandlet _getGameCommandlet;

        internal LoggedInState(ChessApplication application, IDisposable session)
            : base(application)
        {            
            _logoutPlayerCommand = new LogoutPlayerCommandlet(application, session);
            _challengePlayerCommand = new ChallengePlayerCommandlet();
            _getPlayerCommand = new GetPlayerCommandlet();
            _getChallengeCommandlet = new GetChallengeCommandlet();
            _getGameCommandlet = new GetGameCommandlet(application, session);
        }

        protected override string CommandPrompt()
        {
            return Session.Current.PlayerName;
        }

        protected internal override IEnumerable<Commandlet> SupportedCommandLets()
        {
            var baseCommands = base.SupportedCommandLets();
            var thisCommands = new Commandlet[]
            {
                _logoutPlayerCommand,
                _challengePlayerCommand,
                _getPlayerCommand,
                _getChallengeCommandlet,
                _getGameCommandlet
            };
            return thisCommands.Concat(baseCommands);
        }
    }
}
