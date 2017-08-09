using System.Threading.Tasks;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Games
{
    public sealed class PromotePawnHandler : IMessageHandler<PromotePawnCommand>
    {
        private readonly IGameRepository _games;

        public PromotePawnHandler(IGameRepository games)
        {
            _games = games;
        }

        public async Task HandleAsync(PromotePawnCommand message, IMicroProcessorContext context)
        {
            var game = await _games.GetByKeyAsync(message.GameId);

            game.PromotePawn(Session.Current.UserId, message.PromoteTo);
        }
    }
}
