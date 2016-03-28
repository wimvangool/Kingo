using System.Threading.Tasks;
using Kingo.Messaging;
using PostSharp.Patterns.Contracts;

namespace Kingo.Samples.Chess.Games
{
    public sealed class PromotePawnHandler : MessageHandler<PromotePawnCommand>
    {
        private readonly IGameRepository _games;

        public PromotePawnHandler([NotNull] IGameRepository games)
        {
            _games = games;
        }

        public override async Task HandleAsync([NotNull] PromotePawnCommand message)
        {
            var game = await _games.GetByKeyAsync(message.GameId);

            game.PromotePawn(Session.Current.PlayerId, message.PromoteTo);
        }
    }
}
