using System;
using Clients.ConsoleApp.Proxies;
using Kingo.Samples.Chess.Challenges;
using Kingo.Samples.Chess.Players;
using Kingo.Threading;

namespace Clients.ConsoleApp.Commandlets
{
    internal sealed class ChallengePlayerCommandlet : Commandlet
    {
        private const string _PlayerArgument = "<player>";
        private readonly PlayerServiceProxy _playerService;
        private readonly ChallengeServiceProxy _challengeService;

        internal ChallengePlayerCommandlet()
            : base("Challenge-Player", "Challenge another player to play a game of chess.")
        {
            _playerService = new PlayerServiceProxy();
            _challengeService = new ChallengeServiceProxy();
        }

        public override void Execute(string[] args)
        {
            if (args.Length <= 1)
            {
                throw new MissingCommandArgumentException(_PlayerArgument);
            }
            if (args.Length == 2)
            {
                Execute(args[1]);
                return;
            }
            throw new UnknownCommandArgumentException(args[2]);
        }

        internal void Execute(string playerName)
        {
            RegisteredPlayer player;

            if (_playerService.TryGetRegisteredPlayer(playerName, out player))
            {
                _challengeService.ChallengePlayerAsync(new ChallengePlayerCommand(Guid.NewGuid(), player.Id)).Await();
                return;
            }
            using (ChessApplication.UseColor(ConsoleColor.Red))
            {
                Console.WriteLine("Player '{0}' is not registered.", playerName);
            }
        }
    }
}
