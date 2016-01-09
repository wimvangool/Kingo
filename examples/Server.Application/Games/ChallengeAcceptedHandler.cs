using System;
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
            if (challenges == null)
            {
                throw new ArgumentNullException("challenges");
            }
            if (games == null)
            {
                throw new ArgumentNullException("games");
            }
            _challenges = challenges;
            _games = games;
        }

        public async Task HandleAsync(ChallengeAcceptedEvent message)
        {
            var challenge = await _challenges.GetByIdAsync(message.ChallengeId);
            var game = challenge.StartGame();

            _games.Add(game);
        }
    }
}
