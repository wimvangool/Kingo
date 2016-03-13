using System.Threading.Tasks;
using Kingo.Messaging;
using PostSharp.Patterns.Contracts;

namespace Kingo.Samples.Chess.Games
{
    public sealed class MovePieceHandler : MessageHandler<MovePieceCommand>
    {
        private readonly IGameRepository _games;

        public MovePieceHandler([NotNull] IGameRepository games)
        {            
            _games = games;
        }

        public override async Task HandleAsync([NotNull] MovePieceCommand message)
        {
            var from = Square.Parse(message.From);
            var to = Square.Parse(message.To);
            var game = await _games.GetByKeyAsync(message.GameId);
            
            game.MovePiece(Session.Current.PlayerId, from, to);
        }
    }
}
