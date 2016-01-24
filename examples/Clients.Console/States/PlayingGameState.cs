using System;
using System.Collections.Generic;
using System.Linq;
using Clients.ConsoleApp.Commandlets;
using Kingo.Samples.Chess;
using Kingo.Samples.Chess.Games;

namespace Clients.ConsoleApp.States
{
    internal sealed class PlayingGameState : ChessApplicationState
    {
        private readonly ForfeitGameCommandlet _forfeitGameCommandlet;
        private readonly ExitGameCommandlet _exitGameCommandlet;        
        private readonly ActiveGame _game;

        internal PlayingGameState(ChessApplication application, IDisposable session, ActiveGame game)
            : base(application)
        {
            _forfeitGameCommandlet = new ForfeitGameCommandlet(game);
            _exitGameCommandlet = new ExitGameCommandlet(application, session);            
            _game = game;
        }

        protected override string CommandPrompt()
        {
            const string commandPromptFormat = "{0} ({1}) {2} ({3})";
            const char white = 'W';
            const char black = 'B';

            var isWhite = Session.Current.PlayerId.Equals(_game.WhitePlayer.Id);
            if (isWhite)
            {
                return string.Format(commandPromptFormat, _game.WhitePlayer.Name, white, _game.BlackPlayer.Name, black);
            }
            return string.Format(commandPromptFormat, _game.BlackPlayer.Name, black, _game.WhitePlayer.Name, white);
        }

        protected internal override IEnumerable<Commandlet> SupportedCommandLets()
        {
            var baseCommands = base.SupportedCommandLets();
            var thisCommands = new Commandlet[]
            {
                _forfeitGameCommandlet,
                _exitGameCommandlet
            };
            return thisCommands.Concat(baseCommands);
        }
    }
}
