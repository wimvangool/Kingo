using System;
using System.Threading.Tasks;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Games
{
    public sealed class ForfeitGameHandler : IMessageHandler<ForfeitGameCommand>
    {
        private readonly IGameRepository _games;

        public ForfeitGameHandler(IGameRepository games)
        {
            if (games == null)
            {
                throw new ArgumentNullException("games");
            }
            _games = games;
        }

        public async Task HandleAsync(ForfeitGameCommand message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var game = await _games.GetByKeyAsync(message.GameId);

            game.Forfeit();
        }
    }
}
