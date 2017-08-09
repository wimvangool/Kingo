using System.Threading.Tasks;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class AcceptChallengeHandler : IMessageHandler<AcceptChallengeCommand>
    {
        private readonly IChallengeRepository _challenges;

        public AcceptChallengeHandler(IChallengeRepository challenges)
        {            
            _challenges = challenges;
        }

        public async Task HandleAsync(AcceptChallengeCommand message, IMicroProcessorContext context)
        {            
            var challenge = await _challenges.GetByIdAsync(message.ChallengeId);

            challenge.Accept();
        }
    }
}
