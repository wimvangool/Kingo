using System;
using System.Collections.Generic;
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

        protected override IEnumerable<string> Arguments()
        {
            yield return _PlayerArgument;
        }

        internal override void Execute(IReadOnlyList<string> arguments)
        {
            Execute(arguments[0]);
        }

        internal void Execute(string playerName)
        {
            RegisteredPlayer player;

            if (_playerService.TryGetRegisteredPlayer(playerName, out player))
            {
                _challengeService.ChallengePlayerAsync(new ChallengePlayerCommand(Guid.NewGuid(), player.Id)).Await();

                using (ChessApplication.UseColor(ConsoleColor.Green))
                {
                    Console.WriteLine("Player '{0}' was challenged.", playerName);
                }
                return;
            }
            using (ChessApplication.UseColor(ConsoleColor.Red))
            {
                Console.WriteLine("Player '{0}' is not registered.", playerName);
            }
        }
    }
}
