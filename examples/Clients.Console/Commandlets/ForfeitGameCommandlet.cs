using System;
using System.Collections.Generic;
using Clients.ConsoleApp.Proxies;
using Kingo.Samples.Chess.Games;
using Kingo.Threading;

namespace Clients.ConsoleApp.Commandlets
{
    internal sealed class ForfeitGameCommandlet : Commandlet
    {
        private readonly GameServiceProxy _gameService;
        private readonly ActiveGame _game;

        internal ForfeitGameCommandlet(ActiveGame game)
            : base("Forfeit-Game", "Forfeit the current game.")
        {
            _gameService = new GameServiceProxy();
            _game = game;
        }

        internal override void Execute(IReadOnlyList<string> arguments)
        {
            Execute();
        }

        internal void Execute()
        {
            _gameService.ForfeitGameAsync(new ForfeitGameCommand(_game.GameId)).Await();

            using (ChessApplication.UseColor(ConsoleColor.Green))
            {
                Console.WriteLine("You forfeited the game.");
            }
        }
    }
}
