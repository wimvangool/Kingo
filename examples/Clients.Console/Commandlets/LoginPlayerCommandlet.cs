using System;
using System.Linq;
using Clients.ConsoleApp.Proxies;
using Clients.ConsoleApp.States;
using Kingo.Samples.Chess;
using Kingo.Samples.Chess.Players;

namespace Clients.ConsoleApp.Commandlets
{
    internal sealed class LoginPlayerCommandlet : Commandlet
    {
        private const string _PlayerArgument = "<player>";
        private readonly PlayerServiceProxy _playerService;
        private readonly ChessApplication _application;

        internal LoginPlayerCommandlet(ChessApplication application)
            : base("Login-Player", "Log in as a player.")
        {
            _playerService = new PlayerServiceProxy();
            _application = application;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, _PlayerArgument);
        }

        public override void Execute(string[] args)
        {
            if (args.Length <= 1)
            {
                throw new MissingCommandArgumentException(_PlayerArgument);
            }
            if (args.Length > 2)
            {
                throw new UnknownCommandArgumentException(args[2]);
            }
            Execute(args[1]);
        }

        private void Execute(string enteredName)
        {
            string playerName;

            if (TryGetPlayerName(enteredName, out playerName))
            {
                var session = Session.CreateSession(playerName);
                var loggedInState = new LoggedInState(_application, session);

                _application.SwitchTo(loggedInState);
            }
            else
            {
                using (ChessApplication.UseColor(ConsoleColor.Red))
                {
                    Console.WriteLine("Cannot log in as player '{0}' because this player has not been registered.", enteredName);
                }
            }
        }

        private bool TryGetPlayerName(string enteredName, out string playerName)
        {
            var response = _playerService.GetPlayersAsync(new GetPlayersRequest()).Result;
            var matches =
                from player in response.Players
                where string.Compare(player.Name, enteredName, StringComparison.OrdinalIgnoreCase) == 0
                select player.Name;

            return (playerName = matches.FirstOrDefault()) != null;
        }
    }
}
