using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Players;
using PostSharp.Patterns.Contracts;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class ChallengePlayerHandler : MessageHandler<ChallengePlayerCommand>
    {
        private readonly IPlayerRepository _players;
        private readonly IChallengeRepository _challenges;

        public ChallengePlayerHandler([NotNull] IPlayerRepository players, [NotNull] IChallengeRepository challenges)
        {            
            _players = players;
            _challenges = challenges;
        }

        public override async Task HandleAsync([NotNull] ChallengePlayerCommand message)
        {            
            var playerOne = await _players.GetByIdAsync(Session.Current.PlayerId);
            var playerTwo = await _players.GetByIdAsync(message.PlayerId);

            _challenges.Add(playerOne.Challenge(message.ChallengeId, playerTwo));
        }
    }
}
