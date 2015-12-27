using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Players;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class ChallengePlayerHandler : MessageHandler<ChallengePlayerCommand>
    {
        private readonly IPlayerRepository _players;        

        public ChallengePlayerHandler(IPlayerRepository players)
        {
            if (players == null)
            {
                throw new ArgumentNullException("players");
            }
            _players = players;
        }

        public override Task HandleAsync(ChallengePlayerCommand message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            throw new NotImplementedException();
        }
    }
}
