using System;
using System.Threading.Tasks;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class AcceptChallengeHandler : MessageHandler<AcceptChallengeCommand>
    {
        private readonly IChallengeRepository _challenges;

        public AcceptChallengeHandler(IChallengeRepository challenges)
        {
            if (challenges == null)
            {
                throw new ArgumentNullException("challenges");
            }
            _challenges = challenges;
        }

        public override async Task HandleAsync(AcceptChallengeCommand message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var challenge = await _challenges.GetByIdAsync(message.ChallengeId);

            challenge.Accept();
        }
    }
}
