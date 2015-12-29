using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Players;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class ChallengePlayerHandler : MessageHandler<ChallengePlayerCommand>
    {
        private readonly IPlayerRepository _players;
        private readonly IChallengeRepository _challenges;

        public ChallengePlayerHandler(IPlayerRepository players, IChallengeRepository challenges)
        {
            if (players == null)
            {
                throw new ArgumentNullException("players");
            }
            if (challenges == null)
            {
                throw new ArgumentNullException("challenges");
            }
            _players = players;
            _challenges = challenges;
        }

        public override async Task HandleAsync(ChallengePlayerCommand message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var playerOne = await _players.GetByIdAsync(Session.Current.PlayerId);
            var playerTwo = await _players.GetByIdAsync(message.PlayerId);

            _challenges.Add(playerOne.Challenge(message.ChallengeId, playerTwo));
        }
    }
}
