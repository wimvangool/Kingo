using System;
using Clients.ConsoleApp.Proxies;
using Clients.ConsoleApp.States;
using Kingo.Samples.Chess;
using Kingo.Samples.Chess.Games;

namespace Clients.ConsoleApp.Commandlets
{
    internal sealed class GetGameCommandlet : Commandlet
    {
        private const char _EnterGame = 'e';
        private const char _SkipGame = 's';

        private readonly ChessApplication _application;
        private readonly IDisposable _session;
        private readonly GameServiceProxy _gameService;

        internal GetGameCommandlet(ChessApplication application, IDisposable session)
            : base("Get-Game", "Iterate through a list of games you participate in.")
        {
            _application = application;
            _session = session;
            _gameService = new GameServiceProxy();
        }

        public override void Execute(string[] args)
        {
            if (args.Length > 1)
            {
                throw new UnknownCommandArgumentException(args[1]);
            }
            Execute();
        }

        internal void Execute()
        {
            var response = _gameService.GetActiveGames(new GetActiveGamesRequest()).Result;
            if (response.Games.Length == 0)
            {
                using (ChessApplication.UseColor(ConsoleColor.Magenta))
                {
                    Console.WriteLine("There are no games you are currently participating in.");
                }
                return;
            }
            Console.WriteLine("You currently participate in {0} game(s). Please choose which to enter ({1}) or skip ({2})...",
                response.Games.Length,
                _EnterGame,
                _SkipGame);           

            foreach (var game in response.Games)
            {
                string opponentName;

                if (UserWantsToEnter(game, out opponentName))
                {                    
                    Console.WriteLine("\tEntering game against {0}...", opponentName);

                    _application.SwitchTo(new PlayingGameState(_application, _session, game));
                    return;
                }
            }
        }

        private static bool UserWantsToEnter(ActiveGame game, out string opponentName)
        {                      
            opponentName = Session.Current.PlayerId.Equals(game.WhitePlayer.Id)
                ? game.BlackPlayer.Name
                : game.WhitePlayer.Name;

            char choice;

            do
            {
                Console.Write("\tGame against {0}.> ", opponentName);

                choice = Console.ReadKey().KeyChar;

                Console.WriteLine();
            }
            while (choice != _EnterGame && choice != _SkipGame);

            return choice == _EnterGame;
        }
    }
}
