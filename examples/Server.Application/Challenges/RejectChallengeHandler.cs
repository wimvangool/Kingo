using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using PostSharp.Patterns.Contracts;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class RejectChallengeHandler : MessageHandler<RejectChallengeCommand>
    {
        private readonly IChallengeRepository _challenges;

        public RejectChallengeHandler([NotNull] IChallengeRepository challenges)
        {            
            _challenges = challenges;
        }

        public override async Task HandleAsync([NotNull] RejectChallengeCommand message)
        {            
            var challenge = await _challenges.GetByIdAsync(message.ChallengeId);

            challenge.Reject();
        }
    }
}
