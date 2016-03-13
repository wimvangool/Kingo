using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using PostSharp.Patterns.Contracts;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class AcceptChallengeHandler : MessageHandler<AcceptChallengeCommand>
    {
        private readonly IChallengeRepository _challenges;

        public AcceptChallengeHandler([NotNull] IChallengeRepository challenges)
        {            
            _challenges = challenges;
        }

        public override async Task HandleAsync([NotNull] AcceptChallengeCommand message)
        {            
            var challenge = await _challenges.GetByIdAsync(message.ChallengeId);

            challenge.Accept();
        }
    }
}
