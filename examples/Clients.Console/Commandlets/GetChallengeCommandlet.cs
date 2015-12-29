using System;
using System.Linq;
using Clients.ConsoleApp.Proxies;
using Kingo.Samples.Chess.Challenges;
using Kingo.Threading;

namespace Clients.ConsoleApp.Commandlets
{
    internal sealed class GetChallengeCommandlet : Commandlet
    {
        private const char _AcceptChallenge = 'a';
        private const char _DenyChallenge = 'd';
        private const char _SkipChallenge = 's';

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
            char choice;

            do
            {
                Console.Write("You received a challenge from '{0}'. Please accept ({1}), deny ({2}) or skip ({3}).> ",
                    challenge.PlayerName,
                    _AcceptChallenge,
                    _DenyChallenge,
                    _SkipChallenge);
            }
            while (PromptUserForAction(out choice));
            
            if (choice == _AcceptChallenge)
            {
                _challengeService.AcceptChallengeAsync(new AcceptChallengeCommand(challenge.ChallengeId)).Await();

                using (ChessApplication.UseColor(ConsoleColor.Green))
                {
                    Console.WriteLine("Challenge from '{0}' was accepted.", challenge.PlayerName);
                }
            }
        }
        
        private static bool PromptUserForAction(out char choice)
        {
            choice = Console.ReadKey().KeyChar;

            Console.WriteLine();

            return !(choice == _AcceptChallenge || choice == _DenyChallenge || choice == _SkipChallenge);
        }
    }
}
