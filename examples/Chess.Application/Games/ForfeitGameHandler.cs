using System.Threading.Tasks;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Games
{
    public sealed class ForfeitGameHandler : IMessageHandler<ForfeitGameCommand>
    {
        private readonly IGameRepository _games;

        public ForfeitGameHandler(IGameRepository games)
        {            
            _games = games;
        }

        public async Task HandleAsync(ForfeitGameCommand message, IMicroProcessorContext context)
        {            
            var game = await _games.GetByKeyAsync(message.GameId);

            game.Forfeit(Session.Current.UserId);
        }
    }
}
