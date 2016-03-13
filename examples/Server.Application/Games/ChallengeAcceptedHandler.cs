using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Challenges;
using PostSharp.Patterns.Contracts;

namespace Kingo.Samples.Chess.Games
{
    public sealed class ChallengeAcceptedHandler : IMessageHandler<ChallengeAcceptedEvent>
    {
        private readonly IChallengeRepository _challenges;
        private readonly IGameRepository _games;

        public ChallengeAcceptedHandler([NotNull] IChallengeRepository challenges, [NotNull] IGameRepository games)
        {            
            _challenges = challenges;
            _games = games;
        }

        public async Task HandleAsync([NotNull] ChallengeAcceptedEvent message)
        {
            var challenge = await _challenges.GetByIdAsync(message.ChallengeId);
            var game = challenge.StartGame();

            _games.Add(game);
        }
    }
}
