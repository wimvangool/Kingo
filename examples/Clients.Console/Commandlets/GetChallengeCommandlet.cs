using System;
using System.Linq;
using Clients.ConsoleApp.Proxies;
using Kingo.Samples.Chess.Challenges;

namespace Clients.ConsoleApp.Commandlets
{
    internal sealed class GetChallengeCommandlet : Commandlet
    {
        private readonly ChallengeServiceProxy _challengeService;

        internal GetChallengeCommandlet()
            : base("Get-Challenge", "Show all pending challenges that have been received from other players.")
        {
            _challengeService = new ChallengeServiceProxy();
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
            var response = _challengeService.GetPendingChallenges(new GetPendingChallengesRequest()).Result;

            Console.WriteLine();

            foreach (var challenge in response.Challenges.OrderBy(c => c.PlayerName))
            {
                Prompt(challenge);    
            }
            Console.WriteLine();
        }

        private void Prompt(PendingChallenge challenge)
        {
            // TODO: Let user accept, reject or skip challenge.
            Console.WriteLine(challenge.PlayerName);
        }
    }
}
