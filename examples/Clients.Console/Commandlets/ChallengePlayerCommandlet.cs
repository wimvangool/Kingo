using System;
using Clients.ConsoleApp.Proxies;
using Kingo.Samples.Chess.Challenges;
using Kingo.Threading;

namespace Clients.ConsoleApp.Commandlets
{
    internal sealed class ChallengePlayerCommandlet : Commandlet
    {
        private const string _PlayerArgument = "<player>";
        private readonly ChallengeServiceProxy _challengeService;

        internal ChallengePlayerCommandlet()
            : base("Challenge-Player", "Challenge another player to play a game of chess.")
        {
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
            _challengeService.ChallengePlayerAsync(new ChallengePlayerCommand(Guid.NewGuid(), playerName)).Await();
        }
    }
}
