using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Challenges;

namespace Kingo.Samples.Chess.Games
{
    public sealed class ChallengeAcceptedHandler : IMessageHandler<ChallengeAcceptedEvent>
    {
        private readonly IChallengeRepository _challenges;
        private readonly IGameRepository _games;

        public ChallengeAcceptedHandler(IChallengeRepository challenges, IGameRepository games)
        {            
            _challenges = challenges;
            _games = games;
        }

        public async Task HandleAsync(ChallengeAcceptedEvent message, IMicroProcessorContext context)
        {
            var challenge = await _challenges.GetByIdAsync(message.ChallengeId);
            var game = challenge.StartGame();

            _games.AddAsync(game);
        }
    }
}
