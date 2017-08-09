using System.Threading.Tasks;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class RejectChallengeHandler : IMessageHandler<RejectChallengeCommand>
    {
        private readonly IChallengeRepository _challenges;

        public RejectChallengeHandler(IChallengeRepository challenges)
        {            
            _challenges = challenges;
        }

        public async Task HandleAsync(RejectChallengeCommand message, IMicroProcessorContext context)
        {            
            var challenge = await _challenges.GetByIdAsync(message.ChallengeId);

            challenge.Reject();
        }
    }
}
